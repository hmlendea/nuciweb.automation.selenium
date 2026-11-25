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
    public sealed class CollectionConventionTests
    {
        private static string[] ConcreteCollectionTypes =>
        [
            "List<",
            "IList<",
            "IReadOnlyList<",
            "HashSet<",
            "Dictionary<"
        ];

        [Test]
        public void GivenProductionSources_WhenChecked_ThenCollectionParametersUseEnumerables()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                IEnumerable<string> concreteSignatures = root.DescendantNodes()
                    .OfType<ParameterSyntax>()
                    .Select(parameter => parameter.Type?.ToString() ?? string.Empty)
                    .Where(typeText => ConcreteCollectionTypes.Any(collection => typeText.StartsWith(collection, StringComparison.Ordinal)))
                    .Concat(root.DescendantNodes()
                        .OfType<MethodDeclarationSyntax>()
                        .Select(method => method.ReturnType.ToString())
                        .Where(typeText => ConcreteCollectionTypes.Any(collection => typeText.StartsWith(collection, StringComparison.Ordinal))))
                    .ToArray();

                Assert.That(
                    concreteSignatures,
                    Is.Empty,
                    $"{source}: collection parameters and returns must use IEnumerable<T>.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenInlineCollectionsUseCollectionExpressions()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                string[] collectionConstructorCalls = root
                    .DescendantNodes()
                    .OfType<ObjectCreationExpressionSyntax>()
                    .Where(creation => creation.Type.ToString().StartsWith("List<", StringComparison.Ordinal) ||
                        creation.Type.ToString().StartsWith("Dictionary<", StringComparison.Ordinal))
                    .Select(creation => creation.Type.ToString())
                    .ToArray();

                Assert.That(
                    collectionConstructorCalls,
                    Is.Empty,
                    $"{source}: use C# 12 collection expressions '[...]' instead of 'new List<T>()' / 'new Dictionary<K,V>()'.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoMappingClassesAreDeclared()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                string[] mappingTypes = root
                    .DescendantNodes()
                    .OfType<BaseTypeDeclarationSyntax>()
                    .Select(declaration => declaration.Identifier.ValueText)
                    .Where(name => name.EndsWith("MappingExtensions", StringComparison.Ordinal))
                    .ToArray();

                Assert.That(
                    mappingTypes,
                    Is.Empty,
                    $"{source}: mapping methods must live in a dedicated '*MappingExtensions' type.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenDateTimeScalingUsesInvariantCulture()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                InvocationExpressionSyntax[] dateFormattingCalls = root
                    .DescendantNodes()
                    .OfType<InvocationExpressionSyntax>()
                    .Where(invocation => invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
                        memberAccess.Name.Identifier.ValueText == "ToString")
                    .Where(invocation => invocation.ArgumentList.Arguments.Count > 0)
                    .Where(invocation => invocation.ToString().Contains("format", StringComparison.Ordinal))
                    .Where(invocation => !invocation.ToString().Contains("InvariantCulture", StringComparison.Ordinal))
                    .Select(invocation => invocation)
                    .ToArray();

                Assert.That(
                    dateFormattingCalls,
                    Is.Empty,
                    $"{source}: date-time formatting with a format string must pass CultureInfo.InvariantCulture.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoJsonAttributesAreApplied()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                AttributeSyntax[] jsonAttributes = root
                    .DescendantNodes()
                    .OfType<AttributeSyntax>()
                    .Where(attribute => attribute.Name.ToString().EndsWith("JsonPropertyName", StringComparison.Ordinal))
                    .ToArray();

                Assert.That(
                    jsonAttributes,
                    Is.Empty,
                    $"{source}: the library exposes no API DTOs, so no JSON serialisation attributes are expected.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoNestedConditionalExpressionsExist()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                ConditionalExpressionSyntax[] nestedTernaries = root
                    .DescendantNodes()
                    .OfType<ConditionalExpressionSyntax>()
                    .Where(expression => expression.DescendantNodes().OfType<ConditionalExpressionSyntax>().Any())
                    .ToArray();

                Assert.That(
                    nestedTernaries,
                    Is.Empty,
                    $"{source}: conditional expressions must not be nested.");
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