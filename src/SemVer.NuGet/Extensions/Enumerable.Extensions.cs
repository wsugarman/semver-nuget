// Copyright © William Sugarman.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace SemVer.NuGet.Extensions
{
    internal static class EnumerableExtensions
    {
        public static TSource? MaxOrDefault<TSource>(this IEnumerable<TSource> source)
            where TSource : IComparable<TSource>
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            TSource? max = default;
            foreach (TSource value in source)
            {
                if (Comparer<TSource?>.Default.Compare(value, max) > 0)
                    max = value;
            }

            return max;
        }
    }
}
