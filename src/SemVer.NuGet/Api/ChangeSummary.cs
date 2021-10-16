// Copyright © William Sugarman.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using NuGet.Frameworks;
using NuGet.Versioning;

namespace SemVer.NuGet.Api
{
    internal class ChangeSummary
    {
        public NuGetVersion? CurrentVersion { get; set; }

        public ChangeKind Kind { get; }

        public IReadOnlyDictionary<ChangeKind, IReadOnlyList<CodeChange>> Changes { get; }

        public ChangeSummary(NuGetVersion version, Dictionary<string, List<NuGetFramework>> majorChanges, Dictionary<string, List<NuGetFramework>> minorChanges)
        {
            CurrentVersion = version ?? throw new ArgumentNullException(nameof(version));

            Dictionary<ChangeKind, IReadOnlyList<CodeChange>> changes = new Dictionary<ChangeKind, IReadOnlyList<CodeChange>>();
            if (majorChanges is not null && majorChanges.Count > 0)
                changes.Add(ChangeKind.Major, majorChanges.Select(p => new CodeChange(p.Key, p.Value)).ToList());

            if (minorChanges is not null && minorChanges.Count > 0)
                changes.Add(ChangeKind.Minor, minorChanges.Select(p => new CodeChange(p.Key, p.Value)).ToList());

            Changes = changes;

            if (changes.ContainsKey(ChangeKind.Major))
                Kind = ChangeKind.Major;
            else if (changes.ContainsKey(ChangeKind.Minor))
                Kind = ChangeKind.Minor;
            else
                Kind = ChangeKind.None;
        }

        private ChangeSummary()
        {
            CurrentVersion = null;
            Kind = ChangeKind.New;
            Changes = new Dictionary<ChangeKind, IReadOnlyList<CodeChange>>();
        }

        public static ChangeSummary New()
            => new ChangeSummary();
    }
}
