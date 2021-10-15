// Copyright © William Sugarman.
// Licensed under the MIT License.

using System;
using Microsoft.CodeAnalysis;
using NuGet.Frameworks;
using SemVer.NuGet.Api;

namespace SemVer.NuGet.Extensions
{
    internal static class ChangeSummaryBuilderExtensions
    {
        public static void AddIfChanged(
            this ChangeSummaryBuilder changes,
            NuGetFramework targetFramework,
            TypeSignature typeSignature,
            Type before,
            INamedTypeSymbol after)
        {
            // Check signature/declaration

            // Interfaces

            // Check generic constraints

            // Check members
        }

        private static void AddIfTypeDeclarationChanged(
            this ChangeSummaryBuilder changes,
            NuGetFramework targetFramework,
            TypeSignature typeSignature,
            Type before,
            INamedTypeSymbol after)
        {
            if (before.IsValueType != after.IsValueType) // Only valid for non-parameter types
                changes.Add(
                    targetFramework,
                    ChangeKind.Major,
                    SR.Format(Changes.TypeCategoryChangeFormat, typeSignature, before.GetCategory(), after.GetCategory()));

            // TODO: Are changes to a structs readonly status a breaking change?
        }
    }
}
