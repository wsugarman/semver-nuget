// Copyright © William Sugarman.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using SemVer.NuGet.Extensions;

namespace SemVer.NuGet
{
    internal class NuGetClient : INuGetClient
    {
        private readonly IEnumerable<SourceRepository> _repositories;
        private readonly SourceCacheContext _sourceCacheContext;
        private readonly ISettings _settings;
        private readonly ILogger _logger;

        public NuGetClient(ISettings settings, SourceCacheContext sourceCacheContext, ILogger logger)
            : this(PackageSourceProvider.LoadPackageSources(settings).Select(x => Repository.Factory.GetCoreV3(x)), sourceCacheContext, settings, logger)
        { }

        public NuGetClient(IEnumerable<SourceRepository> repositories, SourceCacheContext sourceCacheContext, ISettings settings, ILogger logger)
        {
            _repositories = repositories ?? throw new ArgumentNullException(nameof(repositories));
            _sourceCacheContext = sourceCacheContext ?? throw new ArgumentNullException(nameof(sourceCacheContext));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<NuGetVersion?> GetLatestVersionAsync(
            string packageId,
            bool includePrerelease = false,
            bool includeUnlisted = false,
            CancellationToken cancellationToken = default)
        {
            if (packageId is null)
                throw new ArgumentNullException(nameof(packageId));

            NuGetVersion? latest = null;
            foreach (SourceRepository repository in _repositories)
            {
                PackageMetadataResource resource = await repository.GetResourceAsync<PackageMetadataResource>().ConfigureAwait(false);
                if (resource is not null)
                {
                    IEnumerable<IPackageSearchMetadata> results = await resource.GetMetadataAsync(
                        packageId,
                        includePrerelease,
                        includeUnlisted,
                        _sourceCacheContext,
                        _logger,
                        cancellationToken).ConfigureAwait(false);

                    NuGetVersion? sourceLatest = results
                        .Where(x => x.Identity.HasVersion)
                        .Select(x => x.Identity.Version)
                        .MaxOrDefault();

                    if (sourceLatest is not null)
                    {
                        if (latest is not null && latest != sourceLatest)
                            throw new InvalidOperationException(SR.Format(Exceptions.AmbiguousVersionFormat, packageId));

                        latest = sourceLatest;
                    }
                }
            }

            return latest;
        }

        [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "NuGet normalizes using lowercase.")]
        public async Task<IReadOnlyDictionary<NuGetFramework, Assembly>> GetLibAssembliesAsync(PackageIdentity packageIdentity, CancellationToken cancellationToken = default)
        {
            if (packageIdentity is null)
                throw new ArgumentNullException(nameof(packageIdentity));

            string packagesFolder = SettingsUtility.GetGlobalPackagesFolder(_settings);
            foreach (SourceRepository repository in _repositories)
            {
                DownloadResource resource = await repository.GetResourceAsync<DownloadResource>().ConfigureAwait(false);
                if (resource is not null)
                {
                    DownloadResourceResult result = await resource.GetDownloadResourceResultAsync(
                        packageIdentity,
                        new PackageDownloadContext(_sourceCacheContext),
                        packagesFolder,
                        _logger,
                        cancellationToken).ConfigureAwait(false);

                    if (result.Status == DownloadResourceResultStatus.Available)
                    {
                        Dictionary<NuGetFramework, Assembly> libs = new Dictionary<NuGetFramework, Assembly>();

                        // The package should be downloaded to the cache, and by convention
                        // will be located in the /lib/ folder based on the framework
                        DirectoryInfo libDir = new DirectoryInfo(
                            Path.Combine(
                                packagesFolder,
                                packageIdentity.Id.ToLowerInvariant(),
                                packageIdentity.Version.ToNormalizedString(),
                                "lib"));

                        if (!libDir.Exists)
                            throw new InvalidOperationException(Exceptions.MissingLibFolderMessage);

                        DirectoryInfo[] folders = libDir.GetDirectories();
                        if (folders.Length == 0)
                            throw new InvalidOperationException(Exceptions.MissingTargetFrameworkMessage);

                        foreach (DirectoryInfo folder in folders)
                        {
                            NuGetFramework nugetFramework = NuGetFramework.Parse(folder.Name);
                            if (nugetFramework is null)
                                throw new InvalidOperationException(SR.Format(Exceptions.InvalidTargetFrameworkFormat, folder));

                            FileInfo[] files = folder.GetFiles()
                                .Where(x => string.Equals(x.Extension, ".dll", StringComparison.OrdinalIgnoreCase)
                                    || string.Equals(x.Extension, ".exe", StringComparison.OrdinalIgnoreCase))
                                .ToArray();

                            if (files.Length != 1)
                                throw new InvalidOperationException(SR.Format(Exceptions.UnexpectedAssembliesFormat, files.Length));

                            libs.Add(nugetFramework, Assembly.LoadFrom(files[0].FullName));
                        }

                        return libs;
                    }
                }
            }

            return new Dictionary<NuGetFramework, Assembly>();
        }
    }
}
