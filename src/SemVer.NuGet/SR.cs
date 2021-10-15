// Copyright © William Sugarman.
// Licensed under the MIT License.

using System.Globalization;

namespace SemVer.NuGet
{
    internal static class SR
    {
        public static string Format(string format, object? arg)
            => string.Format(CultureInfo.CurrentCulture, format, arg);

        public static string Format(string format, object? arg0, object? arg1)
            => string.Format(CultureInfo.CurrentCulture, format, arg0, arg1);

        public static string Format(string format, object? arg0, object? arg1, object? arg2)
            => string.Format(CultureInfo.CurrentCulture, format, arg0, arg1, arg2);
    }
}
