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
    public sealed class AsyncConventionTests
    {
        [Test]
        public void GivenProductionSources_WhenChecked_ThenAsyncIsOnlyUsedWhereRequired()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                MethodDeclarationSyntax[] asyncMethods = root
                    .DescendantNodes()
                    .OfType<MethodDeclarationSyntax>()
                    .Where(method => method.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.AsyncKeyword)))
                    .ToArray();

                Assert.That(
                    asyncMethods,
                    Is.Empty,
                    $"{source}: synchronous methods are preferred; async/await requires explicit justification.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoCancellationTokenIsDeclaredLast()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                ParameterSyntax[] cancellationTokens = root
                    .DescendantNodes()
                    .OfType<ParameterSyntax>()
                    .Where(parameter => parameter.Type?.ToString() == "CancellationToken")
                    .Where(parameter => parameter.Parent is ParameterListSyntax list &&
                        list.Parameters.Count > 0 &&
                        list.Parameters[^1] != parameter)
                    .ToArray();

                Assert.That(
                    cancellationTokens,
                    Is.Empty,
                    $"{source}: CancellationToken must be declared last in the parameter list.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoManualTaskSchedulingIsUsed()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                InvocationExpressionSyntax[] manualScheduling = root
                    .DescendantNodes()
                    .OfType<InvocationExpressionSyntax>()
                    .Where(invocation => invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
                        memberAccess.Name.Identifier.ValueText is "Run" or "Delay" or "Wait")
                    .Select(invocation => invocation)
                    .ToArray();

                Assert.That(
                    manualScheduling,
                    Is.Empty,
                    $"{source}: manual task scheduling is not needed in this synchronous library.");
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