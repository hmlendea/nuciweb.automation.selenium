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
    public sealed class TypeDeclarationConventionTests
    {
        private static string[] IntentionallyExtensibleTypes =>
        [
            "NotFoundException"
        ];

        [Test]
        public void GivenProductionSources_WhenChecked_ThenEveryTypeDeclaresExplicitAccessibility()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                BaseTypeDeclarationSyntax[] undocumentedTypes = root
                    .DescendantNodes()
                    .OfType<BaseTypeDeclarationSyntax>()
                    .Where(declaration => !DeclaresAccessibility(declaration))
                    .ToArray();

                Assert.That(undocumentedTypes, Is.Empty, $"{source}: every type must declare an explicit accessibility.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenEveryClassBreaksPreviewFeatures()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxTree tree = CSharpSyntaxTree.ParseText(
                    ReadSource(source),
                    new CSharpParseOptions(LanguageVersion.Latest),
                    path: source);

                Assert.That(
                    tree.GetDiagnostics().Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error),
                    Is.Empty,
                    $"{source}: preview language features are not allowed.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenEveryClassIsSealedUnlessIntentional()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                ClassDeclarationSyntax[] unsealedClasses = root
                    .DescendantNodes()
                    .OfType<ClassDeclarationSyntax>()
                    .Where(declaration => !declaration.Modifiers.Any(modifier =>
                        modifier.IsKind(SyntaxKind.SealedKeyword) ||
                        modifier.IsKind(SyntaxKind.StaticKeyword) ||
                        modifier.IsKind(SyntaxKind.AbstractKeyword)))
                    .Where(declaration => !IntentionallyExtensibleTypes.Contains(
                        declaration.Identifier.ValueText, StringComparer.Ordinal))
                    .ToArray();

                Assert.That(
                    unsealedClasses.Select(declaration => declaration.Identifier.ValueText),
                    Is.Empty,
                    $"{source}: every non-static, non-abstract class must be sealed unless intentionally extensible.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoMarkerInterfacesOrEmptyTypesExist()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                InterfaceDeclarationSyntax[] emptyInterfaces = root
                    .DescendantNodes()
                    .OfType<InterfaceDeclarationSyntax>()
                    .Where(declaration => declaration.Members.Count == 0)
                    .ToArray();

                Assert.That(
                    emptyInterfaces,
                    Is.Empty,
                    $"{source}: marker interfaces without members are not allowed.");
            }
        }

        private static bool DeclaresAccessibility(BaseTypeDeclarationSyntax declaration)
            => declaration.Modifiers.Any(modifier =>
                modifier.IsKind(SyntaxKind.PublicKeyword) ||
                modifier.IsKind(SyntaxKind.InternalKeyword) ||
                modifier.IsKind(SyntaxKind.PrivateKeyword) ||
                modifier.IsKind(SyntaxKind.ProtectedKeyword));

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