// Copyright © William Sugarman.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace SemVer.NuGet.Api
{
    internal class ChangeSummary
    {
        public ChangeKind Kind { get; }

        public IReadOnlyDictionary<ChangeKind, IReadOnlyList<string>> Changes { get; }

        public ChangeSummary(IReadOnlyList<string> majorChanges, IReadOnlyList<string> minorChanges)
        {
            Dictionary<ChangeKind, IReadOnlyList<string>> changes = new Dictionary<ChangeKind, IReadOnlyList<string>>();
            if (majorChanges is not null && majorChanges.Count > 0)
                changes.Add(ChangeKind.Major, majorChanges);

            if (minorChanges is not null && minorChanges.Count > 0)
                changes.Add(ChangeKind.Minor, minorChanges);

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
            Changes = new Dictionary<ChangeKind, IReadOnlyList<string>>();
        }

        public static ChangeSummary New()
            => new ChangeSummary(ChangeKind.New);
    }
}
