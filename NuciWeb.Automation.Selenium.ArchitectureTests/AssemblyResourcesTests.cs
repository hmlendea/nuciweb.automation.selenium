using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using NUnit.Framework;

namespace NuciWeb.Automation.Selenium.ArchitectureTests
{
    [TestFixture]
    public sealed class AssemblyResourcesTests
    {
        [Test]
        public void GivenProductionSources_WhenEnumerated_ThenTheyAreDiscovered()
        {
            IReadOnlyList<string> sources = ArchitectureResources.GetResourcePaths(
                ArchitectureResources.ProductionSourcePrefix);

            Assert.That(sources, Is.Not.Empty, "No production source files were found.");
        }

        [Test]
        public void GivenUnitTestSources_WhenEnumerated_ThenTheyAreDiscovered()
        {
            IReadOnlyList<string> sources = ArchitectureResources.GetResourcePaths(
                ArchitectureResources.UnitTestSourcePrefix);

            Assert.That(sources, Is.Not.Empty, "No unit test source files were found.");
        }

        [Test]
        public void GivenArchitectureTestSources_WhenEnumerated_ThenTheyAreDiscovered()
        {
            IReadOnlyList<string> sources = ArchitectureResources.GetResourcePaths(
                ArchitectureResources.ArchitectureTestSourcePrefix);

            Assert.That(sources, Is.Not.Empty, "No architecture test source files were found.");
        }
    }
}