// Copyright © William Sugarman.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis;
using SemVer.NuGet.Reflection;
using SemVer.NuGet.Roslyn;

namespace SemVer.NuGet.Extensions
{
    internal static class RoslynExtensions
    {
        public static IEnumerable<INamedTypeSymbol> GetExportedTypes(this Compilation compilation, CancellationToken cancellationToken = default)
        {
            ExportedTypeVisitor visitor = new ExportedTypeVisitor(cancellationToken);
            visitor.Visit(compilation.GlobalNamespace);

            return visitor.Types;
        }

        public static bool IsAccessibleOutsideOfAssembly(this ISymbol symbol)
        {
            if (symbol is null)
                throw new ArgumentNullException(nameof(symbol));

            switch (symbol.DeclaredAccessibility)
            {
                case Accessibility.Protected:
                case Accessibility.ProtectedOrInternal:
                case Accessibility.Public:
                    return true;
                default:
                    return false;
            }
        }

        public static TypeDeclarationKind GetTypeDeclarationKind(this INamedTypeSymbol symbol)
        {
            if (symbol is null)
                throw new ArgumentNullException(nameof(symbol));

            if (!symbol.IsType)
                throw new ArgumentException(SR.Format(Exceptions.InvalidTypeSymbolFormat, symbol.Name), nameof(symbol));

            return symbol.TypeKind switch
            {
                TypeKind.Enum => TypeDeclarationKind.Enum,
                TypeKind.Struct => TypeDeclarationKind.Struct,
                TypeKind.Class => TypeDeclarationKind.Class,
                TypeKind.Interface => TypeDeclarationKind.Interface,
                TypeKind.Delegate => TypeDeclarationKind.Delegate,
                _ => throw new ArgumentOutOfRangeException(nameof(symbol))
            };
        }

        public static ExportedMemberSymbolInfo GetExportedMembers(this INamedTypeSymbol symbol, CancellationToken cancellationToken = default)
        {
            if (symbol is null)
                throw new ArgumentNullException(nameof(symbol));

            if (!symbol.IsType)
                throw new ArgumentException(SR.Format(Exceptions.InvalidTypeSymbolFormat, symbol.Name), nameof(symbol));

            ExportedMemberVisitor visitor = new ExportedMemberVisitor(cancellationToken);
            visitor.Visit(symbol);

            return new ExportedMemberSymbolInfo(visitor.Events, visitor.Fields, visitor.Methods, visitor.Properties);
        }
    }
}
