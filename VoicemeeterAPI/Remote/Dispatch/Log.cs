// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using Microsoft.Extensions.Logging;

public partial class Remote
{
    private static partial class Log
    {
        public static readonly Func<ILogger, Guid, Guid, IDisposable?> CallScope = LoggerMessage.DefineScope<Guid, Guid>("{Instance}, {Call}");

        #region Entry

        [LoggerMessage(
            EventId = (int)Event.RemoteMethodStart,
            Level = LogLevel.Information,
            Message = "[VoicemeeterAPI.Remote.{MethodName}] " +
                "Starting... " +
                "{Payload}[{MethodName}]"
        )]
        public static partial void RemoteMethodStart(
            ILogger logger,
            string methodName,
            LogArgs payload
        );

        [LoggerMessage(
            EventId = (int)Event.RemoteWrapperCall,
            Message = "[VoicemeeterAPI.Remote.{MethodName}] " +
                "Calling 'this.wrapper.{WrapperMethodName}'... " +
                "{Payload}[{ExecutionPath}]"
        )]
        public static partial void RemoteWrapperCall(
            ILogger logger,
            LogLevel level,
            string methodName,
            string wrapperMethodName,
            LogArgs payload,
            string executionPath
        );

        [LoggerMessage(
            EventId = (int)Event.RemoteInternalLogin,
            Level = LogLevel.Information,
            Message = "[VoicemeeterAPI.Remote.{MethodName}] " +
                "Logging in... " +
                "[{ExecutionPath}]"
        )]
        public static partial void RemoteInternalLogin(
            ILogger logger,
            string methodName,
            string executionPath
        );

        [LoggerMessage(
            EventId = (int)Event.RemoteInternalLogout,
            Level = LogLevel.Information,
            Message = "[VoicemeeterAPI.Remote.{MethodName}] " +
                "Logging out... " +
                "[{ExecutionPath}]"
        )]
        public static partial void RemoteInternalLogout(
            ILogger logger,
            string methodName,
            string executionPath
        );

        [LoggerMessage(
            EventId = (int)Event.WaitForRunningStart,
            Level = LogLevel.Information,
            Message = "[VoicemeeterAPI.Remote.{MethodName}] " +
                "Waiting for {TargetDescription}... " +
                "{Payload}[{ExecutionPath}]"
        )]
        public static partial void WaitForRunningStart(
            ILogger logger,
            string methodName,
            string targetDescription,
            LogArgs payload,
            string executionPath
        );

        [LoggerMessage(
            EventId = (int)Event.YieldForEngineSettle,
            Level = LogLevel.Information,
            Message = "[VoicemeeterAPI.Remote.{MethodName}] " +
                "Allowing {TargetDescription} to settle... " +
                "[{ExecutionPath}]"
        )]
        public static partial void YieldForEngineSettle(
            ILogger logger,
            string methodName,
            string targetDescription,
            string executionPath
        );

        [LoggerMessage(
            EventId = (int)Event.ConnectionStateChanged,
            Level = LogLevel.Information,
            Message = "[VoicemeeterAPI.Remote.{MethodName}] " +
                "Connection state changed. Updated LastConnectionState. " +
                "{Payload}[{ExecutionPath}]"
        )]
        public static partial void ConnectionStateChanged(
            ILogger logger,
            string methodName,
            CacheLogArgs payload,
            string executionPath
        );

        #endregion

        #region Exit

        [LoggerMessage(
            EventId = (int)Event.RemoteMethodSuccess,
            Message = "[VoicemeeterAPI.Remote.{MethodName}] " +
                "Success. " +
                "{Payload}[{ExecutionPath}]"
        )]
        public static partial void RemoteMethodSuccess(
            ILogger logger,
            LogLevel level,
            string methodName,
            LogArgs payload,
            string executionPath
        );

        [LoggerMessage(
            EventId = (int)Event.WaitForRunningDetected,
            Level = LogLevel.Information,
            Message = "[VoicemeeterAPI.Remote.{MethodName}] " +
                "{TargetDescription} detected. " +
                "{Payload}[{ExecutionPath}]"
        )]
        public static partial void WaitForRunningDetected(
            ILogger logger,
            string methodName,
            string targetDescription,
            LogArgs payload,
            string executionPath
        );

        #endregion

        #region Warning

        [LoggerMessage(
            EventId = (int)Event.RemoteNotConnected,
            Level = LogLevel.Warning,
            Message = "[VoicemeeterAPI.Remote.{MethodName}] " +
                "{TargetDescription} is not running. " +
                "{Payload}[{ExecutionPath}]"
        )]
        public static partial void RemoteNotConnected(
            ILogger logger,
            string methodName,
            string targetDescription,
            LogArgs payload,
            string executionPath
        );

