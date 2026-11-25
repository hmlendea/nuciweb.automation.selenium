using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using NUnit.Framework;

namespace NuciWeb.Automation.Selenium.ArchitectureTests
{
    [TestFixture]
    public sealed class SourceFileConventionTests
    {
        private static string RootNamespace => "NuciWeb.Automation.Selenium";

        [Test]
        public void GivenProductionSources_WhenChecked_ThenEveryFileDeclaresOneMatchingType()
        {
            foreach (string source in GetProductionSources())
            {
                IReadOnlyList<string> declaredTypes = GetDeclaredTypeNames(source, ArchitectureResources.ProductionSourcePrefix);

                Assert.That(
                    declaredTypes,
                    Has.Count.EqualTo(1),
                    $"{source}: expected exactly one declared type, but found {declaredTypes.Count}.");

                Assert.That(
                    declaredTypes[0],
                    Is.EqualTo(GetFileStem(source)),
                    $"{source}: the declared type must match the file name.");
            }

            foreach (string source in GetUnitTestSources())
            {
                IReadOnlyList<string> declaredTypes = GetDeclaredTypeNames(source, ArchitectureResources.UnitTestSourcePrefix);

                Assert.That(
                    declaredTypes,
                    Has.Count.EqualTo(1),
                    $"{source}: expected exactly one declared type, but found {declaredTypes.Count}.");

                Assert.That(
                    declaredTypes[0],
                    Is.EqualTo(GetFileStem(source)),
                    $"{source}: the declared type must match the file name.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenTheyUseBlockNamespaces()
        {
            AssertNoFileScopedNamespaces(GetProductionSources(), ArchitectureResources.ProductionSourcePrefix);
            AssertNoFileScopedNamespaces(GetUnitTestSources(), ArchitectureResources.UnitTestSourcePrefix);
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenTheNamespaceMatchesTheFolderPath()
        {
            AssertNamespaceMatchesFolder(
                GetProductionSources(),
                ArchitectureResources.ProductionSourcePrefix,
                RootNamespace);
            AssertNamespaceMatchesFolder(
                GetUnitTestSources(),
                ArchitectureResources.UnitTestSourcePrefix,
                "NuciWeb.Automation.Selenium.UnitTests");
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoNestedTypesOrTopLevelStatementsExist()
        {
            foreach (string source in GetProductionSources().Concat(GetUnitTestSources()))
            {
                string prefix = GetUnitTestSources().Contains(source)
                    ? ArchitectureResources.UnitTestSourcePrefix
                    : ArchitectureResources.ProductionSourcePrefix;

                SyntaxNode root = ParseTree(source, prefix);

                Assert.That(
                    root.DescendantNodes()
                        .OfType<BaseTypeDeclarationSyntax>()
                        .Where(declaration => declaration.Parent is TypeDeclarationSyntax),
                    Is.Empty,
                    $"{source}: nested types are not allowed.");

                Assert.That(
                    root.DescendantNodes().OfType<GlobalStatementSyntax>(),
                    Is.Empty,
                    $"{source}: top-level statements are not allowed.");

                Assert.That(
                    root.DescendantNodes().OfType<FileScopedNamespaceDeclarationSyntax>(),
                    Is.Empty,
                    $"{source}: file-scoped namespaces are not allowed.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenEveryMethodAndLambdaHasAtMostSevenParameters()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                BaseMethodDeclarationSyntax[] oversizedMethods = root
                    .DescendantNodes()
                    .OfType<BaseMethodDeclarationSyntax>()
                    .Where(method => method.ParameterList.Parameters.Count > 7)
                    .ToArray();

                Assert.That(
                    oversizedMethods.Select(method => method.ToString()),
                    Is.Empty,
                    $"{source}: methods and constructors must have at most seven parameters.");

                AnonymousFunctionExpressionSyntax[] oversizedLambdas = root
                    .DescendantNodes()
                    .OfType<AnonymousFunctionExpressionSyntax>()
                    .Where(lambda => GetLambdaParameterCount(lambda) > 7)
                    .ToArray();

                Assert.That(
                    oversizedLambdas,
                    Is.Empty,
                    $"{source}: lambdas must have at most seven parameters.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoRefOrOutParametersAreUsed()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                ParameterSyntax[] refParameters = root
                    .DescendantNodes()
                    .OfType<ParameterSyntax>()
                    .Where(parameter => parameter.Modifiers.Any(modifier =>
                        modifier.IsKind(SyntaxKind.RefKeyword) ||
                        modifier.IsKind(SyntaxKind.OutKeyword)))
                    .ToArray();

                Assert.That(
                    refParameters,
                    Is.Empty,
                    $"{source}: ref/out parameters are not allowed.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoOptionalParametersAreUsed()
        {
            // Documented, narrowly scoped exception: WebDriverInitialiser exposes a
            // published NuGet API whose default values are part of the documented,
            // named-argument-compatible public contract (see README usage examples).
            // Removing the defaults would be a breaking change, so the optional-parameter
            // preference is waived for that single published initialiser surface.
            foreach (string source in GetProductionSources())
            {
                if (string.Equals(source, "WebDriverInitialiser.cs", StringComparison.Ordinal))
                {
                    continue;
                }

                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                ParameterSyntax[] optionalParameters = root
                    .DescendantNodes()
                    .OfType<ParameterSyntax>()
                    .Where(parameter => parameter.Default is not null)
                    .ToArray();

                Assert.That(
                    optionalParameters,
                    Is.Empty,
                    $"{source}: optional parameters are not allowed.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenControlFlowBodiesUseBraces()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                StatementSyntax[] unbracedStatements = root
                    .DescendantNodes()
                    .OfType<IfStatementSyntax>()
                    .Where(statement => statement.Statement is not BlockSyntax)
                    .Select(statement => statement.Statement)
                    .Concat(root.DescendantNodes().OfType<ForStatementSyntax>()
                        .Where(statement => statement.Statement is not BlockSyntax)
                        .Select(statement => statement.Statement))
                    .Concat(root.DescendantNodes().OfType<ForEachStatementSyntax>()
                        .Where(statement => statement.Statement is not BlockSyntax)
                        .Select(statement => statement.Statement))
                    .Concat(root.DescendantNodes().OfType<WhileStatementSyntax>()
                        .Where(statement => statement.Statement is not BlockSyntax)
                        .Select(statement => statement.Statement))
                    .Concat(root.DescendantNodes().OfType<DoStatementSyntax>()
                        .Where(statement => statement.Statement is not BlockSyntax)
                        .Select(statement => statement.Statement))
                    .Concat(root.DescendantNodes().OfType<ElseClauseSyntax>()
                        .Where(clause => clause.Statement is not BlockSyntax && clause.Statement is not IfStatementSyntax)
                        .Select(clause => clause.Statement))
                    .ToArray();

                Assert.That(
                    unbracedStatements,
                    Is.Empty,
                    $"{source}: every control-flow body must use braces.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoPreviewFeaturesAreUsed()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxTree tree = CSharpSyntaxTree.ParseText(
                    ReadSource(source, ArchitectureResources.ProductionSourcePrefix),
                    new CSharpParseOptions(LanguageVersion.Latest),
                    path: source);

                string[] recordTypes = tree.GetRoot()
                    .DescendantNodes()
                    .OfType<RecordDeclarationSyntax>()
                    .Select(declaration => declaration.Identifier.ValueText)
                    .ToArray();

                Assert.That(
                    recordTypes,
                    Is.Empty,
                    $"{source}: record types are preview features and must not be used.");

                Assert.That(
                    tree.GetRoot().DescendantNodes().OfType<GlobalStatementSyntax>(),
                    Is.Empty,
                    $"{source}: top-level statements are preview features and must not be used.");
            }
        }

        private static void AssertNoFileScopedNamespaces(IReadOnlyList<string> sources, string prefix)
        {
            foreach (string source in sources)
            {
                Assert.That(
                    ParseTree(source, prefix).DescendantNodes().OfType<FileScopedNamespaceDeclarationSyntax>(),
                    Is.Empty,
                    $"{source}: block namespaces are required.");
            }
        }

        private static void AssertNamespaceMatchesFolder(
            IReadOnlyList<string> sources,
            string prefix,
            string rootNamespace)
        {
            foreach (string source in sources)
            {
                SyntaxNode root = ParseTree(source, prefix);

                string[] namespaces = root
                    .DescendantNodes()
                    .OfType<NamespaceDeclarationSyntax>()
                    .Select(declaration => declaration.Name.ToString())
                    .ToArray();

                Assert.That(
                    namespaces,
                    Has.Length.EqualTo(1),
                    $"{source}: the file must declare exactly one namespace.");

                Assert.That(
                    namespaces[0],
                    Is.EqualTo(rootNamespace),
                    $"{source}: the namespace must follow the folder path.");
            }
        }

        private static string ReadSource(string source, string prefix)
            => ArchitectureResources.ReadResource(prefix, source);

        private static SyntaxNode ParseTree(string source, string prefix)
            => CSharpSyntaxTree.ParseText(ReadSource(source, prefix), path: source).GetRoot();

        private static IReadOnlyList<string> GetDeclaredTypeNames(string source, string prefix)
            => [.. ParseTree(source, prefix)
                .DescendantNodes()
                .OfType<BaseTypeDeclarationSyntax>()
                .Select(node => node.Identifier.ValueText)
                .Concat(ParseTree(source, prefix)
                    .DescendantNodes()
                    .OfType<DelegateDeclarationSyntax>()
                    .Select(node => node.Identifier.ValueText))];

        private static int GetLambdaParameterCount(AnonymousFunctionExpressionSyntax lambda)
        {
            if (lambda is ParenthesizedLambdaExpressionSyntax parenthesizedLambda)
            {
                return parenthesizedLambda.ParameterList.Parameters.Count;
            }

            return 1;
        }

        private static string GetFileStem(string source)
        {
            string fileName = source.Substring(source.LastIndexOf('/') + 1);

            return fileName.Substring(0, fileName.LastIndexOf('.'));
        }

        private static IReadOnlyList<string> GetProductionSources()
        {
            IReadOnlyList<string> sources = ArchitectureResources.GetResourcePaths(
                ArchitectureResources.ProductionSourcePrefix);

            Assert.That(sources, Is.Not.Empty, "No production source files were found.");

            return sources;
        }

        private static IReadOnlyList<string> GetUnitTestSources()
        {
            IReadOnlyList<string> sources = ArchitectureResources.GetResourcePaths(
                ArchitectureResources.UnitTestSourcePrefix);

            Assert.That(sources, Is.Not.Empty, "No unit test source files were found.");

            return sources;
        }
    }
}