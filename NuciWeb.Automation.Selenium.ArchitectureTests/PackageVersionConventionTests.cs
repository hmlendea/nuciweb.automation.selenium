using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using NUnit.Framework;

namespace NuciWeb.Automation.Selenium.ArchitectureTests
{
    [TestFixture]
    public sealed class PackageVersionConventionTests
    {
        private const string RequiredSeleniumVersion = "4.5.1";

        [Test]
        public void GivenTheProductionProject_WhenChecked_ThenTheSeleniumPackagesArePinnedToTheExactVersion()
        {
            IReadOnlyDictionary<string, string> seleniumVersions = GetSeleniumPackageVersions();

            Assert.That(
                seleniumVersions,
                Is.Not.Empty,
                "No Selenium package references were found in the production project.");

            foreach (KeyValuePair<string, string> package in seleniumVersions)
            {
                Assert.That(
                    package.Value,
                    Is.EqualTo(RequiredSeleniumVersion),
                    $"The '{package.Key}' package must be pinned to exactly version {RequiredSeleniumVersion}, but was '{package.Value}'.");
            }
        }

        [Test]
        public void GivenTheProductionProject_WhenChecked_ThenBothRequiredSeleniumPackagesArePresent()
        {
            IReadOnlyDictionary<string, string> seleniumVersions = GetSeleniumPackageVersions();

            Assert.Multiple(() =>
            {
                Assert.That(
                    seleniumVersions.ContainsKey("Selenium.WebDriver"),
                    Is.True,
                    "Selenium.WebDriver must be referenced by the production project.");

                Assert.That(
                    seleniumVersions.ContainsKey("Selenium.Support"),
                    Is.True,
                    "Selenium.Support must be referenced by the production project.");
            });
        }

        private static IReadOnlyDictionary<string, string> GetSeleniumPackageVersions()
        {
            string projectPath = Path.Combine(
                ArchitectureResources.ProductionSourceRoot,
                "NuciWeb.Automation.Selenium.csproj");

            Assert.That(
                File.Exists(projectPath),
                Is.True,
                $"The production project file was not found at '{projectPath}'.");

            return XDocument
                .Load(projectPath)
                .Descendants("PackageReference")
                .Where(reference => GetInclude(reference).StartsWith("Selenium.", StringComparison.Ordinal))
                .ToDictionary(
                    reference => GetInclude(reference),
                    reference => reference.Attribute("Version")?.Value ?? string.Empty,
                    StringComparer.Ordinal);
        }

        private static string GetInclude(XElement reference)
            => reference.Attribute("Include")?.Value ?? string.Empty;
    }
}
