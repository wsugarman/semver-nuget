// Copyright © William Sugarman.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.Build.Framework;
using NuGet.Frameworks;

namespace SemVer.NuGet.MSBuild
{
    partial class VersionDetectionTask
    {
        /// <summary>
        /// Gets or sets the NuGet package ID.
        /// </summary>
        /// <value>The name of the package in the NuGet feed.</value>
        [Required]
        public string PackageId { get; set; } = "";

        /// <summary>
        /// Gets or sets the name of the assembly within the NuGet package.
        /// </summary>
        /// <remarks>
        /// By default, this is the same as the <see cref="PackageId"/>.
        /// </remarks>
        /// <value>The name of the assembly file.</value>
        [Required]
        public string AssemblyName { get; set; } = "";

        /// <summary>
        /// Gets or sets the type of output type, like <c>Library</c> and <c>Exe</c>.
        /// </summary>
        /// <value>The output for the project.</value>
        [Required]
        public string OutputType { get; set; } = "";

        /// <summary>
        /// Gets or sets the moniker for the target framework like <c>net5.0</c>.
        /// </summary>
        /// <value>The target framework moniker.</value>
        [Required]
        public IReadOnlyCollection<string> TargetFrameworks { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Gets or sets the absolute path for the project.
        /// </summary>
        /// <value>The absolute project path.</value>
        [Required]
        public string ProjectPath { get; set; } = "";

        /// <summary>
        /// Gets or sets an optional flag indicating whether pre-release versions should be considered.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if pre-release versions taken into consideration; otherwise, <see langword="false"/>
        /// </value>
        public bool IncludePrerelease { get; set; }

        /// <summary>
        /// Gets or sets the optional default package version if the package has not yet been published.
        /// </summary>
        /// <remarks>
        /// If left unspecified, the value default version will be <c>1.0.0</c>.
        /// </remarks>
        /// <value>The default semantic version.</value>
        public string? DefaultVersion { get; set; }

        internal string AssemblyExtension
        {
            get
            {
                if (string.Equals(OutputType, "Library", StringComparison.OrdinalIgnoreCase))
                    return ".dll";
                else if (string.Equals(OutputType, "Exe", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(OutputType, "Winexe", StringComparison.OrdinalIgnoreCase))
                    return ".exe";
                else
                    throw new ArgumentException(SR.Format(Exceptions.InvalidOutputTypeFormat, OutputType));
            }
        }

        private NuGetPackageSpecification GetSpecification()
        {
            List<NuGetFramework> nugetFrameworks = new List<NuGetFramework>();
            foreach (string framework in TargetFrameworks)
            {
                NuGetFramework nugetFramework = NuGetFramework.Parse(framework);
                if (nugetFramework is null)
                    throw new InvalidOperationException(SR.Format(Exceptions.InvalidTargetFrameworkFormat, framework));

                nugetFrameworks.Add(nugetFramework);
            }

            return new NuGetPackageSpecification(
                PackageId,
                AssemblyName + AssemblyExtension,
                ProjectPath,
                nugetFrameworks,
                IncludePrerelease);
        }
    }
}
