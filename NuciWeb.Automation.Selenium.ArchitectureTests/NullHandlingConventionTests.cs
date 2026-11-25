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
    public sealed class NullHandlingConventionTests
    {
        [Test]
        public void GivenProductionSources_WhenChecked_ThenNullComparisonsUsePatternMatching()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                BinaryExpressionSyntax[] equalityChecks = root
                    .DescendantNodes()
                    .OfType<BinaryExpressionSyntax>()
                    .Where(expression => expression.IsKind(SyntaxKind.EqualsExpression) ||
                        expression.IsKind(SyntaxKind.NotEqualsExpression))
                    .Where(expression => IsNull(expression.Left) || IsNull(expression.Right))
                    .ToArray();

                Assert.That(
                    equalityChecks,
                    Is.Empty,
                    $"{source}: use 'is null' / 'is not null' instead of '== null' / '!= null'.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoNullCoalescingOperatorIsUsed()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                BinaryExpressionSyntax[] coalescing = root
                    .DescendantNodes()
                    .OfType<BinaryExpressionSyntax>()
                    .Where(expression => expression.IsKind(SyntaxKind.CoalesceExpression))
                    .ToArray();

                Assert.That(coalescing, Is.Empty, $"{source}: the null-coalescing operator '??' is not allowed.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoUnsafeCastsAreUsed()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                CastExpressionSyntax[] unsafeCasts = root
                    .DescendantNodes()
                    .OfType<CastExpressionSyntax>()
                    .Where(cast => cast.Type is IdentifierNameSyntax identifier &&
                        char.IsUpper(identifier.Identifier.ValueText[0]))
                    .ToArray();

                Assert.That(unsafeCasts, Is.Empty, $"{source}: prefer 'as' for casts that may fail instead of a direct cast.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoNullForgivingOperatorIsUsed()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                PostfixUnaryExpressionSyntax[] nullForgiving = root
                    .DescendantNodes()
                    .OfType<PostfixUnaryExpressionSyntax>()
                    .Where(expression => expression.IsKind(SyntaxKind.SuppressNullableWarningExpression))
                    .ToArray();

                Assert.That(nullForgiving, Is.Empty, $"{source}: the null-forgiving operator '!' is not allowed.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenInstanceEqualsIsNotUsedInLiteralComparisons()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                InvocationExpressionSyntax[] calls = root
                    .DescendantNodes()
                    .OfType<InvocationExpressionSyntax>()
                    .Where(invocation => invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
                        memberAccess.Name.Identifier.ValueText == "Equals" &&
                        invocation.ArgumentList.Arguments.Count == 1 &&
                        IsNull(memberAccess.Expression))
                    .ToArray();

                Assert.That(calls, Is.Empty, $"{source}: prefer static Equals(a, b) over instance '.Equals()'.");
            }
        }

        private static bool IsNull(ExpressionSyntax expression)
            => expression.IsKind(SyntaxKind.NullLiteralExpression);

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