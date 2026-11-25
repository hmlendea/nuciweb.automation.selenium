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
    public sealed class CodingConventionTests
    {
        [Test]
        public void GivenProductionSources_WhenChecked_ThenExplicitTypesAreUsed()
        {
            AssertNoSyntax(GetProductionSources(),
                root => root.DescendantNodes().OfType<VariableDeclarationSyntax>()
                    .Where(declaration => declaration.Type.IsVar),
                "the 'var' keyword must not be used");
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoTernariesAreUsed()
        {
            AssertNoSyntax(GetProductionSources(),
                root => root.DescendantNodes().OfType<ConditionalExpressionSyntax>(),
                "ternary expressions are not allowed");
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoNullCoalescingIsUsed()
        {
            AssertNoSyntax(GetProductionSources(),
                root => root.DescendantNodes().OfType<BinaryExpressionSyntax>()
                    .Where(expression => expression.IsKind(SyntaxKind.CoalesceExpression)),
                "the null-coalescing operator '??' is not allowed");
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoTuplesAreUsed()
        {
            AssertNoSyntax(GetProductionSources(),
                root => root.DescendantNodes()
                    .Where(node => node.IsKind(SyntaxKind.TupleType) || node.IsKind(SyntaxKind.TupleExpression)),
                "tuples are not allowed");
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoRegionsAreUsed()
        {
            foreach (string source in GetProductionSources())
            {
                Assert.That(
                    ReadSource(source),
                    Does.Not.Match(@"(?m)^\s*#(end)?region\b"),
                    $"{source}: regions are not allowed.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoRecordTypesAreUsed()
        {
            AssertNoSyntax(GetProductionSources(),
                root => root.DescendantNodes().OfType<RecordDeclarationSyntax>(),
                "record types are preview features and are not allowed");
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoInitOnlyAccessorsAreUsed()
        {
            AssertNoSyntax(GetProductionSources(),
                root => root.DescendantNodes().OfType<AccessorDeclarationSyntax>()
                    .Where(accessor => accessor.IsKind(SyntaxKind.InitAccessorDeclaration)),
                "init-only accessors are not allowed");
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoUsingStaticOrGlobalUsingsAreUsed()
        {
            AssertNoSyntax(GetProductionSources(),
                root => root.DescendantNodes().OfType<UsingDirectiveSyntax>()
                    .Where(usingDirective =>
                        usingDirective.StaticKeyword.IsKind(SyntaxKind.StaticKeyword) ||
                        usingDirective.GlobalKeyword.IsKind(SyntaxKind.GlobalKeyword)),
                "'using static' and 'global using' directives are not allowed");
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoNullableDirectivesAreUsed()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                SyntaxTrivia[] nullableDirectives = root
                    .DescendantTrivia()
                    .Where(trivia => trivia.IsKind(SyntaxKind.NullableDirectiveTrivia))
                    .ToArray();

                Assert.That(nullableDirectives, Is.Empty, $"{source}: the '#nullable' directive is not allowed.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoPreviewLanguageFeaturesAreUsed()
        {
            AssertNoSyntax(GetProductionSources(),
                root => root.DescendantNodes().OfType<RecordDeclarationSyntax>(),
                "record types are preview features");
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenInlineFullyQualifiedTypesAreNotUsed()
        {
            foreach (string source in GetProductionSources())
            {
                Assert.That(
                    ReadSource(source),
                    Does.Not.Match(
                        @"\bSystem\.(String|Int32|Int64|Boolean|Object|Byte|Char|Double|Single|Decimal|DateTime|Exception|Guid|Convert|Console|Environment|Math)\b"),
                    $"{source}: inline fully qualified types are not allowed; add a using directive instead.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoConstDeclarationsAreUsed()
        {
            AssertNoSyntax(GetProductionSources(),
                root => root.DescendantNodes().OfType<FieldDeclarationSyntax>()
                    .Where(field => field.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.ConstKeyword))),
                "'const' is not allowed; use a static property instead.");
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenCompoundAssignmentsAreUsed()
        {
            AssertNoSyntax(GetProductionSources(),
                root => root.DescendantNodes().OfType<AssignmentExpressionSyntax>()
                    .Where(assignment => assignment.IsKind(SyntaxKind.SimpleAssignmentExpression))
                    .Where(IsSelfIncrementOrDecrement),
                "use '+=' or '-=' instead of self-assignments.");
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoNestedConditionalExpressionsExist()
        {
            AssertNoSyntax(GetProductionSources(),
                root => root.DescendantNodes().OfType<ConditionalExpressionSyntax>()
                    .Where(expression => expression.DescendantNodes().OfType<ConditionalExpressionSyntax>().Any()),
                "conditional expressions must not be nested.");
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoEmptyCatchBlocksExist()
        {
            AssertNoSyntax(GetProductionSources(),
                root => root.DescendantNodes().OfType<CatchClauseSyntax>()
                    .Where(catchClause => catchClause.Block.Statements.Count == 0),
                "empty catch blocks are not allowed.");
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenRethrowIsBare()
        {
            AssertNoSyntax(GetProductionSources(),
                root => root.DescendantNodes().OfType<CatchClauseSyntax>()
                    .SelectMany(catchClause => catchClause.Block.DescendantNodes().OfType<ThrowStatementSyntax>())
                    .Where(throwStatement => throwStatement.Expression is not null),
                "a rethrow must use a bare 'throw;'.");
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenOnlyBclExceptionsAreThrown()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                string[] invalidThrows = root
                    .DescendantNodes()
                    .OfType<ThrowStatementSyntax>()
                    .Select(throwStatement => throwStatement.Expression?.ToString().TrimEnd(';') ?? string.Empty)
                    .Where(expression => !IsBclException(expression))
                    .ToArray();

                Assert.That(
                    invalidThrows,
                    Is.Empty,
                    $"{source}: only base class library exceptions may be thrown; found {string.Join(", ", invalidThrows)}.");
            }
        }

        private static bool IsBclException(string expression)
        {
            string[] permitted =
            [
                "Exception",
                "ArgumentNullException",
                "ArgumentException",
                "InvalidOperationException",
                "NotSupportedException",
                "TimeoutException",
                "NotFoundException"
            ];

            return permitted.Any(candidate => expression.StartsWith("new " + candidate, StringComparison.Ordinal));
        }

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

        private static void AssertNoSyntax(
            IReadOnlyList<string> sources,
            Func<SyntaxNode, IEnumerable<SyntaxNode>> selector,
            string reason)
        {
            foreach (string source in sources)
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                SyntaxNode[] matches = selector(root).ToArray();

                Assert.That(matches, Is.Empty, $"{source}: {reason}.");
            }
        }

        private static string ReadSource(string source)
            => ArchitectureResources.ReadResource(ArchitectureResources.ProductionSourcePrefix, source);

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