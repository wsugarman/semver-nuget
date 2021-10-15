// Copyright © William Sugarman.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using NuGet.Frameworks;

namespace SemVer.NuGet.Api
{
    internal class CodeChange
    {
        public string Description { get; }

        public IReadOnlyCollection<NuGetFramework> TargetFrameworks { get; }

        public CodeChange(string description, IReadOnlyCollection<NuGetFramework> targetFrameworks)
        {
            Description = description ?? throw new ArgumentNullException(nameof(description));
            TargetFrameworks = targetFrameworks ?? throw new ArgumentNullException(nameof(targetFrameworks));
        }
    }
}
