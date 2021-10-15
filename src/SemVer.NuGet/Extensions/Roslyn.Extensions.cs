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

            return visitor.FoundTypes;
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

        public static TypeCategory GetCategory(this INamedTypeSymbol symbol)
        {
            if (symbol is null)
                throw new ArgumentNullException(nameof(symbol));

            if (!symbol.IsType)
                throw new ArgumentException(SR.Format(Exceptions.InvalidTypeSymbolFormat, symbol.Name), nameof(symbol));

            if (symbol.IsValueType)
                return TypeCategory.Struct;
            else if (symbol.IsReferenceType)
                return TypeCategory.Class;
            else
                throw new ArgumentOutOfRangeException(nameof(symbol));
        }
    }
}
