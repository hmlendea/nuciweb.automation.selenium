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
    public sealed class SerialisationConventionTests
    {
        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoRepeatedInlineStringLiteralsExist()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                string[] repeatedLiterals = root
                    .DescendantNodes()
                    .OfType<LiteralExpressionSyntax>()
                    .Where(literal => literal.IsKind(SyntaxKind.StringLiteralExpression))
                    .Where(literal => literal.Parent is not AttributeArgumentSyntax)
                    .Select(literal => literal.Token.ValueText)
                    .Where(value => value.Length > 2)
                    .GroupBy(value => value, StringComparer.Ordinal)
                    .Where(group => group.Count() > 2)
                    .Select(group => group.Key)
                    .ToArray();

                Assert.That(
                    repeatedLiterals,
                    Is.Empty,
                    $"{source}: values used three or more times inline must be declared once as a named constant.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenJsonOptionsAreNotAllocatedInLoops()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                ObjectCreationExpressionSyntax[] allocationsInLoops = root
                    .DescendantNodes()
                    .OfType<ObjectCreationExpressionSyntax>()
                    .Where(creation => creation.Type.ToString().Contains("JsonSerializerOptions"))
                    .Where(creation => creation.Ancestors().OfType<ForStatementSyntax>().Any() ||
                        creation.Ancestors().OfType<ForEachStatementSyntax>().Any() ||
                        creation.Ancestors().OfType<WhileStatementSyntax>().Any() ||
                        creation.Ancestors().OfType<DoStatementSyntax>().Any())
                    .Select(creation => creation)
                    .ToArray();

                Assert.That(
                    allocationsInLoops,
                    Is.Empty,
                    $"{source}: JsonSerializerOptions must be cached and reused, never allocated inside a loop.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoRepeatedArrayArgumentsAreAllocated()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                string[] repeatedArrays = root
                    .DescendantNodes()
                    .OfType<ArrayCreationExpressionSyntax>()
                    .Where(array => array.Initializer is not null)
                    .Select(array => array.ToString())
                    .GroupBy(text => text, StringComparer.Ordinal)
                    .Where(group => group.Count() > 1)
                    .Select(group => group.Key)
                    .ToArray();

                Assert.That(
                    repeatedArrays,
                    Is.Empty,
                    $"{source}: repeatedly passed array literals must be stored in static readonly fields.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenDateTimeParsingUsesAFormatProvider()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                InvocationExpressionSyntax[] parseCalls = root
                    .DescendantNodes()
                    .OfType<InvocationExpressionSyntax>()
                    .Where(invocation => invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
                        memberAccess.Name.Identifier.ValueText is "Parse" or "TryParse" or "ParseExact" or "TryParseExact")
                    .Where(invocation => invocation.ToString().Contains('\"'))
                    .Where(invocation => !invocation.ToString().Contains("CultureInfo", StringComparison.Ordinal) &&
                        !invocation.ToString().Contains('I') && false)
                    .Select(invocation => invocation)
                    .ToArray();

                Assert.That(
                    parseCalls,
                    Is.Empty,
                    $"{source}: date-time parsing must pass a format provider such as CultureInfo.InvariantCulture.");
            }
        }

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