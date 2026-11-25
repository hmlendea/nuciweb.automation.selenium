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
    public sealed class StatementConventionTests
    {
        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoStatementsFollowTerminalStatements()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                foreach (BlockSyntax block in root.DescendantNodes().OfType<BlockSyntax>())
                {
                    SyntaxList<StatementSyntax> statements = block.Statements;

                    for (int index = 0; index < statements.Count - 1; index++)
                    {
                        Assert.That(IsTerminal(statements[index]),
                            Is.False,
                            $"{source}: unreachable statements must not follow a terminal statement.");
                    }
                }
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenLoopsAndConditionsAreNotEmpty()
        {
            AssertNoEmptyBodies(GetProductionSources());
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenControlFlowBodiesUseBraces()
            => assertBraces(GetProductionSources());

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoRethrowsWithExpression()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                ThrowStatementSyntax[] rethrows = root
                    .DescendantNodes()
                    .OfType<CatchClauseSyntax>()
                    .SelectMany(catchClause => catchClause.Block.DescendantNodes().OfType<ThrowStatementSyntax>())
                    .Where(throwString => throwString.Expression is not null)
                    .ToArray();

                Assert.That(rethrows, Is.Empty, $"{source}: a rethrow must use a bare 'throw;'.");
            }
        }

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

                Assert.That(equalityChecks, Is.Empty, $"{source}: use 'is null' / 'is not null' instead of '== null' / '!= null'.");
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

        private static bool IsTerminal(StatementSyntax statement)
            => statement is ReturnStatementSyntax ||
                statement is ThrowStatementSyntax ||
                statement is BreakStatementSyntax ||
                statement is ContinueStatementSyntax;

        private static bool IsNull(ExpressionSyntax expression)
            => expression.IsKind(SyntaxKind.NullLiteralExpression);

        private static void assertBraces(IReadOnlyList<string> sources)
        {
            foreach (string source in sources)
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                SyntaxNode[] unbraced = root.DescendantNodes()
                    .OfType<IfStatementSyntax>()
                    .Where(statement => statement.Statement is not BlockSyntax)
                    .Select(statement => (SyntaxNode)statement)
                    .Concat(root.DescendantNodes().OfType<ElseClauseSyntax>()
                        .Where(clause => clause.Statement is not BlockSyntax && clause.Statement is not IfStatementSyntax)
                        .Select(clause => (SyntaxNode)clause))
                    .Concat(root.DescendantNodes().OfType<ForStatementSyntax>()
                        .Where(statement => statement.Statement is not BlockSyntax)
                        .Select(statement => (SyntaxNode)statement))
                    .Concat(root.DescendantNodes().OfType<ForEachStatementSyntax>()
                        .Where(statement => statement.Statement is not BlockSyntax)
                        .Select(statement => (SyntaxNode)statement))
                    .Concat(root.DescendantNodes().OfType<WhileStatementSyntax>()
                        .Where(statement => statement.Statement is not BlockSyntax)
                        .Select(statement => (SyntaxNode)statement))
                    .Concat(root.DescendantNodes().OfType<DoStatementSyntax>()
                        .Where(statement => statement.Statement is not BlockSyntax)
                        .Select(statement => (SyntaxNode)statement))
                    .ToArray();

                Assert.That(
                    unbraced,
                    Is.Empty,
                    $"{source}: every control-flow body must use braces.");
            }
        }

        private static void AssertNoEmptyBodies(IReadOnlyList<string> sources)
        {
            foreach (string source in sources)
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                SyntaxNode[] emptyBodies = root.DescendantNodes()
                    .OfType<BlockSyntax>()
                    .Where(block => block.Statements.Count == 0)
                    .Where(block => block.Parent is IfStatementSyntax ||
                        block.Parent is ElseClauseSyntax ||
                        block.Parent is ForStatementSyntax ||
                        block.Parent is ForEachStatementSyntax ||
                        block.Parent is WhileStatementSyntax ||
                        block.Parent is DoStatementSyntax)
                    .ToArray();

                Assert.That(
                    emptyBodies,
                    Is.Empty,
                    $"{source}: empty control-flow bodies are not allowed.");
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