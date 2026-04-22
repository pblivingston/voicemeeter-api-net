// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using Microsoft.Extensions.Logging;

namespace PBLivingston.VoicemeeterAPI.EventManagement.Logging;

internal static partial class RemoteLog
{
    #region Methods

    [LoggerMessage(
        EventId = (int)Event.Method_Success,
        EventName = nameof(Event.Method_Success),
        Level = LogLevel.Information,
        Message = "{MethodName} successful."
    )]
    public static partial void Method_Success(ILogger logger, string methodName);

    [LoggerMessage(
        EventId = (int)Event.Method_Error,
        EventName = nameof(Event.Method_Error),
        Level = LogLevel.Error,
        Message = "{MethodName} failed. Response: {Response}"
    )]
    public static partial void Method_Error(ILogger logger, string methodName, string response);

    [LoggerMessage(
        EventId = (int)Event.YieldForSettle,
        EventName = nameof(Event.YieldForSettle),
        Level = LogLevel.Debug,
        Message = "Waiting for engine to settle..."
    )]
    public static partial void YieldForSettle(ILogger logger);

    #endregion

    #region Dirty

    [LoggerMessage(
        EventId = (int)Event.Dirty_Start,
        EventName = nameof(Event.Dirty_Start),
        Message = "Checking {MethodName}..."
    )]
    public static partial void Dirty_Start(ILogger logger, LogLevel level, string methodName);

    [LoggerMessage(
        EventId = (int)Event.Dirty_Success,
        EventName = nameof(Event.Dirty_Success),
        Message = "{MethodName} successful. Response: {Response}"
    )]
    public static partial void Dirty_Success(ILogger logger, LogLevel level, string methodName, string response);

    #endregion

    #region Connection State

    [LoggerMessage(
        EventId = (int)Event.ConnectionState_Changed,
        EventName = nameof(Event.ConnectionState_Changed),
        Level = LogLevel.Information,
        Message = "Connection State changed during {MethodName}. Last state - {LastStateMembers}. Current state - {CurrentStateMembers}."
    )]
    public static partial void ConnectionState_Changed(ILogger logger, string methodName, string lastStateMembers, string currentStateMembers);

    [LoggerMessage(
        EventId = (int)Event.ConnectionState_KindMismatch,
        EventName = nameof(Event.ConnectionState_KindMismatch),
        Level = LogLevel.Critical,
        Message = "Kind and version did not match when getting Connection State. GetKind returned: {ReturnedKind}; GetVersion returned: {ReturnedVersion}"
    )]
    public static partial void ConnectionState_KindMismatch(ILogger logger, string returnedKind, string returnedVersion);

    #endregion

    #region Dispose

    [LoggerMessage(
        EventId = (int)Event.Dispose_Start,
        EventName = nameof(Event.Dispose_Start),
        Level = LogLevel.Debug,
        Message = "Disposing Remote object..."
    )]
    public static partial void Dispose_Start(ILogger logger);

    [LoggerMessage(
        EventId = (int)Event.Dispose_LoggedIn,
        EventName = nameof(Event.Dispose_LoggedIn),
        Level = LogLevel.Debug,
        Message = "Still logged in. LoginStatus: {LoginStatus}"
    )]
    public static partial void Dispose_LoggedIn(ILogger logger, string loginStatus);

    [LoggerMessage(
        EventId = (int)Event.Dispose_AlreadyDisposed,
        EventName = nameof(Event.Dispose_AlreadyDisposed),
        Level = LogLevel.Error,
        Message = "Remote object already disposed."
    )]
    public static partial void Dispose_AlreadyDisposed(ILogger logger);

    [LoggerMessage(
        EventId = (int)Event.Dispose_Success,
        EventName = nameof(Event.Dispose_Success),
        Level = LogLevel.Debug,
        Message = "Remote object disposed."
    )]
    public static partial void Dispose_Success(ILogger logger);

    #endregion

    #region Guard

    [LoggerMessage(
        EventId = (int)Event.Guard_ObjectDisposed,
        EventName = nameof(Event.Guard_ObjectDisposed),
        Level = LogLevel.Critical,
        Message = "No access. Remote object disposed."
    )]
    public static partial void Guard_ObjectDisposed(ILogger logger);

    [LoggerMessage(
        EventId = (int)Event.Guard_AccessDenied,
        EventName = nameof(Event.Guard_AccessDenied),
        Level = LogLevel.Error,
        Message = "Access denied. LoginStatus: {LoginStatus}"
    )]
    public static partial void Guard_AccessDenied(ILogger logger, string loginStatus);

    #endregion

    #region Retry

    [LoggerMessage(
        EventId = (int)Event.Retry_Start,
        EventName = nameof(Event.Retry_Start),
        Level = LogLevel.Debug,
        Message = "Starting retry loop..."
    )]
    public static partial void Retry_Start(ILogger logger);

    [LoggerMessage(
        EventId = (int)Event.Retry_Attempt,
        EventName = nameof(Event.Retry_Attempt),
        Level = LogLevel.Trace,
        Message = "Attempt {Attempt} failed. Retrying..."
    )]
    public static partial void Retry_Attempt(ILogger logger, int attempt);

    [LoggerMessage(
        EventId = (int)Event.Retry_Success,
        EventName = nameof(Event.Retry_Success),
        Level = LogLevel.Debug,
        Message = "Attempt {Attempt} successful."
    )]
    public static partial void Retry_Success(ILogger logger, int attempt);

    [LoggerMessage(
        EventId = (int)Event.Retry_Timeout,
        EventName = nameof(Event.Retry_Timeout),
        Level = LogLevel.Debug,
        Message = "Retry timed out after {Attempts} attempts."
    )]
    public static partial void Retry_Timeout(ILogger logger, int attempts);

    #endregion
}