// Copyright © William Sugarman.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
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
            IReadOnlyDictionary<NuGetFramework, Assembly>? latest = await GetLastPublishedAsync(cancellationToken).ConfigureAwait(false);
            if (latest is null)
                return ChangeSummary.New(); // We have not yet published this package

            ChangeSummaryBuilder changes = new ChangeSummaryBuilder();

            // Compare each framework
            // (Resolve all of the frameworks to front-load the framework differences in the change list)
            MSBuildLocator.RegisterDefaults();
            List<(NuGetFramework, Assembly)> comparisons = CompareFrameworks(latest, changes).ToList();
            foreach ((NuGetFramework framework, Assembly before) in comparisons)
            {
                Compilation? after = await CompileAsync(framework, cancellationToken).ConfigureAwait(false);
                if (after is null)
                    throw new InvalidOperationException(SR.Format(SR.CompilationFailedFormat, framework));

                CompareAssemblies(framework, before, after, changes);
            }

            return changes.Finalize();
        }

        private async Task<IReadOnlyDictionary<NuGetFramework, Assembly>?> GetLastPublishedAsync(CancellationToken cancellationToken = default)
        {
            NuGetVersion? version = await _client.GetLatestVersionAsync(
                _specification.PackageId,
                _specification.IncludePrerelease,
                includeUnlisted: true,
                cancellationToken).ConfigureAwait(false);

            if (version is null)
                return null;

            return await _client.GetLibAssembliesAsync(
                new PackageIdentity(_specification.PackageId, version),
                cancellationToken).ConfigureAwait(false);
        }

        private IEnumerable<(NuGetFramework, Assembly)> CompareFrameworks(IReadOnlyDictionary<NuGetFramework, Assembly> found, ChangeSummaryBuilder changes)
        {
            Dictionary<NuGetFramework, Assembly> currentlyAvailable = new Dictionary<NuGetFramework, Assembly>(found);
            foreach (NuGetFramework desired in _specification.TargetFrameworks)
            {
                if (!currentlyAvailable.TryGetValue(desired, out Assembly? assembly))
                {
                    changes.Add(ChangeKind.Minor, SR.Format(Changes.TargetFrameworkAddedFormat, desired));
                }
                else
                {
                    currentlyAvailable.Remove(desired);
                    if (!string.Equals(assembly.FullName, _specification.AssemblyName, StringComparison.Ordinal))
                        changes.Add(ChangeKind.Major, SR.Format(Changes.AssemblyRenameFormat, assembly.FullName, _specification.AssemblyName, desired));
                    else
                        yield return (desired, assembly);
                }
            }

            // Check for any remaining frameworks
            foreach (NuGetFramework deleted in currentlyAvailable.Keys)
                changes.Add(ChangeKind.Major, SR.Format(Changes.TargetFrameworkRemovedFormat, deleted));
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

        private static void CompareAssemblies(NuGetFramework framework, Assembly before, Compilation after, ChangeSummaryBuilder changes)
        {
            // TODO ...
        }
    }
}
