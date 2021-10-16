// Copyright © William Sugarman.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using SemVer.NuGet.Extensions;

namespace SemVer.NuGet.Roslyn
{
    // Based on the visitor here: https://stackoverflow.com/questions/64623689/get-all-types-from-compilation-using-roslyn

    internal class ExportedTypeVisitor : SymbolVisitor
    {
        public IReadOnlyCollection<INamedTypeSymbol> Types => _types;

        private readonly HashSet<INamedTypeSymbol> _types;
        private readonly CancellationToken _cancellationToken;

        public ExportedTypeVisitor(CancellationToken cancellation = default)
        {
#pragma warning disable RS1024 // This is a bug with the analyzer: https://github.com/dotnet/roslyn-analyzers/issues/3427
            _types = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
#pragma warning restore RS1024 // Compare symbols correctly
            _cancellationToken = cancellation;
        }

        public override void VisitAssembly(IAssemblySymbol symbol)
        {
            _cancellationToken.ThrowIfCancellationRequested();
            symbol.GlobalNamespace.Accept(this);
        }

        public override void VisitNamespace(INamespaceSymbol symbol)
        {
            foreach (INamespaceOrTypeSymbol namespaceOrType in symbol.GetMembers())
            {
                _cancellationToken.ThrowIfCancellationRequested();
                namespaceOrType.Accept(this);
            }
        }

        public override void VisitNamedType(INamedTypeSymbol symbol)
        {
            _cancellationToken.ThrowIfCancellationRequested();

            if (!symbol.IsAccessibleOutsideOfAssembly() || !_types.Add(symbol))
                return;

            ImmutableArray<INamedTypeSymbol> nestedTypes = symbol.GetTypeMembers();
            if (nestedTypes.IsDefaultOrEmpty)
                return;

            foreach (INamedTypeSymbol nestedType in nestedTypes)
            {
                _cancellationToken.ThrowIfCancellationRequested();
                nestedType.Accept(this);
            }
        }
    }
}
