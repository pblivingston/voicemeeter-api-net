// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using Microsoft.Extensions.Logging;
using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI.Logging;

internal static partial class RemoteLog
{
    #region Methods

    [LoggerMessage(
        EventId = (int)Event.Method_Success,
        EventName = nameof(Event.Method_Success),
        Message = "{MethodName} successful."
    )]
    public static partial void Method_Success(ILogger logger, LogLevel level, string methodName);

    [LoggerMessage(
        EventId = (int)Event.Method_Error_Response,
        EventName = nameof(Event.Method_Error_Response),
        Level = LogLevel.Error,
        Message = "{MethodName} failed. Response: {Response}"
    )]
    public static partial void Method_Error_Response(ILogger logger, string methodName, Response response);

    [LoggerMessage(
        EventId = (int)Event.Method_Error_LoginResponse,
        EventName = nameof(Event.Method_Error_LoginResponse),
        Level = LogLevel.Error,
        Message = "{MethodName} failed. Response: {Response}"
    )]
    public static partial void Method_Error_LoginResponse(ILogger logger, string methodName, LoginResponse response);

    [LoggerMessage(
        EventId = (int)Event.Method_Error_RunResponse,
        EventName = nameof(Event.Method_Error_RunResponse),
        Level = LogLevel.Error,
        Message = "{MethodName} failed. Response: {Response}"
    )]
    public static partial void Method_Error_RunResponse(ILogger logger, string methodName, RunResponse response);

    [LoggerMessage(
        EventId = (int)Event.Method_YieldForSettle,
        EventName = nameof(Event.Method_YieldForSettle),
        Level = LogLevel.Debug,
        Message = "{MethodName} is waiting for engine to settle..."
    )]
    public static partial void Method_YieldForSettle(ILogger logger, string methodName);

    #endregion

    #region Query

    [LoggerMessage(
        EventId = (int)Event.Query_Start,
        EventName = nameof(Event.Query_Start),
        Message = "Checking {MethodName}..."
    )]
    public static partial void Query_Start(ILogger logger, LogLevel level, string methodName);

    [LoggerMessage(
        EventId = (int)Event.Query_Success_Response,
        EventName = nameof(Event.Query_Success_Response),
        Message = "{MethodName} successful. Response: {Response}"
    )]
    public static partial void Query_Success_Response(ILogger logger, LogLevel level, string methodName, Response response);

    [LoggerMessage(
        EventId = (int)Event.Query_Success_RunResponse,
        EventName = nameof(Event.Query_Success_RunResponse),
        Message = "{MethodName} successful. Response: {Response}"
    )]
    public static partial void Query_Success_RunResponse(ILogger logger, LogLevel level, string methodName, RunResponse response);

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
    public static partial void ConnectionState_KindMismatch(ILogger logger, Kind returnedKind, VmVersion returnedVersion);

    [LoggerMessage(
        EventId = (int)Event.ConnectionState_StateMismatch_LoginStatus,
        EventName = nameof(Event.ConnectionState_StateMismatch_LoginStatus),
        Message = "LoginStatus did not match last Connection State. Recommend calling GetConnectionState. Current LoginStatus: {CurrentLoginStatus}; Last LoginStatus: {LastLoginStatus}"
    )]
    public static partial void ConnectionState_StateMismatch_LoginStatus(ILogger logger, LogLevel level, LoginResponse currentLoginStatus, LoginResponse lastLoginStatus);

    [LoggerMessage(
        EventId = (int)Event.ConnectionState_StateMismatch_MacroButtonsIsRunning,
        EventName = nameof(Event.ConnectionState_StateMismatch_MacroButtonsIsRunning),
        Message = "MacroButtonsIsRunning did not match last Connection State. Recommend calling GetConnectionState. Current MacroButtonsIsRunning: {CurrentMacroButtonsIsRunning}; Last MacroButtonsIsRunning: {LastMacroButtonsIsRunning}"
    )]
    public static partial void ConnectionState_StateMismatch_MacroButtonsIsRunning(ILogger logger, LogLevel level, bool currentMacroButtonsIsRunning, bool lastMacroButtonsIsRunning);

    [LoggerMessage(
        EventId = (int)Event.ConnectionState_StateMismatch_RunningKind,
        EventName = nameof(Event.ConnectionState_StateMismatch_RunningKind),
        Message = "RunningKind did not match last Connection State. Recommend calling GetConnectionState. Current RunningKind: {CurrentRunningKind}; Last RunningKind: {LastRunningKind}"
    )]
    public static partial void ConnectionState_StateMismatch_RunningKind(ILogger logger, LogLevel level, Kind currentRunningKind, Kind lastRunningKind);

    [LoggerMessage(
        EventId = (int)Event.ConnectionState_StateMismatch_RunningVersion,
        EventName = nameof(Event.ConnectionState_StateMismatch_RunningVersion),
        Message = "RunningVersion did not match last Connection State. Recommend calling GetConnectionState. Current RunningVersion: {CurrentRunningVersion}; Last RunningVersion: {LastRunningVersion}"
    )]
    public static partial void ConnectionState_StateMismatch_RunningVersion(ILogger logger, LogLevel level, VmVersion currentRunningVersion, VmVersion lastRunningVersion);

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
    public static partial void Dispose_LoggedIn(ILogger logger, LoginResponse loginStatus);

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
    public static partial void Guard_AccessDenied(ILogger logger, LoginResponse loginStatus);

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