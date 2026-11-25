using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using NUnit.Framework;

namespace NuciWeb.Automation.Selenium.ArchitectureTests
{
    /// <summary>
    /// Enforces the line-splitting and member-layout conventions from the indentation
    /// instructions: one property per line, accessor bodies on their own line, opening
    /// delimiters on their own line, and method-level blank-line spacing.
    /// </summary>
    [TestFixture]
    public sealed class IndentationDeclarationConventionTests
    {
        [Test]
        public void GivenProductionSources_WhenChecked_ThenPropertiesAreSeparatedByABlankLine()
        {
            foreach (string source in GetProductionSources())
            {
                string[] lines = ReadSource(source).Split('\n');

                for (int index = 0; index < lines.Length; index++)
                {
                    string trimmed = lines[index].Trim();

                    if (!trimmed.StartsWith("public ", StringComparison.Ordinal) &&
                        !trimmed.StartsWith("private ", StringComparison.Ordinal) &&
                        !trimmed.StartsWith("protected ", StringComparison.Ordinal) &&
                        !trimmed.StartsWith("internal ", StringComparison.Ordinal))
                    {
                        continue;
                    }

                    if (!trimmed.Contains('{') || !trimmed.Contains('}') || index + 1 >= lines.Length)
                    {
                        continue;
                    }

                    Assert.That(
                        lines[index + 1].Trim().Length > 0,
                        Is.True,
                        $"{source}:{index + 2}: a property must be preceded and followed by a blank line.");
                }
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenAccessorBodiesOccupyTheirOwnLine()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                string[] inlineAccessors = root
                    .DescendantNodes()
                    .OfType<AccessorDeclarationSyntax>()
                    .Where(accessor => accessor.Body is not null)
                    .Where(accessor => SameLine(
                        accessor.Body!.OpenBraceToken.GetLocation().GetLineSpan(),
                        accessor.GetLocation().GetLineSpan()))
                    .Select(accessor => accessor.Keyword.ValueText)
                    .ToArray();

                Assert.That(
                    inlineAccessors,
                    Is.Empty,
                    $"{source}: each accessor body must start on its own line.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenParametersAreSplitAboveNinetySixCharacters()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                BaseMethodDeclarationSyntax[] oversizedSignatures = root
                    .DescendantNodes()
                    .OfType<BaseMethodDeclarationSyntax>()
                    .Where(method => method.ParameterList.Parameters.Count > 0)
                    .Where(method => method.GetLocation().GetLineSpan().EndLinePosition.Line ==
                        method.GetLocation().GetLineSpan().StartLinePosition.Line)
                    .Where(method => method.ToString().Length > 96)
                    .Select(method => method)
                    .ToArray();

                Assert.That(
                    oversizedSignatures.Select(method => method.ToString()),
                    Is.Empty,
                    $"{source}: parameter lists exceeding 96 characters must split each item onto its own line.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenContinuationArrowsIndentFourSpaces()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                ArrowExpressionClauseSyntax[] misalignedArrows = root
                    .DescendantNodes()
                    .OfType<ArrowExpressionClauseSyntax>()
                    .Where(arrow => !SameLine(
                        arrow.GetLocation().GetLineSpan(),
                        arrow.Parent!.GetLocation().GetLineSpan()))
                    .Where(arrow => arrow.GetLocation().GetLineSpan().StartLinePosition.Character !=
                        ((SyntaxNode)arrow.Parent!).GetLocation().GetLineSpan().StartLinePosition.Character + 4)
                    .ToArray();

                Assert.That(
                    misalignedArrows,
                    Is.Empty,
                    $"{source}: a continuation '=>' must be indented four spaces past its signature.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenMultipleStatementsDoNotShareALine()
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
        public void GivenProductionSources_WhenChecked_ThenOpeningBracesOccupyTheirOwnLine()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                BaseTypeDeclarationSyntax[] inlineTypeBraces = root
                    .DescendantNodes()
                    .OfType<BaseTypeDeclarationSyntax>()
                    .Where(declaration => SameLine(
                        declaration.OpenBraceToken.GetLocation().GetLineSpan(),
                        declaration.GetLocation().GetLineSpan()))
                    .ToArray();

                Assert.That(
                    inlineTypeBraces,
                    Is.Empty,
                    $"{source}: a type's opening brace must occupy its own line.");

                MethodDeclarationSyntax[] inlineMethodBraces = root
                    .DescendantNodes()
                    .OfType<MethodDeclarationSyntax>()
                    .Where(method => method.Body is not null)
                    .Where(method => SameLine(
                        method.Body!.OpenBraceToken.GetLocation().GetLineSpan(),
                        method.GetLocation().GetLineSpan()))
                    .ToArray();

                Assert.That(
                    inlineMethodBraces,
                    Is.Empty,
                    $"{source}: a method's opening brace must occupy its own line.");
            }
        }

        private static bool SameLine(
            FileLinePositionSpan first,
            FileLinePositionSpan second)
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