        [LoggerMessage(
            EventId = (int)Event.RemoteLostConnection,
            Level = LogLevel.Error,
            Message = "[VoicemeeterAPI.Remote.{MethodName}] " +
                "Lost connection to {TargetDescription}. " +
                "{Payload}[{ExecutionPath}]"
        )]
        public static partial void RemoteLostConnection(
            ILogger logger,
            string methodName,
            string targetDescription,
            LogArgs payload,
            string executionPath
        );

        [LoggerMessage(
            EventId = (int)Event.StaleConnectionState,
            Level = LogLevel.Warning,
            Message = "[VoicemeeterAPI.Remote.{MethodName}] " +
                "Connection state has changed since LastConnectionState was cached. " +
                "Recommend calling GetConnectionState to update. " +
                "{Payload}[{ExecutionPath}]"
        )]
        public static partial void StaleConnectionState(
            ILogger logger,
            string methodName,
            CacheLogArgs payload,
            string executionPath
        );

        [LoggerMessage(
            EventId = (int)Event.RemoteOperationTimeout,
            Level = LogLevel.Warning,
            Message = "[VoicemeeterAPI.Remote.{MethodName}] " +
                "Operation timed out. " +
                "{Payload}[{ExecutionPath}]"
        )]
        public static partial void RemoteOperationTimeout(
            ILogger logger,
            OperationCanceledException ex,
            string methodName,
            LogArgs payload,
            string executionPath
        );

        #endregion

        #region Dev Error

        [LoggerMessage(
            EventId = (int)Event.RemoteContractViolation,
            Level = LogLevel.Error,
            Message = "[VoicemeeterAPI.Remote.{MethodName}] " +
                "API Contract Violation: {Reason} " +
                "{Payload}[{ExecutionPath}]"
        )]
        public static partial void RemoteContractViolation(
            ILogger logger,
            InvalidOperationException ex,
            string methodName,
            string reason,
            LogArgs payload,
            string executionPath
        );

        [LoggerMessage(
            EventId = (int)Event.RemoteInvalidArgument,
            Level = LogLevel.Error,
            Message = "[VoicemeeterAPI.Remote.{MethodName}] " +
                "Invalid argument(s). " +
                "{Payload}[{ExecutionPath}]"
        )]
        public static partial void RemoteInvalidArgument(
            ILogger logger,
            ArgumentException ex,
            string methodName,
            LogArgs payload,
            string executionPath
        );

        [LoggerMessage(
            EventId = (int)Event.RemoteMethodError,
            Level = LogLevel.Error,
            Message = "[VoicemeeterAPI.Remote.{MethodName}] " +
                "Operation could not be completed as requested. " +
                "{Payload}[{ExecutionPath}]"
        )]
        public static partial void RemoteMethodError(
            ILogger logger,
            RemoteException ex,
            string methodName,
            LogArgs payload,
            string executionPath
        );

        #endregion

        #region Env Error

        [LoggerMessage(
            EventId = (int)Event.RemoteLoginFailed,
            Level = LogLevel.Critical,
            Message = "[VoicemeeterAPI.Remote.{MethodName}] " +
                "Unable to open connection to VoicemeeterRemote. " +
                "{Payload}[{ExecutionPath}]"
        )]
        public static partial void RemoteLoginFailed(
            ILogger logger,
            CannotGetClientException ex,
            string methodName,
            LogArgs payload,
            string executionPath
        );

        [LoggerMessage(
            EventId = (int)Event.AppCriticalState,
            Level = LogLevel.Error,
            Message = "[VoicemeeterAPI.Remote.{MethodName}] " +
                "Application needs attention. " +
                "{Payload}[{ExecutionPath}]"
        )]
        public static partial void AppCriticalState(
            ILogger logger,
            AppStateException ex,
            string methodName,
            LogArgs payload,
            string executionPath
        );

        [LoggerMessage(
            EventId = (int)Event.UnhandledLogoutResponse,
            Level = LogLevel.Critical,
            Message = "[VoicemeeterAPI.Remote.{MethodName}] " +
                "CRITICAL: UNHANDLED RESPONSE. " +
                "Please escalate this log to the library maintainer immediately! " +
                "{Payload}[{ExecutionPath}]"
        )]
        public static partial void UnhandledLogoutResponse(
            ILogger logger,
            string methodName,
            LogArgs payload,
            string executionPath
        );

        [LoggerMessage(
            EventId = (int)Event.UnhandledResponse,
            Level = LogLevel.Critical,
            Message = "[VoicemeeterAPI.Remote.{MethodName}] " +
                "CRITICAL: UNHANDLED RESPONSE. " +
                "Please escalate this log to the library maintainer immediately! " +
                "{Payload}[{ExecutionPath}]"
        )]
        public static partial void UnhandledResponse(
            ILogger logger,
            RemoteException ex,
            string methodName,
            LogArgs payload,
            string executionPath
        );

        #endregion
    }
}
