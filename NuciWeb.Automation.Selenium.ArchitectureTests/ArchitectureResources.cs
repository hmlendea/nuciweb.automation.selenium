using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NuciWeb.Automation.Selenium.ArchitectureTests
{
    internal static class ArchitectureResources
    {
        internal const string ProductionSourcePrefix = "ProductionSources/";
        internal const string UnitTestSourcePrefix = "UnitTestSources/";
        internal const string ArchitectureTestSourcePrefix = "ArchitectureTestSources/";
        internal const string ProjectFilePrefix = "ProjectFiles/";

        internal static string ProductionSourceRoot => Path.GetFullPath(ArchitecturePaths.ProductionSourceRoot);

        internal static string UnitTestSourceRoot => Path.GetFullPath(Path.Combine(
            ProductionSourceRoot,
            "..",
            "NuciWeb.Automation.Selenium.UnitTests"));

        internal static string ArchitectureTestSourceRoot => Path.GetFullPath(Path.Combine(
            ProductionSourceRoot,
            "..",
            "NuciWeb.Automation.Selenium.ArchitectureTests"));

        internal static IReadOnlyList<string> GetResourcePaths(string prefix)
        {
            string root = prefix switch
            {
                ProductionSourcePrefix => ProductionSourceRoot,
                UnitTestSourcePrefix => UnitTestSourceRoot,
                ArchitectureTestSourcePrefix => ArchitectureTestSourceRoot,
                _ => throw new InvalidOperationException(
                    $"Unknown resource prefix '{prefix}'.")
            };

            return Directory
                .GetFiles(root, "*.cs", SearchOption.AllDirectories)
                .Where(file => !file.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar)
                    && !file.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar))
                .Select(file => Normalise(Path.GetRelativePath(root, file)))
                .OrderBy(path => path, StringComparer.Ordinal)
                .ToArray();
        }

        internal static string ReadResource(string prefix, string relativePath)
        {
            string root = prefix switch
            {
                ProductionSourcePrefix => ProductionSourceRoot,
                UnitTestSourcePrefix => UnitTestSourceRoot,
                ArchitectureTestSourcePrefix => ArchitectureTestSourceRoot,
                _ => throw new InvalidOperationException(
                    $"Unknown resource prefix '{prefix}'.")
            };

            return File.ReadAllText(Path.Combine(root, relativePath));
        }

        private static string Normalise(string resourceName)
            => resourceName.Replace('\\', '/');
    }
}
