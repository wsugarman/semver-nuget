// Copyright © William Sugarman.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using NuGet.Configuration;
using NuGet.Protocol.Core.Types;
using SemVer.NuGet.Logging;

namespace SemVer.NuGet.MSBuild
{
    /// <summary>
    /// Represents the MSBuild <see cref="ITask"/> for automatically determining the version of a NuGet package.
    /// </summary>
    public sealed partial class VersionDetectionTask : Microsoft.Build.Utilities.Task, ICancelableTask, IDisposable
    {
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        /// <inheritdoc />
        public void Cancel()
            => _tokenSource.Cancel();

        /// <inheritdoc />
        public void Dispose()
            => _tokenSource.Dispose();

        /// <inheritdoc />
        public override bool Execute()
        {
            try
            {
                // Unfortunately, some of the APIs we need to invoke for NuGet are asynchronous, and
                // as such we'll need to invoke them from a synchronous context.
                // This is not ideal, but it should work.
                return ExecuteAsync().Result;
            }
            catch (OperationCanceledException oce) when (oce.CancellationToken == _tokenSource.Token)
            {
                return false;
            }
            catch (AggregateException ae) when (ae.InnerException is not null)
            {
                ExceptionDispatchInfo.Capture(ae.InnerException).Throw();
                return false;
            }
        }

        private async Task<bool> ExecuteAsync()
        {
            // Create NuGetClient
            ISettings settings = Settings.LoadDefaultSettings(Path.GetDirectoryName(ProjectPath));
            using SourceCacheContext sourceCacheContext = new SourceCacheContext();
            NuGetClient client = new NuGetClient(settings, sourceCacheContext, new MSBuildLogger(Log));

            // Create the diff
            NuGetPackageDiff diff = new NuGetPackageDiff(client, GetSpecification());
            await diff.GetChangesAsync(_tokenSource.Token).ConfigureAwait(false);
            return true;
        }
    }
}
