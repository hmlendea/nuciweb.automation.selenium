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
    public sealed class NamespaceAndUsingConventionTests
    {
        [Test]
        public void GivenProductionSources_WhenChecked_ThenUsingsAppearAboveAndOutsideTheNamespace()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                foreach (NamespaceDeclarationSyntax namespaceDeclaration in root
                    .DescendantNodes().OfType<NamespaceDeclarationSyntax>())
                {
                    Assert.That(
                        namespaceDeclaration.Usings,
                        Is.Empty,
                        $"{source}: using directives must be placed above and outside the namespace.");
                }
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenUsingsAreGroupedByDocumentedGroups()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                int[] groupOrders = root
                    .DescendantNodes()
                    .OfType<UsingDirectiveSyntax>()
                    .Select(usingDirective => GetUsingGroupOrder(usingDirective.Name?.ToString() ?? string.Empty))
                    .ToArray();

                Assert.That(
                    groupOrders,
                    Is.Ordered,
                    $"{source}: using groups must be ordered as System.*, Microsoft.*, third-party, then the current solution.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenExactlyOneBlankLinePrecedesTheNamespace()
        {
            foreach (string source in GetProductionSources())
            {
                string content = ReadSource(source);

                Assert.That(
                    content,
                    Does.Not.Match(@"\n[ \t]*\n[ \t]*\n[ \t]*namespace "),
                    $"{source}: exactly one blank line must precede the namespace declaration.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoInterfacesNamespaceIsDeclared()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                string[] interfacesNamespaces = root
                    .DescendantNodes()
                    .OfType<BaseNamespaceDeclarationSyntax>()
                    .Select(declaration => declaration.Name.ToString())
                    .Where(nameSpace => nameSpace.EndsWith(".Interfaces", StringComparison.Ordinal) ||
                        string.Equals(nameSpace, "Interfaces", StringComparison.Ordinal))
                    .ToArray();

                Assert.That(
                    interfacesNamespaces,
                    Is.Empty,
                    $"{source}: a '.Interfaces' namespace is not allowed; interfaces must be colocated.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoUtilityOrHelperTypesAreDeclared()
        {
            string[] forbiddenNames = ["Helper", "Helpers", "Utils", "Utilities", "Common", "Misc", "Shared"];

            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                string[] forbiddenTypes = root
                    .DescendantNodes()
                    .OfType<BaseTypeDeclarationSyntax>()
                    .Select(declaration => declaration.Identifier.ValueText)
                    .Where(forbiddenNames.Contains)
                    .ToArray();

                Assert.That(
                    forbiddenTypes,
                    Is.Empty,
                    $"{source}: catch-all Helper, Utils, Common, Misc, or Shared types are not allowed.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNeitherUsingsStaticNorGlobalUsingsAreUsed()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                Assert.That(
                    root.DescendantNodes()
                        .OfType<UsingDirectiveSyntax>()
                        .Where(usingDirective =>
                            usingDirective.StaticKeyword.IsKind(SyntaxKind.StaticKeyword) ||
                            usingDirective.GlobalKeyword.IsKind(SyntaxKind.GlobalKeyword)),
                    Is.Empty,
                    $"{source}: 'using static' and 'global using' dialects are not allowed.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoTopLevelStatementsExist()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                Assert.That(
                    root.DescendantNodes().OfType<GlobalStatementSyntax>(),
                    Is.Empty,
                    $"{source}: top-level statements are not allowed.");
            }
        }

        private static int GetUsingGroupOrder(string nameSpace)
        {
            if (nameSpace.StartsWith("System", StringComparison.Ordinal)) { return 0; }
            if (nameSpace.StartsWith("Microsoft", StringComparison.Ordinal)) { return 1; }
            if (nameSpace.StartsWith("Nuci", StringComparison.Ordinal)) { return 3; }

            return 2;
        }

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