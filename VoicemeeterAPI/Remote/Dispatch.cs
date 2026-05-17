// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Logging;
using PBLivingston.VoicemeeterAPI.Types;

public partial class Remote
{
    #region Scope

    private IDisposable? BeginInstanceScope()
        => LogScope.Instance(this.logger, this.instanceId);

    private IDisposable? BeginMethodScope([CallerMemberName] string methodName = "")
        => LogScope.Method(this.logger, methodName);

    #endregion

    #region Methods

    private void On_Method_Success(LogLevel level = LogLevel.Information, [CallerMemberName] string methodName = "")
        => RemoteLog.Method_Success(this.logger, level, methodName);

    private RemoteException<Response> On_Method_Error(Response response, [CallerMemberName] string methodName = "")
    {
        RemoteLog.Method_Error_Response(this.logger, methodName, response);

        return new RemoteException<Response>(response);
    }

    private RemoteException<LoginResponse> On_Method_Error(LoginResponse response, [CallerMemberName] string methodName = "")
    {
        RemoteLog.Method_Error_LoginResponse(this.logger, methodName, response);

        return new RemoteException<LoginResponse>(response);
    }

    private RemoteException<RunResponse> On_Method_Error(RunResponse response, [CallerMemberName] string methodName = "")
    {
        RemoteLog.Method_Error_RunResponse(this.logger, methodName, response);

        return new RemoteException<RunResponse>(response);
    }

    private void On_Method_YieldForSettle([CallerMemberName] string methodName = "")
        => RemoteLog.Method_YieldForSettle(this.logger, methodName);

    #endregion

    #region Query

    private void On_Query_Start(LogLevel level = LogLevel.Trace, [CallerMemberName] string methodName = "")
        => RemoteLog.Query_Start(this.logger, level, methodName);

    private void On_Query_Success(Response result, LogLevel level = LogLevel.Trace, [CallerMemberName] string methodName = "")
        => RemoteLog.Query_Success_Response(this.logger, level, methodName, result);

    private void On_Query_Success(RunResponse result, LogLevel level = LogLevel.Trace, [CallerMemberName] string methodName = "")
        => RemoteLog.Query_Success_RunResponse(this.logger, level, methodName, result);

    #endregion

    #region Connection State

    private void On_ConnectionState_Changed(ConnectionStateEventArgs currentState, [CallerMemberName] string methodName = "")
    {
        RemoteLog.ConnectionState_Changed(this.logger, methodName, this.lastState.MemberString, currentState.MemberString);

        ConnectionStateChanged?.Invoke(this, currentState);

        this.lastState = currentState;
    }

    private KindMismatchException On_ConnectionState_KindMismatch(Kind returnedKind, VmVersion returnedVersion)
    {
        RemoteLog.ConnectionState_KindMismatch(this.logger, returnedKind, returnedVersion);

        return new KindMismatchException(returnedKind, returnedVersion);
    }

    private void On_ConnectionState_StateMismatch(LoginResponse currentLoginStatus, LogLevel level = LogLevel.Warning)
        => RemoteLog.ConnectionState_StateMismatch_LoginStatus(this.logger, level, currentLoginStatus, this.lastState.LoginStatus);

    private void On_ConnectionState_StateMismatch(bool currentMacroButtonsIsRunning, LogLevel level = LogLevel.Warning)
        => RemoteLog.ConnectionState_StateMismatch_MacroButtonsIsRunning(this.logger, level, currentMacroButtonsIsRunning, this.lastState.MacroButtonsIsRunning);

    private void On_ConnectionState_StateMismatch(Kind currentRunningKind, LogLevel level = LogLevel.Warning)
        => RemoteLog.ConnectionState_StateMismatch_RunningKind(this.logger, level, currentRunningKind, this.lastState.RunningKind);

    private void On_ConnectionState_StateMismatch(VmVersion currentRunningVersion, LogLevel level = LogLevel.Warning)
        => RemoteLog.ConnectionState_StateMismatch_RunningVersion(this.logger, level, currentRunningVersion, this.lastState.RunningVersion);

    #endregion

    #region Dispose

    private void On_Dispose_Start()
        => RemoteLog.Dispose_Start(this.logger);

    private void On_Dispose_LoggedIn(LoginResponse loginStatus)
        => RemoteLog.Dispose_LoggedIn(this.logger, loginStatus);

    private void On_Dispose_AlreadyDisposed()
        => RemoteLog.Dispose_AlreadyDisposed(this.logger);

    private void On_Dispose_Success()
        => RemoteLog.Dispose_Success(this.logger);

    #endregion

    #region Guard

    private ObjectDisposedException On_Guard_ObjectDisposed()
    {
        RemoteLog.Guard_ObjectDisposed(this.logger);

        return new ObjectDisposedException(nameof(Remote));
    }

    private AccessDeniedException On_Guard_AccessDenied(LoginResponse loginStatus)
    {
        RemoteLog.Guard_AccessDenied(this.logger, loginStatus);

        return new AccessDeniedException(loginStatus);
    }

    #endregion

    #region Retry

    private void On_Retry_Start()
        => RemoteLog.Retry_Start(this.logger);

    private void On_Retry_Attempt(int attempt)
        => RemoteLog.Retry_Attempt(this.logger, attempt);

    private void On_Retry_Success(int attempt)
        => RemoteLog.Retry_Success(this.logger, attempt);

    private void On_Retry_Timeout(int attempts)
        => RemoteLog.Retry_Timeout(this.logger, attempts);

    #endregion
}
