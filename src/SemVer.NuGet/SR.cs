// Copyright © William Sugarman.
// Licensed under the MIT License.

using System.Globalization;
using System.Resources;

namespace SemVer.NuGet
{
    internal static class SR
    {
        public static string AmbiguousVersionFormat => ExceptionResourceManager.GetString(nameof(AmbiguousVersionFormat), CultureInfo.CurrentUICulture)!;

        public static string CompilationFailedFormat => ExceptionResourceManager.GetString(nameof(CompilationFailedFormat), CultureInfo.CurrentUICulture)!;

        public static string InvalidOutputTypeFormat => ExceptionResourceManager.GetString(nameof(InvalidOutputTypeFormat), CultureInfo.CurrentUICulture)!;

        public static string InvalidTargetFrameworkFormat => ExceptionResourceManager.GetString(nameof(InvalidTargetFrameworkFormat), CultureInfo.CurrentUICulture)!;

        public static string InvalidTypeSymbolFormat => ExceptionResourceManager.GetString(nameof(InvalidTypeSymbolFormat), CultureInfo.CurrentUICulture)!;

        public static string InvalidVersionFormat => ExceptionResourceManager.GetString(nameof(InvalidVersionFormat), CultureInfo.CurrentUICulture)!;

        public static string MissingLibFolderMessage => ExceptionResourceManager.GetString(nameof(MissingLibFolderMessage), CultureInfo.CurrentUICulture)!;

        public static string MissingTargetFrameworkMessage => ExceptionResourceManager.GetString(nameof(MissingTargetFrameworkMessage), CultureInfo.CurrentUICulture)!;

        public static string SourceNotFoundFormat => ExceptionResourceManager.GetString(nameof(SourceNotFoundFormat), CultureInfo.CurrentUICulture)!;

        public static string UnexpectedAssembliesFormat => ExceptionResourceManager.GetString(nameof(UnexpectedAssembliesFormat), CultureInfo.CurrentUICulture)!;

        public static string Format(string format, object? arg)
            => string.Format(CultureInfo.CurrentCulture, format, arg);

        public static string Format(string format, object? arg0, object? arg1)
            => string.Format(CultureInfo.CurrentCulture, format, arg0, arg1);

        public static string Format(string format, object? arg0, object? arg1, object? arg2)
            => string.Format(CultureInfo.CurrentCulture, format, arg0, arg1, arg2);

        private static readonly ResourceManager ExceptionResourceManager = new ResourceManager("SemVer.NuGet.Resources.Exceptions", typeof(SR).Assembly);
    }
}
