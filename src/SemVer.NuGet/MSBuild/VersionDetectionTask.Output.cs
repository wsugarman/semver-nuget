// Copyright © William Sugarman.
// Licensed under the MIT License.

using System;
using Microsoft.Build.Framework;
using NuGet.Versioning;
using SemVer.NuGet.Api;

namespace SemVer.NuGet.MSBuild
{
    partial class VersionDetectionTask
    {
        /// <summary>
        /// Gets or sets the next package version prefix based on the changes.
        /// </summary>
        /// <value>The prefix in the format of major.minor.patch.</value>
        [Output]
        public string NextPackageVersion { get; set; } = "";

        private void SetOutputProperties(ChangeSummary change, NuGetVersion defaultVersion)
        {
            // Note: we ignore legacy versions
            Log.LogMessage(MessageImportance.Low, "Setting output properties");

            NuGetVersion newVersion;
            switch (change.Kind)
            {
                case ChangeKind.New:
                    Log.LogMessage(MessageImportance.Normal, SR.Format(Messages.NewPackageFormat, PackageId, defaultVersion.ToNormalizedString()));
                    newVersion = defaultVersion;
                    break;
                case ChangeKind.None:
                case ChangeKind.Patch:
                    Log.LogMessage(MessageImportance.Normal, Messages.PatchChangeMessage);

                    newVersion = new NuGetVersion(
                        change.CurrentVersion!.Major,
                        change.CurrentVersion!.Minor,
                        change.CurrentVersion!.Patch + 1,
                        change.CurrentVersion!.Release);
                    break;
                case ChangeKind.Minor:
                    Log.LogMessage(MessageImportance.Normal, Messages.MinorChangeMessage);

                    newVersion = new NuGetVersion(
                        change.CurrentVersion!.Major,
                        change.CurrentVersion!.Minor + 1,
                        0,
                        change.CurrentVersion!.Release);
                    break;
                case ChangeKind.Major:
                    Log.LogMessage(MessageImportance.Normal, Messages.MajorChangeMessage);

                    newVersion = new NuGetVersion(
                        change.CurrentVersion!.Major + 1,
                        0,
                        0,
                        change.CurrentVersion!.Release);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(change));
            }

            NextPackageVersion = newVersion.ToNormalizedString();
            Log.LogMessage(MessageImportance.Normal, SR.Format(Messages.VersionDetectedFormat, NextPackageVersion));
        }
    }
}
