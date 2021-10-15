// Copyright © William Sugarman.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using NuGet.Common;
using MessageImportance = Microsoft.Build.Framework.MessageImportance;

namespace SemVer.NuGet.Logging
{
    // The below logger is copied from the NuGet.exe as an adapter between MSBuild and NuGet's logging frameworks

    internal class MSBuildLogger : LoggerBase, ILogger
    {
        private delegate void LogMessageWithDetails(string subcategory, string? code, string? helpKeyword, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, MessageImportance importance, string message, params object[] messageArgs);

        private delegate void LogErrorWithDetails(string subcategory, string? code, string? helpKeyword, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message, params object[] messageArgs);

        private delegate void LogMessageAsString(MessageImportance importance, string message, params object[] messageArgs);

        private delegate void LogErrorAsString(string message, params object[] messageArgs);

        private readonly Microsoft.Build.Utilities.TaskLoggingHelper _taskLogging;

        public MSBuildLogger(Microsoft.Build.Utilities.TaskLoggingHelper taskLogging)
            => _taskLogging = taskLogging ?? throw new ArgumentNullException(nameof(taskLogging));

        public override void Log(ILogMessage message)
        {
            if (!DisplayMessage(message.Level))
                return;

            if (RuntimeEnvironmentHelper.IsMono)
            {
                LogForMono(message);
            }
            else
            {
                if (message is not INuGetLogMessage nuGetLogMessage)
                {
                    nuGetLogMessage = new RestoreLogMessage(message.Level, message.Message)
                    {
                        Code = message.Code,
                        FilePath = message.ProjectPath
                    };
                }

                LogForNonMono(nuGetLogMessage);
            }
        }

        private void LogForNonMono(INuGetLogMessage message)
        {
            switch (message.Level)
            {
                case LogLevel.Error:
                    LogError(message, _taskLogging.LogError, _taskLogging.LogError);
                    break;
                case LogLevel.Warning:
                    LogError(message, _taskLogging.LogWarning, _taskLogging.LogWarning);
                    break;
                case LogLevel.Minimal:
                    LogMessage(message, MessageImportance.High, _taskLogging.LogMessage, _taskLogging.LogMessage);
                    break;
                case LogLevel.Information:
                    LogMessage(message, MessageImportance.Normal, _taskLogging.LogMessage, _taskLogging.LogMessage);
                    break;
                default:
                    LogMessage(message, MessageImportance.Low, _taskLogging.LogMessage, _taskLogging.LogMessage);
                    break;
            }
        }

        private void LogForMono(ILogMessage message)
        {
            switch (message.Level)
            {
                case LogLevel.Error:
                    _taskLogging.LogError(message.Message);
                    break;
                case LogLevel.Warning:
                    _taskLogging.LogWarning(message.Message);
                    break;
                case LogLevel.Minimal:
                    _taskLogging.LogMessage(MessageImportance.High, message.Message);
                    break;
                case LogLevel.Information:
                    _taskLogging.LogMessage(MessageImportance.Normal, message.Message);
                    break;
                default:
                    _taskLogging.LogMessage(MessageImportance.Low, message.Message);
                    break;
            }
        }

        private static void LogMessage(INuGetLogMessage logMessage, MessageImportance importance, LogMessageWithDetails logWithDetails, LogMessageAsString logAsString)
        {
            if (logMessage.Code > NuGetLogCode.Undefined)
            {
                logWithDetails(string.Empty, Enum.GetName(typeof(NuGetLogCode), logMessage.Code), Enum.GetName(typeof(NuGetLogCode), logMessage.Code), logMessage.FilePath, logMessage.StartLineNumber, logMessage.StartColumnNumber, logMessage.EndLineNumber, logMessage.EndColumnNumber, importance, logMessage.Message);
            }
            else
            {
                logAsString(importance, logMessage.Message);
            }
        }

        private static void LogError(INuGetLogMessage logMessage, LogErrorWithDetails logWithDetails, LogErrorAsString logAsString)
        {
            if (logMessage.Code > NuGetLogCode.Undefined)
            {
                logWithDetails(string.Empty, Enum.GetName(typeof(NuGetLogCode), logMessage.Code), Enum.GetName(typeof(NuGetLogCode), logMessage.Code), logMessage.FilePath, logMessage.StartLineNumber, logMessage.StartColumnNumber, logMessage.EndLineNumber, logMessage.EndColumnNumber, logMessage.Message);
            }
            else
            {
                logAsString(logMessage.Message);
            }
        }

        public override Task LogAsync(ILogMessage message)
        {
            Log(message);
            return Task.CompletedTask;
        }
    }
}
