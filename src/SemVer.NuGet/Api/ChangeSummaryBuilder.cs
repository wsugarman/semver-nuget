// Copyright © William Sugarman.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace SemVer.NuGet.Api
{
    internal class ChangeSummaryBuilder
    {
        private readonly List<string> _majorChanges = new List<string>();
        private readonly List<string> _minorChanges = new List<string>();

        public void Add(ChangeKind kind, string description)
        {
            // TODO: Support patch if necessary
            switch (kind)
            {
                case ChangeKind.Major:
                    _majorChanges.Add(description);
                    break;
                case ChangeKind.Minor:
                    _minorChanges.Add(description);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind));
            }
        }

        public ChangeSummary Finalize()
            => new ChangeSummary(_majorChanges, _minorChanges);
    }
}
