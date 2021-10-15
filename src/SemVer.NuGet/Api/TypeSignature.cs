// Copyright © William Sugarman.
// Licensed under the MIT License.

using System;
using System.Text;
using Microsoft.CodeAnalysis;

namespace SemVer.NuGet.Api
{
    internal class TypeSignature : IEquatable<TypeSignature>
    {
        public string? Namespace { get; }

        public string Name { get; }

        public bool IsGenericType => TypeParameterCount > 0;

        public int TypeParameterCount { get; }

        public TypeSignature(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            Namespace = type.Namespace;
            Name = type.Name;
            TypeParameterCount = type.GetGenericTypeDefinition().GetGenericArguments().Length;
        }

        public TypeSignature(INamedTypeSymbol symbol)
        {
            if (symbol is null)
                throw new ArgumentNullException(nameof(symbol));

            if (!symbol.IsType)
                throw new ArgumentException(SR.Format(SR.InvalidTypeSymbolFormat, symbol.Name), nameof(symbol));

            Namespace = symbol.ContainingNamespace?.Name;
            Name = symbol.Name;
            TypeParameterCount = symbol.TypeParameters.Length;
        }

        public override bool Equals(object? obj)
            => obj is TypeSignature other && Equals(other);

        public bool Equals(TypeSignature? other)
            => other is not null
                && string.Equals(Namespace, other.Namespace, StringComparison.Ordinal)
                && string.Equals(Name, other.Name, StringComparison.Ordinal)
                && TypeParameterCount == other.TypeParameterCount;

        public override int GetHashCode()
            => HashCode.Combine(Namespace, Name, TypeParameterCount);

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(Namespace))
            {
                builder.Append(Namespace);
                builder.Append('.');
            }

            builder.Append(Name);
            if (IsGenericType)
            {
                builder.Append('`');
                builder.Append(TypeParameterCount);
            }

            return builder.ToString();
        }
    }
}
