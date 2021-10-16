// Copyright © William Sugarman.
// Licensed under the MIT License.

using System.Globalization;
using System.Resources;

namespace SemVer.NuGet
{
    internal static class Exceptions
    {
        public static string AmbiguousVersionFormat => ExceptionResourceManager.GetString(nameof(AmbiguousVersionFormat), CultureInfo.CurrentUICulture)!;

        public static string CompilationFailedFormat => ExceptionResourceManager.GetString(nameof(CompilationFailedFormat), CultureInfo.CurrentUICulture)!;

        public static string InvalidDefaultVersionFormat => ExceptionResourceManager.GetString(nameof(InvalidDefaultVersionFormat), CultureInfo.CurrentUICulture)!;

        public static string InvalidLatestVersionFormat => ExceptionResourceManager.GetString(nameof(InvalidLatestVersionFormat), CultureInfo.CurrentUICulture)!;

        public static string InvalidOutputTypeFormat => ExceptionResourceManager.GetString(nameof(InvalidOutputTypeFormat), CultureInfo.CurrentUICulture)!;

        public static string InvalidTargetFrameworkFormat => ExceptionResourceManager.GetString(nameof(InvalidTargetFrameworkFormat), CultureInfo.CurrentUICulture)!;

        public static string InvalidTypeSymbolFormat => ExceptionResourceManager.GetString(nameof(InvalidTypeSymbolFormat), CultureInfo.CurrentUICulture)!;

        public static string MissingLibFolderMessage => ExceptionResourceManager.GetString(nameof(MissingLibFolderMessage), CultureInfo.CurrentUICulture)!;

        public static string MissingTargetFrameworkMessage => ExceptionResourceManager.GetString(nameof(MissingTargetFrameworkMessage), CultureInfo.CurrentUICulture)!;

        public static string SourceNotFoundFormat => ExceptionResourceManager.GetString(nameof(SourceNotFoundFormat), CultureInfo.CurrentUICulture)!;

        public static string UnexpectedAssembliesFormat => ExceptionResourceManager.GetString(nameof(UnexpectedAssembliesFormat), CultureInfo.CurrentUICulture)!;

        private static readonly ResourceManager ExceptionResourceManager = new ResourceManager("SemVer.NuGet.Resources.Exceptions", typeof(Exceptions).Assembly);
    }
}
