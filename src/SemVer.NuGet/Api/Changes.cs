// Copyright © William Sugarman.
// Licensed under the MIT License.

using System.Globalization;
using System.Resources;

namespace SemVer.NuGet.Api
{
    internal static class Changes
    {
        public static string AssemblyRenameFormat => ChangeResourceManager.GetString(nameof(AssemblyRenameFormat), CultureInfo.CurrentUICulture)!;

        public static string TargetFrameworkAddedFormat => ChangeResourceManager.GetString(nameof(TargetFrameworkAddedFormat), CultureInfo.CurrentUICulture)!;

        public static string TargetFrameworkRemovedFormat => ChangeResourceManager.GetString(nameof(TargetFrameworkRemovedFormat), CultureInfo.CurrentUICulture)!;

        private static readonly ResourceManager ChangeResourceManager = new ResourceManager("SemVer.NuGet.Resources.Changes", typeof(Changes).Assembly);
    }
}
