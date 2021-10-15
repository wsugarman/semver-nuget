// Copyright © William Sugarman.
// Licensed under the MIT License.

using System;
using SemVer.NuGet.Reflection;

namespace SemVer.NuGet.Extensions
{
    internal static class TypeExtensions
    {
        public static TypeCategory GetCategory(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.IsValueType ? TypeCategory.Struct : TypeCategory.Class;
        }
    }
}
