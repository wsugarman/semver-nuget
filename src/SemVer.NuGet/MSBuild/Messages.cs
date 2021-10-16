// Copyright © William Sugarman.
// Licensed under the MIT License.

using System.Globalization;
using System.Resources;

namespace SemVer.NuGet.Api
{
    internal static class Messages
    {
        public static string ChangeFormat => MessageResourceManager.GetString(nameof(ChangeFormat), CultureInfo.CurrentUICulture)!;

        public static string ChangeHeaderFormat => MessageResourceManager.GetString(nameof(ChangeHeaderFormat), CultureInfo.CurrentUICulture)!;

        public static string MajorChangeMessage => MessageResourceManager.GetString(nameof(MajorChangeMessage), CultureInfo.CurrentUICulture)!;

        public static string MinorChangeMessage => MessageResourceManager.GetString(nameof(MinorChangeMessage), CultureInfo.CurrentUICulture)!;

        public static string NewPackageFormat => MessageResourceManager.GetString(nameof(NewPackageFormat), CultureInfo.CurrentUICulture)!;

        public static string PatchChangeMessage => MessageResourceManager.GetString(nameof(PatchChangeMessage), CultureInfo.CurrentUICulture)!;

        public static string VersionDetectedFormat => MessageResourceManager.GetString(nameof(VersionDetectedFormat), CultureInfo.CurrentUICulture)!;

        private static readonly ResourceManager MessageResourceManager = new ResourceManager("SemVer.NuGet.Resources.Messages", typeof(Changes).Assembly);
    }
}
