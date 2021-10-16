// Copyright © William Sugarman.
// Licensed under the MIT License.

using System;
using SemVer.NuGet.Reflection;
using System.Runtime.CompilerServices;
using System.Linq;

namespace SemVer.NuGet.Extensions
{
    internal static class TypeExtensions
    {
        public static TypeDeclarationKind GetTypeDeclarationKind(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (type.IsEnum)
                return TypeDeclarationKind.Enum;
            else if (type.IsValueType)
                return TypeDeclarationKind.Struct;
            else if (type.IsInterface)
                return TypeDeclarationKind.Interface;
            else if (type.IsDelegate())
                return TypeDeclarationKind.Delegate;
            else
                return TypeDeclarationKind.Class;
        }

        public static bool IsReadOnly(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.CustomAttributes.Any(x => x.AttributeType == typeof(IsReadOnlyAttribute));
        }

        public static bool IsRefLikeType(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.CustomAttributes.Any(x => x.AttributeType == typeof(IsByRefLikeAttribute));
        }

        public static bool IsStatic(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.IsAbstract && type.IsSealed;
        }

        public static bool IsDelegate(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            // System.Delegate is a special type from which normal types cannot inherit
            return type.InheritsFrom(typeof(Delegate));
        }

        public static bool InheritsFrom(this Type type, Type baseType)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (baseType is null)
                throw new ArgumentNullException(nameof(baseType));

            while (type.BaseType is not null && type.BaseType != baseType)
                type = type.BaseType;

            return type.BaseType is not null;
        }
    }
}
