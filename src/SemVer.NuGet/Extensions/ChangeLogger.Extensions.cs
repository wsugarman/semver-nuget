// Copyright © William Sugarman.
// Licensed under the MIT License.

using System;
using Microsoft.CodeAnalysis;
using NuGet.Frameworks;
using SemVer.NuGet.Api;
using SemVer.NuGet.Reflection;

namespace SemVer.NuGet.Extensions
{
    internal static partial class ChangeLoggerExtensions
    {
        public static void AddIfChanged(
            this ChangeLogger changes,
            NuGetFramework targetFramework,
            TypeSignature typeSignature,
            Type before,
            INamedTypeSymbol after)
        {
            TypeDeclarationKind beforeKind = before.GetTypeDeclarationKind();
            TypeDeclarationKind afterKind = after.GetTypeDeclarationKind();

            if (beforeKind != afterKind) // Only valid for non-parameter types
            {
                changes.Add(
                    targetFramework,
                    ChangeKind.Major,
                    SR.Format(Changes.TypeCategoryChangeFormat, typeSignature, beforeKind, afterKind));
            }
            else
            {
                switch (afterKind)
                {
                    case TypeDeclarationKind.Class:
                        changes.AddIfClassChanged(targetFramework, typeSignature, before, after);
                        break;
                    case TypeDeclarationKind.Struct:
                        changes.AddIfStructChanged(targetFramework, typeSignature, before, after);
                        break;
                    case TypeDeclarationKind.Enum:
                        changes.AddIfEnumChanged(targetFramework, typeSignature, before, after);
                        break;
                    case TypeDeclarationKind.Interface:
                        changes.AddIfInterfaceChanged(targetFramework, typeSignature, before, after);
                        break;
                    case TypeDeclarationKind.Delegate:
                        changes.AddIfDelegateChanged(targetFramework, typeSignature, before, after);
                        break;
                }
            }
        }
    }
}
