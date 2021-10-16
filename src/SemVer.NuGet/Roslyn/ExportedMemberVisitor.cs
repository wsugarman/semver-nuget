// Copyright © William Sugarman.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using SemVer.NuGet.Extensions;

namespace SemVer.NuGet.Roslyn
{
    internal class ExportedMemberVisitor : SymbolVisitor
    {
        public IReadOnlyCollection<IEventSymbol> Events => _events;

        public IReadOnlyCollection<IFieldSymbol> Fields => _fields;

        public IReadOnlyCollection<IMethodSymbol> Methods => _methods;

        public IReadOnlyCollection<IPropertySymbol> Properties => _properties;

        private readonly HashSet<IEventSymbol> _events;
        private readonly HashSet<IFieldSymbol> _fields;
        private readonly HashSet<IMethodSymbol> _methods;
        private readonly HashSet<IPropertySymbol> _properties;
        private readonly CancellationToken _cancellationToken;

        public ExportedMemberVisitor(CancellationToken cancellation = default)
        {
#pragma warning disable RS1024 // This is a bug with the analyzer: https://github.com/dotnet/roslyn-analyzers/issues/3427
            _events = new HashSet<IEventSymbol>(SymbolEqualityComparer.Default);
            _fields = new HashSet<IFieldSymbol>(SymbolEqualityComparer.Default);
            _methods = new HashSet<IMethodSymbol>(SymbolEqualityComparer.Default);
            _properties = new HashSet<IPropertySymbol>(SymbolEqualityComparer.Default);
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

            ImmutableArray<ISymbol> members = symbol.GetMembers();
            if (members.IsDefaultOrEmpty)
                return;

            foreach (ISymbol member in members)
            {
                _cancellationToken.ThrowIfCancellationRequested();
                member.Accept(this);
            }

            INamedTypeSymbol? baseType = symbol.BaseType;
            if (baseType is not null)
            {
                _cancellationToken.ThrowIfCancellationRequested();
                baseType.Accept(this);
            }
        }

        public override void VisitEvent(IEventSymbol symbol)
        {
            _cancellationToken.ThrowIfCancellationRequested();

            if (!symbol.IsAccessibleOutsideOfAssembly())
                return;

            _events.Add(symbol);
        }

        public override void VisitField(IFieldSymbol symbol)
        {
            _cancellationToken.ThrowIfCancellationRequested();

            if (!symbol.IsAccessibleOutsideOfAssembly())
                return;

            _fields.Add(symbol);
        }

        public override void VisitMethod(IMethodSymbol symbol)
        {
            _cancellationToken.ThrowIfCancellationRequested();

            if (!symbol.IsAccessibleOutsideOfAssembly())
                return;

            _methods.Add(symbol);
        }

        public override void VisitProperty(IPropertySymbol symbol)
        {
            _cancellationToken.ThrowIfCancellationRequested();

            if (!symbol.IsAccessibleOutsideOfAssembly())
                return;

            _properties.Add(symbol);
        }
    }
}
