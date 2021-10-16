// Copyright © William Sugarman.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace SemVer.NuGet.Roslyn
{
    internal sealed class ExportedMemberSymbolInfo
    {
        public IReadOnlyCollection<IEventSymbol> Events { get; }

        public IReadOnlyCollection<IFieldSymbol> Fields { get; }

        public IReadOnlyCollection<IMethodSymbol> Methods { get; }

        public IReadOnlyCollection<IPropertySymbol> Properties { get; }

        public ExportedMemberSymbolInfo(
            IReadOnlyCollection<IEventSymbol> events,
            IReadOnlyCollection<IFieldSymbol> fields,
            IReadOnlyCollection<IMethodSymbol> methods,
            IReadOnlyCollection<IPropertySymbol> properties)
        {
            Events = events ?? throw new ArgumentNullException(nameof(events));
            Fields = fields ?? throw new ArgumentNullException(nameof(fields));
            Methods = methods ?? throw new ArgumentNullException(nameof(methods));
            Properties = properties ?? throw new ArgumentNullException(nameof(properties));
        }
    }
}
