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
    public sealed class LowLevelConventionTests
    {
        [Test]
        public void GivenProductionSources_WhenChecked_ThenCompoundIncrementIsUsed()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                AssignmentExpressionSyntax[] selfAssignments = root
                    .DescendantNodes()
                    .OfType<AssignmentExpressionSyntax>()
                    .Where(assignment => assignment.IsKind(SyntaxKind.SimpleAssignmentExpression))
                    .Where(IsSelfIncrementOrDecrement)
                    .ToArray();

                Assert.That(
                    selfAssignments,
                    Is.Empty,
                    $"{source}: use '+=' or '-=' instead of self-assignments.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoUnsafeOrFixedCodeIsUsed()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                Assert.That(
                    root.DescendantNodes().Where(node => node.IsKind(SyntaxKind.UnsafeStatement)),
                    Is.Empty,
                    $"{source}: unsafe code is not allowed.");

                Assert.That(
                    root.DescendantNodes().Where(node => node.IsKind(SyntaxKind.FixedStatement)),
                    Is.Empty,
                    $"{source}: fixed statements are not allowed.");

                Assert.That(
                    root.DescendantNodes().OfType<PointerTypeSyntax>(),
                    Is.Empty,
                    $"{source}: pointer types are not allowed.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoGotoOrLabeledStatementsAreUsed()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                Assert.That(
                    root.DescendantNodes().OfType<GotoStatementSyntax>(),
                    Is.Empty,
                    $"{source}: goto statements are not allowed.");

                Assert.That(
                    root.DescendantNodes().OfType<LabeledStatementSyntax>(),
                    Is.Empty,
                    $"{source}: labeled statements are not allowed.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoOperatorOverloadsAreDeclaredOutsideEquality()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                OperatorDeclarationSyntax[] operators = root
                    .DescendantNodes()
                    .OfType<OperatorDeclarationSyntax>()
                    .Where(operatorDeclaration => !IsEqualityOperator(operatorDeclaration))
                    .ToArray();

                Assert.That(operators, Is.Empty, $"{source}: only equality and inequality operators are permitted.");
            }
        }

        private static bool IsEqualityOperator(OperatorDeclarationSyntax operatorDeclaration)
            => operatorDeclaration.OperatorToken.IsKind(SyntaxKind.EqualsEqualsToken) ||
                operatorDeclaration.OperatorToken.IsKind(SyntaxKind.ExclamationEqualsToken);

        private static bool IsSelfIncrementOrDecrement(AssignmentExpressionSyntax assignment)
        {
            if (assignment.Left is not IdentifierNameSyntax left)
            {
                return false;
            }

            if (assignment.Right is not BinaryExpressionSyntax binary)
            {
                return false;
            }

            bool isIncrement = binary.IsKind(SyntaxKind.AddExpression);
            bool isDecrement = binary.IsKind(SyntaxKind.SubtractExpression);

            if (!isIncrement && !isDecrement)
            {
                return false;
            }

            bool targetOnLeft = binary.Left is IdentifierNameSyntax binaryLeft &&
                binaryLeft.Identifier.ValueText == left.Identifier.ValueText;

            bool targetOnRight = binary.Right is IdentifierNameSyntax binaryRight &&
                binaryRight.Identifier.ValueText == left.Identifier.ValueText;

            return targetOnLeft || targetOnRight;
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