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
    public sealed class FormattingConventionTests
    {
        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoConsecutiveBlankLinesExist()
        {
            foreach (string source in GetProductionSources())
            {
                Assert.That(
                    ReadSource(source,
                        ArchitectureResources.ProductionSourcePrefix),
                    Does.Not.Match(@"\n[ \t]*\n[ \t]*\n"),
                    $"{source}: two or more consecutive blank lines are not allowed.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenFileEndsWithOneLineFeed()
        {
            foreach (string source in GetProductionSources())
            {
                string content = ReadSource(source, ArchitectureResources.ProductionSourcePrefix);

                Assert.That(content.EndsWith('\n'), Is.True, $"{source}: the file must end with a line feed.");
                Assert.That(
                    content.EndsWith("\n\n", StringComparison.Ordinal),
                    Is.False,
                    $"{source}: the file must end with exactly one line feed.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoCarriageReturnsExist()
        {
            foreach (string source in GetProductionSources())
            {
                Assert.That(
                    ReadSource(source, ArchitectureResources.ProductionSourcePrefix),
                    Does.Not.Contain("\r"),
                    $"{source}: line endings must use a single LF.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoTrailingWhitespaceExists()
        {
            foreach (string source in GetProductionSources())
            {
                Assert.That(
                    ReadSource(source, ArchitectureResources.ProductionSourcePrefix),
                    Does.Not.Match(@"[ \t]+(\r?\n)"),
                    $"{source}: trailing whitespace is not allowed.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoTabsAreUsed()
        {
            foreach (string source in GetProductionSources())
            {
                Assert.That(
                    ReadSource(source, ArchitectureResources.ProductionSourcePrefix),
                    Does.Not.Match(@"(?m)^[ \t]*\t"),
                    $"{source}: tabs must not be used for indentation.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoBlankLinePrecedesClosingDelimiters()
        {
            foreach (string source in GetProductionSources())
            {
                Assert.That(
                    ReadSource(source, ArchitectureResources.ProductionSourcePrefix),
                    Does.Not.Match(@"\n[ \t]*\n[ \t]*[}\])]"),
                    $"{source}: no blank line may precede a closing delimiter.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoByteOrderMarkIsPresent()
        {
            foreach (string source in GetProductionSources())
            {
                Assert.That(
                    ReadSource(source, ArchitectureResources.ProductionSourcePrefix).StartsWith('\uFEFF'),
                    Is.False,
                    $"{source}: the file must not start with a byte-order mark.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenIndentationUsesFourSpaceMultiples()
        {
            foreach (string source in GetProductionSources())
            {
                string[] lines = ReadSource(source, ArchitectureResources.ProductionSourcePrefix).Split('\n');

                for (int index = 0; index < lines.Length; index++)
                {
                    string line = lines[index];

                    if (line.Trim().Length == 0)
                    {
                        continue;
                    }

                    int leadingSpaces = line.Length - line.TrimStart(' ').Length;

                    Assert.That(
                        leadingSpaces % 4,
                        Is.EqualTo(0),
                        $"{source}:{index + 1}: indentation must use multiples of four spaces.");
                }
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenMembersAreSeparatedByOneBlankLine()
        {
            foreach (string source in GetProductionSources())
            {
                string[] lines = ReadSource(source, ArchitectureResources.ProductionSourcePrefix).Split('\n');

                for (int index = 1; index < lines.Length; index++)
                {
                    if (!IsClosingBrace(lines[index - 1].Trim()) || !IsMemberStart(lines[index].Trim()))
                    {
                        continue;
                    }

                    Assert.That(
                        lines[index - 1].Length == 0 || lines[index - 1].Trim().Length == 0,
                        Is.True,
                        $"{source}:{index + 1}: members must be separated by a blank line.");
                }
            }
        }

        private static bool IsClosingBrace(string trimmed)
            => trimmed.Length > 0 && trimmed[^1] == '}';

        private static bool IsMemberStart(string trimmed)
            => trimmed.StartsWith("public ", StringComparison.Ordinal) ||
                trimmed.StartsWith("private ", StringComparison.Ordinal) ||
                trimmed.StartsWith("protected ", StringComparison.Ordinal) ||
                trimmed.StartsWith("internal ", StringComparison.Ordinal) ||
                trimmed.StartsWith("/// ", StringComparison.Ordinal);

        private static string ReadSource(string source, string prefix)
            => ArchitectureResources.ReadResource(prefix, source);

        private static IReadOnlyList<string> GetProductionSources()
        {
            IReadOnlyList<string> sources = ArchitectureResources.GetResourcePaths(
                ArchitectureResources.ProductionSourcePrefix);

            Assert.That(sources, Is.Not.Empty, "No production source files were found.");

            return sources;
        }
    }
}