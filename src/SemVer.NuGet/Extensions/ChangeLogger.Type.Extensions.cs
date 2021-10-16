// Copyright © William Sugarman.
// Licensed under the MIT License.

using System;
using Microsoft.CodeAnalysis;
using NuGet.Frameworks;
using SemVer.NuGet.Api;
using SemVer.NuGet.Roslyn;

namespace SemVer.NuGet.Extensions
{
    static partial class ChangeLoggerExtensions
    {
        private static void AddIfStructChanged(
            this ChangeLogger changes,
            NuGetFramework targetFramework,
            TypeSignature typeSignature,
            Type before,
            INamedTypeSymbol after)
        {
            // Readonly
            if (before.IsReadOnly() != after.IsReadOnly)
            {
                if (after.IsReadOnly)
                    changes.Add(targetFramework, ChangeKind.Major, SR.Format(Changes.StructReadOnlyAddedFormat, typeSignature));
                else
                    changes.Add(targetFramework, ChangeKind.Major, SR.Format(Changes.StructReadOnlyRemovedFormat, typeSignature));
            }

            // ByRef
            if (before.IsRefLikeType() != after.IsRefLikeType)
            {
                if (after.IsRefLikeType)
                    changes.Add(targetFramework, ChangeKind.Major, SR.Format(Changes.StructRefAddedFormat, typeSignature));
                else
                    changes.Add(targetFramework, ChangeKind.Major, SR.Format(Changes.StructRefRemovedFormat, typeSignature));
            }

            // Members
            ExportedMemberSymbolInfo afterMembers = after.GetExportedMembers();

            // Interfaces
        }

        private static void AddIfEnumChanged(
            this ChangeLogger changes,
            NuGetFramework targetFramework,
            TypeSignature typeSignature,
            Type before,
            INamedTypeSymbol after)
        {
            string beforeTypeName = before.GetEnumUnderlyingType().Name;
            string afterTypeName = after.EnumUnderlyingType!.Name;

            // Underlying type changed?
            if (beforeTypeName.Equals(afterTypeName, StringComparison.Ordinal))
                changes.Add(
                    targetFramework,
                    ChangeKind.Major,
                    SR.Format(Changes.EnumUnderlyingTypeChangedFormat, typeSignature, beforeTypeName, afterTypeName));

            // Members
        }

        private static void AddIfClassChanged(
            this ChangeLogger changes,
            NuGetFramework targetFramework,
            TypeSignature typeSignature,
            Type before,
            INamedTypeSymbol after)
        {
            // Static
            if (before.IsStatic() != after.IsStatic)
            {
                if (after.IsStatic)
                    changes.Add(targetFramework, ChangeKind.Major, SR.Format(Changes.ClassStaticAddedFormat, typeSignature));
                else
                    changes.Add(targetFramework, ChangeKind.Minor, SR.Format(Changes.ClassStaticRemovedFormat, typeSignature));
            }
            else if (!before.IsStatic())
            {
                // Abstract
                if (before.IsAbstract != after.IsAbstract)
                {
                    if (after.IsAbstract)
                        changes.Add(targetFramework, ChangeKind.Major, SR.Format(Changes.ClassAbstractAddedFormat, typeSignature));
                    else
                        changes.Add(targetFramework, ChangeKind.Minor, SR.Format(Changes.ClassAbstractRemovedFormat, typeSignature));
                }

                // Sealed
                if (before.IsSealed != after.IsSealed)
                {
                    if (after.IsSealed)
                        changes.Add(targetFramework, ChangeKind.Major, SR.Format(Changes.ClassSealedAddedFormat, typeSignature));
                    else
                        changes.Add(targetFramework, ChangeKind.Minor, SR.Format(Changes.ClassSealedRemovedFormat, typeSignature));
                }

            }

            // Members

            // Interfaces

            // Parent
        }

        private static void AddIfInterfaceChanged(
            this ChangeLogger changes,
            NuGetFramework targetFramework,
            TypeSignature typeSignature,
            Type before,
            INamedTypeSymbol after)
        {
            // Members

            // Interfaces
        }

        private static void AddIfDelegateChanged(
            this ChangeLogger changes,
            NuGetFramework targetFramework,
            TypeSignature typeSignature,
            Type before,
            INamedTypeSymbol after)
        {

        }
    }
}
