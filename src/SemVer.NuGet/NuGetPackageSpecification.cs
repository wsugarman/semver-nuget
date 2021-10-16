// Copyright © William Sugarman.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using NuGet.Frameworks;
using NuGet.Versioning;

namespace SemVer.NuGet
{
    internal class NuGetPackageSpecification
    {
        public string PackageId { get; }

        public string AssemblyName { get; }

        public IReadOnlyCollection<NuGetFramework> TargetFrameworks { get; }

        public bool IncludePrerelease { get; }

        public string ProjectPath { get; }

        public NuGetVersion DefaultVersion { get; }

        public NuGetPackageSpecification(
            string packageId,
            string projectPath,
            IReadOnlyCollection<NuGetFramework> targetFrameworks,
            bool includePrerelease = false,
            NuGetVersion? defaultVersion = null)
            : this(packageId, projectPath, packageId + ".dll", targetFrameworks, includePrerelease, defaultVersion)
        { }

        public NuGetPackageSpecification(
            string packageId,
            string assemblyFile,
            string projectPath,
            IReadOnlyCollection<NuGetFramework> targetFrameworks,
            bool includePrerelease = false,
            NuGetVersion? defaultVersion = null)
        {
            if (string.IsNullOrWhiteSpace(packageId))
                throw new ArgumentNullException(nameof(packageId));

            if (string.IsNullOrWhiteSpace(assemblyFile))
                throw new ArgumentNullException(nameof(assemblyFile));

            if (string.IsNullOrWhiteSpace(projectPath))
                throw new ArgumentNullException(nameof(projectPath));

            if (targetFrameworks is null)
                throw new ArgumentNullException(nameof(targetFrameworks));

            if (targetFrameworks.Count == 0)
                throw new ArgumentOutOfRangeException(nameof(targetFrameworks));

            defaultVersion ??= new NuGetVersion(1, 0, 0);
            if (defaultVersion.IsLegacyVersion || !defaultVersion.IsSemVer2)
                throw new ArgumentException(
                    SR.Format(Exceptions.InvalidDefaultVersionFormat, defaultVersion.ToNormalizedString()),
                    nameof(defaultVersion));

            PackageId = packageId;
            AssemblyName = assemblyFile;
            ProjectPath = projectPath;
            TargetFrameworks = targetFrameworks;
            IncludePrerelease = includePrerelease;
            DefaultVersion = defaultVersion;
        }
    }
}
