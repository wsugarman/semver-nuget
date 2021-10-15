// Copyright © William Sugarman.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using NuGet.Frameworks;

namespace SemVer.NuGet.Api
{
    internal class ChangeSummaryBuilder
    {
        private readonly Dictionary<string, List<NuGetFramework>> _majorChanges = new Dictionary<string, List<NuGetFramework>>();
        private readonly Dictionary<string, List<NuGetFramework>> _minorChanges = new Dictionary<string, List<NuGetFramework>>();

        public void Add(NuGetFramework nugetFramework, ChangeKind kind, string description)
        {
            if (nugetFramework is null)
                throw new ArgumentNullException(nameof(nugetFramework));

            Dictionary<string, List<NuGetFramework>> changes = kind switch
            {
                ChangeKind.Major => _majorChanges,
                ChangeKind.Minor => _minorChanges,
                _ => throw new ArgumentOutOfRangeException(nameof(kind)),
            };

            if (!changes.TryGetValue(description, out List<NuGetFramework>? frameworks))
            {
                frameworks = new List<NuGetFramework>();
                changes[description] = frameworks;
            }

            frameworks.Add(nugetFramework);
        }

        public ChangeSummary Finalize()
            => new ChangeSummary(_majorChanges, _minorChanges);
    }
}
