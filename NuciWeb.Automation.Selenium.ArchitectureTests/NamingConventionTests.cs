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
    public sealed class NamingConventionTests
    {
        private static string[] BclTypeNames =>
        [
            "String", "Int32", "Boolean", "Object", "Byte", "Char", "Int64",
            "Double", "Single", "Decimal", "UInt32", "UInt64", "Int16", "UInt16"
        ];

        private static string[] ForbiddenBooleanNames =>
        [
            "flag", "Flag", "check", "Check", "result", "Result"
        ];

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoBclTypeNamesAreUsed()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                string[] bclTypeUsages = root
                    .DescendantNodes()
                    .OfType<TypeSyntax>()
                    .SelectMany(type => type.DescendantNodesAndSelf())
                    .OfType<SimpleNameSyntax>()
                    .Where(name => name.Parent is not MemberAccessExpressionSyntax)
                    .Select(name => name.Identifier.ValueText)
                    .Where(BclTypeNames.Contains)
                    .Distinct(StringComparer.Ordinal)
                    .ToArray();

                Assert.That(
                    bclTypeUsages,
                    Is.Empty,
                    $"{source}: built-in aliases must be used instead of {string.Join(", ", bclTypeUsages)}.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenTypesUsePascalCase()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                string[] invalidNames = root
                    .DescendantNodes()
                    .OfType<BaseTypeDeclarationSyntax>()
                    .Select(declaration => declaration.Identifier.ValueText)
                    .Where(name => !IsPascalCase(name))
                    .ToArray();

                Assert.That(invalidNames, Is.Empty, $"{source}: type names must use PascalCase.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenMethodsUsePascalCase()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                string[] invalidNames = root
                    .DescendantNodes()
                    .OfType<MethodDeclarationSyntax>()
                    .Select(method => method.Identifier.ValueText)
                    .Where(name => !IsPascalCase(name))
                    .ToArray();

                Assert.That(invalidNames, Is.Empty, $"{source}: method names must use PascalCase.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenPublicMembersUseUppercaseInitials()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                IEnumerable<string> invalidNames = root
                    .DescendantNodes()
                    .OfType<MemberDeclarationSyntax>()
                    .Where(IsPublic)
                    .SelectMany(GetDeclaredMemberNames)
                    .Where(name => !IsPascalCase(name));

                Assert.That(invalidNames, Is.Empty, $"{source}: public and protected members must begin uppercase.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenPrivateFieldsUseCamelCase()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                VariableDeclaratorSyntax[] underscoreFields = root
                    .DescendantNodes()
                    .OfType<FieldDeclarationSyntax>()
                    .Where(field => field.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PrivateKeyword)))
                    .SelectMany(field => field.Declaration.Variables)
                    .Where(variable => variable.Identifier.ValueText.StartsWith('_'))
                    .ToArray();

                Assert.That(
                    underscoreFields,
                    Is.Empty,
                    $"{source}: private fields must use camelCase without an underscore prefix.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenInterfacesUseTheIPrefix()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                string[] invalidNames = root
                    .DescendantNodes()
                    .OfType<InterfaceDeclarationSyntax>()
                    .Select(declaration => declaration.Identifier.ValueText)
                    .Where(name => !name.StartsWith('I') || name.Length < 2 || !char.IsUpper(name[1]))
                    .ToArray();

                Assert.That(
                    invalidNames,
                    Is.Empty,
                    $"{source}: interface names must use the 'I' prefix followed by PascalCase.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenBooleanNamesUsePermittedForms()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                string[] forbiddenNames = root
                    .DescendantNodes()
                    .OfType<PropertyDeclarationSyntax>()
                    .Where(property => property.Type is PredefinedTypeSyntax predefined &&
                        predefined.Keyword.IsKind(SyntaxKind.BoolKeyword))
                    .Select(property => property.Identifier.ValueText)
                    .Where(ForbiddenBooleanNames.Contains)
                    .ToArray();

                Assert.That(
                    forbiddenNames,
                    Is.Empty,
                    $"{source}: boolean names must use Is/Has/Does/Are forms instead of flag/check/result.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenNoEnumAliasSharesNumericValues()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                EnumDeclarationSyntax[] duplicateValueEnums = root
                    .DescendantNodes()
                    .OfType<EnumDeclarationSyntax>()
                    .Where(declaration => declaration.Members
                        .Select(member => member.EqualsValue?.Value.ToString() ?? member.Identifier.ValueText)
                        .GroupBy(value => value, StringComparer.Ordinal)
                        .Any(group => group.Count() > 1))
                    .ToArray();

                Assert.That(
                    duplicateValueEnums,
                    Is.Empty,
                    $"{source}: enum numeric values must stay unique unless a documented alias is required.");
            }
        }

        private static bool IsPascalCase(string name)
            => !string.IsNullOrEmpty(name) && char.IsUpper(name[0]);

        private static bool IsPublic(MemberDeclarationSyntax member)
            => GetModifiers(member).Any(modifier =>
                modifier.IsKind(SyntaxKind.PublicKeyword) ||
                modifier.IsKind(SyntaxKind.ProtectedKeyword));

        private static SyntaxTokenList GetModifiers(MemberDeclarationSyntax member)
            => member switch
            {
                BaseTypeDeclarationSyntax typeDeclaration => typeDeclaration.Modifiers,
                BaseMethodDeclarationSyntax methodDeclaration => methodDeclaration.Modifiers,
                PropertyDeclarationSyntax propertyDeclaration => propertyDeclaration.Modifiers,
                FieldDeclarationSyntax fieldDeclaration => fieldDeclaration.Modifiers,
                EventFieldDeclarationSyntax eventDeclaration => eventDeclaration.Modifiers,
                _ => default
            };

        private static IEnumerable<string> GetDeclaredMemberNames(MemberDeclarationSyntax member)
        {
            if (member is PropertyDeclarationSyntax property) { return [property.Identifier.ValueText]; }
            if (member is MethodDeclarationSyntax method) { return [method.Identifier.ValueText]; }
            if (member is BaseTypeDeclarationSyntax typeDeclaration) { return [typeDeclaration.Identifier.ValueText]; }
            if (member is FieldDeclarationSyntax field)
            {
                return field.Declaration.Variables.Select(variable => variable.Identifier.ValueText);
            }

            return [];
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