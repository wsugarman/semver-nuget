// Copyright © William Sugarman.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using SemVer.NuGet.Api;
using SemVer.NuGet.Extensions;

namespace SemVer.NuGet
{
    internal class NuGetPackageDiff
    {
        private readonly INuGetClient _client;
        private readonly NuGetPackageSpecification _specification;

        public NuGetPackageDiff(INuGetClient client, NuGetPackageSpecification specification)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _specification = specification ?? throw new ArgumentNullException(nameof(specification));
        }

        public async Task<ChangeSummary> GetChangesAsync(CancellationToken cancellationToken = default)
        {
            PackageMetadata? latest = await GetLastPublishedAsync(cancellationToken).ConfigureAwait(false);
            if (latest is null)
                return ChangeSummary.New(); // We have not yet published this package

            ChangeLogger changes = new ChangeLogger();

            // Compare each framework
            // (Resolve all of the frameworks to front-load the framework differences in the change list)
            MSBuildLocator.RegisterDefaults();
            List<(NuGetFramework, Assembly)> comparisons = CompareFrameworks(latest.Lib, changes).ToList();
            foreach ((NuGetFramework framework, Assembly before) in comparisons)
            {
                Compilation? after = await CompileAsync(framework, cancellationToken).ConfigureAwait(false);
                if (after is null)
                    throw new InvalidOperationException(SR.Format(Exceptions.CompilationFailedFormat, framework));

                CompareAssemblies(framework, before, after, changes);
            }

            return changes.Summarize(latest.Identity.Version);
        }

        private async Task<PackageMetadata?> GetLastPublishedAsync(CancellationToken cancellationToken = default)
        {
            NuGetVersion? version = await _client.GetLatestVersionAsync(
                _specification.PackageId,
                _specification.IncludePrerelease,
                includeUnlisted: true,
                cancellationToken).ConfigureAwait(false);

            if (version is null)
                return null;

            if (version.IsLegacyVersion || !version.IsSemVer2)
                throw new InvalidOperationException(SR.Format(Exceptions.InvalidLatestVersionFormat, version.ToNormalizedString(), _specification.PackageId));

            return new PackageMetadata(
                new PackageIdentity(_specification.PackageId, version),
                await _client.GetLibAssembliesAsync(
                    new PackageIdentity(_specification.PackageId, version),
                    cancellationToken).ConfigureAwait(false));
        }

        private IEnumerable<(NuGetFramework, Assembly)> CompareFrameworks(IReadOnlyDictionary<NuGetFramework, Assembly> found, ChangeLogger changes)
        {
            Dictionary<NuGetFramework, Assembly> currentlyAvailable = new Dictionary<NuGetFramework, Assembly>(found);
            foreach (NuGetFramework desired in _specification.TargetFrameworks)
            {
                if (!currentlyAvailable.TryGetValue(desired, out Assembly? assembly))
                {
                    changes.Add(desired, ChangeKind.Minor, SR.Format(Changes.TargetFrameworkAddedFormat, desired));
                }
                else
                {
                    currentlyAvailable.Remove(desired);
                    if (!string.Equals(assembly.FullName, _specification.AssemblyName, StringComparison.Ordinal))
                        changes.Add(desired, ChangeKind.Major, SR.Format(Changes.AssemblyRenameFormat, assembly.FullName, _specification.AssemblyName, desired));
                    else
                        yield return (desired, assembly);
                }
            }

            // Check for any remaining frameworks
            foreach (NuGetFramework deleted in currentlyAvailable.Keys)
                changes.Add(deleted, ChangeKind.Major, SR.Format(Changes.TargetFrameworkRemovedFormat, deleted));
        }

        private async Task<Compilation?> CompileAsync(NuGetFramework framework, CancellationToken cancellationToken = default)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>
            {
                { "TargetFramework", framework.DotNetFrameworkName }
            };

            using MSBuildWorkspace workspace = MSBuildWorkspace.Create(properties);
            Project project = await workspace.OpenProjectAsync(
                _specification.ProjectPath,
                progress: null,
                cancellationToken: cancellationToken).ConfigureAwait(false);
            Compilation? compilation = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);
            return compilation;
        }

        [SuppressMessage("MicrosoftCodeAnalysisCorrectness", "RS1024:Compare symbols correctly", Justification = "Analyzer is incorrectly flagged signature comparison.")]
        private static void CompareAssemblies(NuGetFramework framework, Assembly before, Compilation after, ChangeLogger changes)
        {
            Dictionary<TypeSignature, Type> beforeTypes = before.GetExportedTypes().ToDictionary(x => new TypeSignature(x));
            Dictionary<TypeSignature, INamedTypeSymbol> afterTypes = after.GetExportedTypes().ToDictionary(x => new TypeSignature(x));

            // Check for any differences and removed types
            foreach ((TypeSignature signature, Type beforeType) in beforeTypes)
            {
                beforeTypes.Remove(signature);
                if (!afterTypes.TryGetValue(signature, out INamedTypeSymbol? afterType))
                {
                    changes.Add(framework,ChangeKind.Major, SR.Format(Changes.TypeRemovedFormat, signature));
                }
                else
                {
                    afterTypes.Remove(signature);
                    changes.AddIfChanged(framework, signature, beforeType, afterType);
                }
            }

            // Check for any added types
            foreach (TypeSignature signature in afterTypes.Keys)
                changes.Add(framework, ChangeKind.Major, SR.Format(Changes.TypeAddedFormat, signature));
        }

        private sealed class PackageMetadata
        {
            public PackageIdentity Identity { get; }

            public IReadOnlyDictionary<NuGetFramework, Assembly> Lib { get; }

            public PackageMetadata(PackageIdentity identity, IReadOnlyDictionary<NuGetFramework, Assembly> lib)
            {
                Identity = identity ?? throw new ArgumentNullException(nameof(identity));
                Lib = lib ?? throw new ArgumentNullException(nameof(lib));
            }
        }
    }
}
