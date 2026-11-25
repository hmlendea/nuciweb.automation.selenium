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
    public sealed class InterfaceAndEnumerationConventionTests
    {
        [Test]
        public void GivenProductionSources_WhenChecked_ThenEveryInterfaceMemberHasXmlSummary()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                InterfaceDeclarationSyntax[] publicInterfaces = root
                    .DescendantNodes()
                    .OfType<InterfaceDeclarationSyntax>()
                    .Where(declaration => declaration.Modifiers.Any(SyntaxKind.PublicKeyword))
                    .ToArray();

                foreach (InterfaceDeclarationSyntax interfaceDeclaration in publicInterfaces)
                {
                    Assert.That(
                        HasXmlSummary(interfaceDeclaration),
                        Is.True,
                        $"{source}: public interface '{interfaceDeclaration.Identifier.ValueText}' must have a summary.");

                    foreach (MemberDeclarationSyntax member in interfaceDeclaration.Members)
                    {
                        Assert.That(
                            HasXmlSummary(member),
                            Is.True,
                            $"{source}: interface member in '{interfaceDeclaration.Identifier.ValueText}' must have a summary.");
                    }
                }
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenDocumentedBlocksAreNotEmpty()
        {
            foreach (string source in GetProductionSources())
            {
                string content = ReadSource(source);

                Assert.That(
                    content,
                    Does.Not.Match(@"(?m)^\s*///\s*<summary>\s*</summary>"),
                    $"{source}: documentation summaries must carry content.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenEachDocumentedBlockHasASummaryOrInheritdoc()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                DocumentationCommentTriviaSyntax[] blocks = root
                    .DescendantNodes()
                    .SelectMany(node => node.GetLeadingTrivia())
                    .Select(trivia => trivia.GetStructure())
                    .OfType<DocumentationCommentTriviaSyntax>()
                    .ToArray();

                foreach (DocumentationCommentTriviaSyntax documentation in blocks)
                {
                    string text = documentation.Content.ToFullString();

                    if (text.Contains("<inheritdoc", StringComparison.Ordinal))
                    {
                        continue;
                    }

                    Assert.That(
                        text.Contains("<summary>", StringComparison.Ordinal),
                        Is.True,
                        $"{source}: every documentation block must carry a summary element or inheritdoc.");
                }
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoInterfacesNodeSpaceIsDeclared()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                Assert.That(
                    root.DescendantNodes()
                        .OfType<BaseNamespaceDeclarationSyntax>()
                        .Select(declaration => declaration.Name.ToString())
                        .Where(nameSpace => nameSpace.EndsWith(".Interfaces", StringComparison.Ordinal)),
                    Is.Empty,
                    $"{source}: interfaces must stay colocated, never in a dedicated '.Interfaces' namespace.");
            }
        }

        private static bool HasXmlSummary(SyntaxNode node)
            => node.GetLeadingTrivia()
                .Any(trivia => trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia));

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