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
    public sealed class MemberOrganisationTests
    {
        [Test]
        public void GivenProductionSources_WhenChecked_ThenMembersDeclareAccessibility()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                MemberDeclarationSyntax[] undocumentedMembers = root
                    .DescendantNodes()
                    .OfType<MemberDeclarationSyntax>()
                    .Where(IsFieldOrPropertyOrEventOrConstructorOrMethod)
                    .Where(member => !DeclaresAccessibility(member))
                    .ToArray();

                Assert.That(
                    undocumentedMembers,
                    Is.Empty,
                    $"{source}: every field, property, event, constructor, and method must declare accessibility.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenMembersOrderedByKind()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                foreach (TypeDeclarationSyntax typeDeclaration in root.DescendantNodes().OfType<TypeDeclarationSyntax>())
                {
                    int[] kinds = typeDeclaration.Members
                        .Select(GetMemberKindOrder)
                        .Where(order => order >= 0)
                        .ToArray();

                    Assert.That(
                        kinds,
                        Is.Ordered,
                        $"{source}: members of '{typeDeclaration.Identifier.ValueText}' must be ordered fields -> properties -> events -> constructors -> methods.");
                }
            }
        }

        [Test]
        public void GivenProductionSources_WhenChecked_ThenFieldsOrderedByKind()
        {
            foreach (string source in GetProductionSources())
            {
                SyntaxNode root = ParseTree(source, ArchitectureResources.ProductionSourcePrefix);

                foreach (TypeDeclarationSyntax typeDeclaration in root.DescendantNodes().OfType<TypeDeclarationSyntax>())
                {
                    int[] orders = typeDeclaration.Members
                        .OfType<FieldDeclarationSyntax>()
                        .Select(GetFieldKindOrder)
                        .ToArray();

                    Assert.That(
                        orders,
                        Is.Ordered,
                        $"{source}: fields of '{typeDeclaration.Identifier.ValueText}' must be ordered static readonly -> static mutable -> instance readonly -> instance mutable.");
                }
            }
        }

        private static bool IsFieldOrPropertyOrEventOrConstructorOrMethod(MemberDeclarationSyntax member)
            => member is FieldDeclarationSyntax ||
                member is PropertyDeclarationSyntax ||
                member is EventFieldDeclarationSyntax ||
                member is ConstructorDeclarationSyntax ||
                member is MethodDeclarationSyntax;

        private static bool DeclaresAccessibility(MemberDeclarationSyntax member)
            => GetModifiers(member).Any(modifier =>
                modifier.IsKind(SyntaxKind.PublicKeyword) ||
                modifier.IsKind(SyntaxKind.ProtectedKeyword) ||
                modifier.IsKind(SyntaxKind.InternalKeyword) ||
                modifier.IsKind(SyntaxKind.PrivateKeyword));

        private static SyntaxTokenList GetModifiers(MemberDeclarationSyntax member)
            => member switch
            {
                FieldDeclarationSyntax field => field.Modifiers,
                PropertyDeclarationSyntax property => property.Modifiers,
                EventFieldDeclarationSyntax eventField => eventField.Modifiers,
                ConstructorDeclarationSyntax constructor => constructor.Modifiers,
                MethodDeclarationSyntax method => method.Modifiers,
                _ => default
            };

        private static int GetMemberKindOrder(MemberDeclarationSyntax member)
        {
            if (member is FieldDeclarationSyntax) { return 0; }
            if (member is PropertyDeclarationSyntax) { return 1; }
            if (member is EventFieldDeclarationSyntax) { return 2; }
            if (member is ConstructorDeclarationSyntax) { return 3; }
            if (member is MethodDeclarationSyntax) { return 4; }

            return -1;
        }

        private static int GetFieldKindOrder(FieldDeclarationSyntax field)
        {
            bool isStatic = field.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.StaticKeyword));
            bool isReadOnly = field.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.ReadOnlyKeyword));

            if (isStatic && isReadOnly) { return 0; }
            if (isStatic) { return 1; }
            if (isReadOnly) { return 2; }

            return 3;
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