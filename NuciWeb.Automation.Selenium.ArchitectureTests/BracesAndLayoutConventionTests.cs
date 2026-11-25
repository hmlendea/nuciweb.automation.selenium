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
    public sealed class BracesAndLayoutConventionTests
    {
        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoBlankLinePrecedesClosingCurly()
        {
            foreach (string source in GetProductionSources())
            {
                string[] lines = ReadSource(source).Split('\n');

                for (int index = 0; index < lines.Length - 1; index++)
                {
                    string trimmedNext = lines[index + 1].Trim();

                    if (trimmedNext.Length == 0 || trimmedNext[0] != '}')
                    {
                        continue;
                    }

                    Assert.That(
                        lines[index].Trim().Length > 0,
                        Is.True,
                        $"{source}:{index + 2}: no blank line may precede a closing curly bracket.");
                }
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenElseIfIsOnOneLine()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

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
        public void GivenProductionSources_WhenChecked_ThenNoTrailingBlankLineBeforeDelimiter()
        {
            foreach (string source in GetProductionSources())
            {
                string content = ReadSource(source);

                Assert.That(
                    content,
                    Does.Not.Match(@"\n[ \t]*\n[ \t]*[}\])]"),
                    $"{source}: no blank line may precede a closing delimiter.");
            }
        }

        private static bool SameLine(
            FileLinePositionSpan first,
            FileLinePositionSpan second)
            => first.StartLinePosition.Line == second.StartLinePosition.Line;

        private static SyntaxNode ParseTree(string source, string prefix)
            => CSharpSyntaxTree.ParseText(
                ArchitectureResources.ReadResource(prefix, source),
                path: source).GetRoot();

        private static string ReadSource(string source)
            => ArchitectureResources.ReadResource(ArchitectureResources.ProductionSourcePrefix, source);

        private static IReadOnlyList<string> GetProductionSources()
        {
            IReadOnlyList<string> sources = ArchitectureResources.GetResourcePaths(
                ArchitectureResources.ProductionSourcePrefix);

            Assert.That(sources, Is.Not.Empty, "No production source files were found.");

            return sources;
        }
    }
}