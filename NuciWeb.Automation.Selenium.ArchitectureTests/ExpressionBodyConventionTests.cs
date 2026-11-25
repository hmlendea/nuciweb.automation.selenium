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
    public sealed class ExpressionBodyConventionTests
    {
        [Test]
        public void GivenProductionSources_WhenChecked_ThenSingleStatementMethodsAreExpressionBodied()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                MethodDeclarationSyntax[] violations = root
                    .DescendantNodes()
                    .OfType<MethodDeclarationSyntax>()
                    .Where(method => method.Body is not null && method.Body.Statements.Count == 1)
                    .Where(method => !IsControlFlow(method.Body!.Statements[0]))
                    .ToArray();

                Assert.That(
                    violations.Select(method => method.Identifier.ValueText),
                    Is.Empty,
                    $"{source}: single-statement methods must be expression-bodied.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenSingleStatementGettersAreExpressionBodied()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                PropertyDeclarationSyntax[] violations = root
                    .DescendantNodes()
                    .OfType<PropertyDeclarationSyntax>()
                    .Where(property => property.ExpressionBody is null && property.AccessorList is not null)
                    .Where(HasSingleStatementGetter)
                    .ToArray();

                Assert.That(
                    violations.Select(property => property.Identifier.ValueText),
                    Is.Empty,
                    $"{source}: computed read-only properties must be expression-bodied.");
            }
        }

        private static bool IsControlFlow(StatementSyntax statement)
            => statement is IfStatementSyntax ||
                statement is SwitchStatementSyntax ||
                statement is TryStatementSyntax ||
                statement is ForStatementSyntax ||
                statement is ForEachStatementSyntax ||
                statement is WhileStatementSyntax ||
                statement is DoStatementSyntax ||
                statement is LockStatementSyntax ||
                statement is UsingStatementSyntax;

        private static bool HasSingleStatementGetter(PropertyDeclarationSyntax property)
        {
            if (property.AccessorList is null || property.AccessorList.Accessors.Count != 1)
            {
                return false;
            }

            AccessorDeclarationSyntax accessor = property.AccessorList.Accessors[0];

            return accessor.IsKind(SyntaxKind.GetAccessorDeclaration) &&
                accessor.Body is not null &&
                accessor.Body.Statements.Count == 1;
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