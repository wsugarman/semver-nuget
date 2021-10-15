// Copyright © William Sugarman.
// Licensed under the MIT License.

using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace SemVer.NuGet.Console
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            ISettings settings = Settings.LoadDefaultSettings(@"C:\Git\Sweetener");

            using SourceCacheContext sourceCacheContext = new SourceCacheContext();
            var client = new NuGetClient(settings, sourceCacheContext, NullLogger.Instance);

            NuGetVersion? version = await client.GetLatestVersionAsync(
                "Sweetener.Linq",
                true,
                includeUnlisted: true,
                CancellationToken.None).ConfigureAwait(false);

            if (version is not null)
            {
                var lib = await client.GetLibAssembliesAsync(
                    new PackageIdentity("Sweetener.Linq", version),
                    CancellationToken.None).ConfigureAwait(false);

                foreach ((NuGetFramework _, Assembly assembly) in lib)
                {
                    System.Console.WriteLine(assembly.Location);
                }
            }
        }
    }
}
