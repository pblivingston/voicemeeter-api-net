// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Logging;
using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI;

partial class Remote
{
    #region Methods

    private void On_Method_Success(LogLevel level = LogLevel.Information, [CallerMemberName] string methodName = "")
    {
        RemoteLog.Method_Success(_logger, level, methodName);
    }

    private RemoteException<Response> On_Method_Error(Response response, [CallerMemberName] string methodName = "")
    {
        RemoteLog.Method_Error_Response(_logger, methodName, response);

        return new RemoteException<Response>(response);
    }

    private RemoteException<LoginResponse> On_Method_Error(LoginResponse response, [CallerMemberName] string methodName = "")
    {
        RemoteLog.Method_Error_LoginResponse(_logger, methodName, response);

        return new RemoteException<LoginResponse>(response);
    }

    private RemoteException<RunResponse> On_Method_Error(RunResponse response, [CallerMemberName] string methodName = "")
    {
        RemoteLog.Method_Error_RunResponse(_logger, methodName, response);

        return new RemoteException<RunResponse>(response);
    }

    private void On_Method_YieldForSettle([CallerMemberName] string methodName = "")
    {
        RemoteLog.Method_YieldForSettle(_logger, methodName);
    }

    #endregion

    #region Query

    private void On_Query_Start(LogLevel level = LogLevel.Trace, [CallerMemberName] string methodName = "")
    {
        RemoteLog.Query_Start(_logger, level, methodName);
    }

    private void On_Query_Success(Response result, LogLevel level = LogLevel.Trace, [CallerMemberName] string methodName = "")
    {
        RemoteLog.Query_Success_Response(_logger, level, methodName, result);
    }

    private void On_Query_Success(RunResponse result, LogLevel level = LogLevel.Trace, [CallerMemberName] string methodName = "")
    {
        RemoteLog.Query_Success_RunResponse(_logger, level, methodName, result);
    }

    #endregion

    #region Connection State

    private void On_ConnectionState_Changed(ConnectionStateEventArgs currentState, [CallerMemberName] string methodName = "")
    {
        RemoteLog.ConnectionState_Changed(_logger, methodName, _lastState.MemberString, currentState.MemberString);

        ConnectionStateChanged?.Invoke(this, currentState);

        _lastState = currentState;
    }

    private KindMismatchException On_ConnectionState_KindMismatch(Kind returnedKind, VmVersion returnedVersion)
    {
        RemoteLog.ConnectionState_KindMismatch(_logger, returnedKind, returnedVersion);

        return new KindMismatchException(returnedKind, returnedVersion);
    }

    private void On_ConnectionState_StateMismatch(LoginResponse currentLoginStatus, LogLevel level = LogLevel.Warning)
    {
        RemoteLog.ConnectionState_StateMismatch_LoginStatus(_logger, level, currentLoginStatus, _lastState.LoginStatus);
    }

    private void On_ConnectionState_StateMismatch(bool currentMacroButtonsIsRunning, LogLevel level = LogLevel.Warning)
    {
        RemoteLog.ConnectionState_StateMismatch_MacroButtonsIsRunning(_logger, level, currentMacroButtonsIsRunning, _lastState.MacroButtonsIsRunning);
    }

    private void On_ConnectionState_StateMismatch(Kind currentRunningKind, LogLevel level = LogLevel.Warning)
    {
        RemoteLog.ConnectionState_StateMismatch_RunningKind(_logger, level, currentRunningKind, _lastState.RunningKind);
    }

    private void On_ConnectionState_StateMismatch(VmVersion currentRunningVersion, LogLevel level = LogLevel.Warning)
    {
        RemoteLog.ConnectionState_StateMismatch_RunningVersion(_logger, level, currentRunningVersion, _lastState.RunningVersion);
    }

    #endregion

    #region Dispose

    private void On_Dispose_Start()
    {
        RemoteLog.Dispose_Start(_logger);
    }

    private void On_Dispose_LoggedIn(LoginResponse loginStatus)
    {
        RemoteLog.Dispose_LoggedIn(_logger, loginStatus);
    }

    private void On_Dispose_AlreadyDisposed()
    {
        RemoteLog.Dispose_AlreadyDisposed(_logger);
    }

    private void On_Dispose_Success()
    {
        RemoteLog.Dispose_Success(_logger);
    }

    #endregion

    #region Guard

    private ObjectDisposedException On_Guard_ObjectDisposed()
    {
        RemoteLog.Guard_ObjectDisposed(_logger);

        return new ObjectDisposedException(nameof(Remote));
    }

    private AccessDeniedException On_Guard_AccessDenied(LoginResponse loginStatus)
    {
        RemoteLog.Guard_AccessDenied(_logger, loginStatus);

        return new AccessDeniedException(loginStatus);
    }

    #endregion

    #region Retry

    private void On_Retry_Start()
    {
        RemoteLog.Retry_Start(_logger);
    }

    private void On_Retry_Attempt(int attempt)
    {
        RemoteLog.Retry_Attempt(_logger, attempt);
    }

    private void On_Retry_Success(int attempt)
    {
        RemoteLog.Retry_Success(_logger, attempt);
    }

    private void On_Retry_Timeout(int attempts)
    {
        RemoteLog.Retry_Timeout(_logger, attempts);
    }

    #endregion
}