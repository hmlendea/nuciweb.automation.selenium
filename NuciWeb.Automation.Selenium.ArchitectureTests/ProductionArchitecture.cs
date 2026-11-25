using ArchUnitNET.Domain;
using ArchUnitNET.Loader;

using static ArchUnitNET.Fluent.ArchRuleDefinition;

using ReflectionAssembly = System.Reflection.Assembly;
using ArchUnitNET.Loader;

namespace NuciWeb.Automation.Selenium.ArchitectureTests
{
    internal static class ProductionArchitecture
    {
        internal static ReflectionAssembly Assembly => typeof(IWebProcessor).Assembly;

        internal static Architecture Model { get; } = new ArchLoader()
            .LoadAssemblies(Assembly)
            .Build();

        internal static IObjectProvider<IType> ProductionTypes { get; } = Types(
            ).That()
            .ResideInAssembly(Assembly)
            .As("NuciWeb.Automation.Selenium production types");
    }
}