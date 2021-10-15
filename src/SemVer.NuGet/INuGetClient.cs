// Copyright © William Sugarman.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Versioning;

namespace SemVer.NuGet
{
    internal interface INuGetClient
    {
        Task<NuGetVersion?> GetLatestVersionAsync(
            string packageId,
            bool includePrerelease = false,
            bool includeUnlisted = false,
            CancellationToken cancellationToken = default);

         Task<IReadOnlyDictionary<NuGetFramework, Assembly>> GetLibAssembliesAsync(
            PackageIdentity packageIdentity,
            CancellationToken cancellationToken = default);
    }
}
