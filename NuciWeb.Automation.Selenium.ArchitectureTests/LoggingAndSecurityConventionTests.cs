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
    public sealed class LoggingAndSecurityConventionTests
    {
        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoSensitiveLoggingCallsExist()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                InvocationExpressionSyntax[] sensitiveCalls = root
                    .DescendantNodes()
                    .OfType<InvocationExpressionSyntax>()
                    .Where(invocation => invocation.ToString().Contains("password", StringComparison.OrdinalIgnoreCase) ||
                        invocation.ToString().Contains("secret", StringComparison.OrdinalIgnoreCase) ||
                        invocation.ToString().Contains("token", StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                Assert.That(
                    sensitiveCalls,
                    Is.Empty,
                    $"{source}: sensitive values must not flow into logging or telemetry calls.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoBareExceptionMessagesAreLogged()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                InvocationExpressionSyntax[] messageConcatenations = root
                    .DescendantNodes()
                    .OfType<InvocationExpressionSyntax>()
                    .Where(invocation => invocation.ArgumentList.Arguments
                        .Any(argument => argument.Expression is BinaryExpressionSyntax binary &&
                            binary.IsKind(SyntaxKind.AddExpression) &&
                            (binary.Left.ToString().Contains(".Message") ||
                             binary.Right.ToString().Contains(".Message"))))
                    .Select(invocation => invocation)
                    .ToArray();

                Assert.That(
                    messageConcatenations,
                    Is.Empty,
                    $"{source}: exception text must not be embedded in log messages; pass exceptions via the dedicated parameter.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoSensitiveDataIsInterpolatedIntoMessages()
        {
            string[] sensitiveFragments = ["password=", "apiKey=", "api_key=", "secret="];

            foreach (string source in GetProductionSources())
            {
                string content = ReadSource(source);

                foreach (string fragment in sensitiveFragments)
                {
                    Assert.That(
                        content.Contains(fragment, StringComparison.OrdinalIgnoreCase),
                        Is.False,
                        $"{source}: sensitive values must not be embedded in messages.");
                }
            }
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