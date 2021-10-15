// Copyright © William Sugarman.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using NuGet.Frameworks;

namespace SemVer.NuGet.Api
{
    internal class ChangeSummary
    {
        public ChangeKind Kind { get; }

        public IReadOnlyDictionary<ChangeKind, IReadOnlyList<CodeChange>> Changes { get; }

        public ChangeSummary(Dictionary<string, List<NuGetFramework>> majorChanges, Dictionary<string, List<NuGetFramework>> minorChanges)
        {
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

        private ChangeSummary(ChangeKind kind)
        {
            Kind = kind;
            Changes = new Dictionary<ChangeKind, IReadOnlyList<CodeChange>>();
        }

        public static ChangeSummary New()
            => new ChangeSummary(ChangeKind.New);
    }
}
