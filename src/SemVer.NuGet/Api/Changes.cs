// Copyright © William Sugarman.
// Licensed under the MIT License.

using System.Globalization;
using System.Resources;

namespace SemVer.NuGet.Api
{
    internal static class Changes
    {
        public static string AssemblyRenameFormat => ChangeResourceManager.GetString(nameof(AssemblyRenameFormat), CultureInfo.CurrentUICulture)!;

        public static string ClassAbstractAddedFormat => ChangeResourceManager.GetString(nameof(ClassAbstractAddedFormat), CultureInfo.CurrentUICulture)!;

        public static string ClassAbstractRemovedFormat => ChangeResourceManager.GetString(nameof(ClassAbstractRemovedFormat), CultureInfo.CurrentUICulture)!;

        public static string ClassSealedAddedFormat => ChangeResourceManager.GetString(nameof(ClassSealedAddedFormat), CultureInfo.CurrentUICulture)!;

        public static string ClassSealedRemovedFormat => ChangeResourceManager.GetString(nameof(ClassSealedRemovedFormat), CultureInfo.CurrentUICulture)!;

        public static string ClassStaticAddedFormat => ChangeResourceManager.GetString(nameof(ClassStaticAddedFormat), CultureInfo.CurrentUICulture)!;

        public static string ClassStaticRemovedFormat => ChangeResourceManager.GetString(nameof(ClassStaticRemovedFormat), CultureInfo.CurrentUICulture)!;

        public static string EnumUnderlyingTypeChangedFormat => ChangeResourceManager.GetString(nameof(EnumUnderlyingTypeChangedFormat), CultureInfo.CurrentUICulture)!;

        public static string StructReadOnlyAddedFormat => ChangeResourceManager.GetString(nameof(StructReadOnlyAddedFormat), CultureInfo.CurrentUICulture)!;

        public static string StructReadOnlyRemovedFormat => ChangeResourceManager.GetString(nameof(StructReadOnlyRemovedFormat), CultureInfo.CurrentUICulture)!;

        public static string StructRefAddedFormat => ChangeResourceManager.GetString(nameof(StructRefAddedFormat), CultureInfo.CurrentUICulture)!;

        public static string StructRefRemovedFormat => ChangeResourceManager.GetString(nameof(StructRefRemovedFormat), CultureInfo.CurrentUICulture)!;

        public static string TargetFrameworkAddedFormat => ChangeResourceManager.GetString(nameof(TargetFrameworkAddedFormat), CultureInfo.CurrentUICulture)!;

        public static string TargetFrameworkRemovedFormat => ChangeResourceManager.GetString(nameof(TargetFrameworkRemovedFormat), CultureInfo.CurrentUICulture)!;

        public static string TypeAddedFormat => ChangeResourceManager.GetString(nameof(TypeAddedFormat), CultureInfo.CurrentUICulture)!;

        public static string TypeCategoryChangeFormat => ChangeResourceManager.GetString(nameof(TypeCategoryChangeFormat), CultureInfo.CurrentUICulture)!;

        public static string TypeRemovedFormat => ChangeResourceManager.GetString(nameof(TypeRemovedFormat), CultureInfo.CurrentUICulture)!;

        private static readonly ResourceManager ChangeResourceManager = new ResourceManager("SemVer.NuGet.Resources.Changes", typeof(Changes).Assembly);
    }
}
