using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using NUnit.Framework;

namespace NuciWeb.Automation.Selenium.ArchitectureTests
{
    [TestFixture]
    public sealed class IndentationConventionTests
    {
        [Test]
        public void GivenProductionSources_WhenChecked_ThenOpeningBracesOccupyTheirOwnLine()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                string[] inlineTypeBraces = root
                    .DescendantNodes()
                    .OfType<BaseTypeDeclarationSyntax>()
                    .Where(declaration => SameLine(
                        declaration.OpenBraceToken.GetLocation().GetLineSpan(),
                        declaration.GetLocation().GetLineSpan()))
                    .Select(declaration => declaration.Identifier.ValueText)
                    .ToArray();

                Assert.That(
                    inlineTypeBraces,
                    Is.Empty,
                    $"{source}: a type's opening brace must occupy its own line.");

                string[] inlineMethodBraces = root
                    .DescendantNodes()
                    .OfType<MethodDeclarationSyntax>()
                    .Where(method => method.Body is not null)
                    .Where(method => SameLine(
                        method.Body!.OpenBraceToken.GetLocation().GetLineSpan(),
                        method.GetLocation().GetLineSpan()))
                    .Select(method => method.Identifier.ValueText)
                    .ToArray();

                Assert.That(
                    inlineMethodBraces,
                    Is.Empty,
                    $"{source}: a method's opening brace must occupy its own line.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenElseClausesAreBracedAndElseIfIsOnOneLine()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                ElseClauseSyntax[] braceLessElse = root
                    .DescendantNodes()
                    .OfType<ElseClauseSyntax>()
                    .Where(clause => clause.Statement is not BlockSyntax && clause.Statement is not IfStatementSyntax)
                    .ToArray();

                Assert.That(
                    braceLessElse,
                    Is.Empty,
                    $"{source}: every 'else' clause must use braces.");

                IfStatementSyntax[] splitElseIf = root
                    .DescendantNodes()
                    .OfType<IfStatementSyntax>()
                    .Where(statement => statement.Parent is ElseClauseSyntax)
                    .Where(statement => !SameLine(
                        statement.IfKeyword.GetLocation().GetLineSpan(),
                        ((ElseClauseSyntax)statement.Parent!).ElseKeyword.GetLocation().GetLineSpan()))
                    .ToArray();

                Assert.That(
                    splitElseIf,
                    Is.Empty,
                    $"{source}: 'else if' must stay on one line.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenContinuationOperatorsAreAtLineEnd()
        {
            foreach (string source in GetProductionSources())
            {
                string[] lines = ReadSource(source).Split('\n');

                for (int index = 0; index < lines.Length; index++)
                {
                    string trimmed = lines[index].TrimStart();

                    Assert.That(
                        trimmed.StartsWith("&&", StringComparison.Ordinal) ||
                        trimmed.StartsWith("||", StringComparison.Ordinal),
                        Is.False,
                        $"{source}:{index + 1}: a continuation operator must be at the end of the previous line.");
                }
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoTwoStatementsShareALine()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                BlockSyntax[] multiStatementBlocks = root
                    .DescendantNodes()
                    .OfType<BlockSyntax>()
                    .Where(block => block.Statements.Count > 1)
                    .Where(block => block.Statements
                        .Zip(block.Statements.Skip(1), (first, second) => first.GetLocation()
                            .GetLineSpan().StartLinePosition.Line ==
                            second.GetLocation().GetLineSpan().StartLinePosition.Line)
                        .Any(shareLine => shareLine))
                    .ToArray();

                Assert.That(
                    multiStatementBlocks,
                    Is.Empty,
                    $"{source}: statements must not share a line.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenContinuationArrowsAreIndentedFourSpaces()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                string[] misalignedArrows = root
                    .DescendantNodes()
                    .OfType<ArrowExpressionClauseSyntax>()
                    .Where(arrow => !SameLine(
                        arrow.GetLocation().GetLineSpan(),
                        arrow.Parent!.GetLocation().GetLineSpan()))
                    .Where(arrow => arrow.GetLocation().GetLineSpan().StartLinePosition.Character !=
                        ((SyntaxNode)arrow.Parent!).GetLocation().GetLineSpan().StartLinePosition.Character + 4)
                    .Select(arrow => arrow.ToString())
                    .ToArray();

                Assert.That(
                    misalignedArrows,
                    Is.Empty,
                    $"{source}: a continuation '=>' must be indented four spaces past its signature.");
            }
        }

        private static bool SameLine(FileLinePositionSpan first, FileLinePositionSpan second)
            => first.StartLinePosition.Line == second.StartLinePosition.Line;

        private static string ReadSource(string source)
            => ArchitectureResources.ReadResource(ArchitectureResources.ProductionSourcePrefix, source);

        private static SyntaxNode ParseTree(string source, string prefix)
            => CSharpSyntaxTree.ParseText(
                ArchitectureResources.ReadResource(prefix, source),
                path: source).GetRoot();

        private static IReadOnlyList<string> GetProductionSources()
        {
            IReadOnlyList<string> sources = ArchitectureResources.GetResourcePaths(
                ArchitectureResources.ProductionSourcePrefix);

            Assert.That(sources, Is.Not.Empty, "No production source files were found.");

            return sources;
        }
    }
}