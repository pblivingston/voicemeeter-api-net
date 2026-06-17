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
    {
#if NET7_0_OR_GREATER
        ObjectDisposedException.ThrowIf(this.isDisposed != 0, this);
#else
        if (this.isDisposed != 0)
        {
            throw new ObjectDisposedException(nameof(Remote));
        }
#endif

        return LogScope.Instance(this.logger, this.instanceId);
    }

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

    #endregion

    #region Connection State

    private void On_ConnectionState_Changed(ConnectionState currentState, [CallerMemberName] string methodName = "")
    {
        if (this.lastConnectionState == currentState)
        {
            return;
        }

        RemoteLog.ConnectionState_Changed(this.logger, methodName, this.lastConnectionState, currentState);

        ConnectionStateChanged?.Invoke(this, new(this.lastConnectionState, currentState));

        this.lastConnectionState = currentState;
    }

    private void On_ConnectionState_StateMismatch(LoginResponse currentLoginStatus, LogLevel level = LogLevel.Warning)
    {
        if (this.lastConnectionState.LoginStatus == currentLoginStatus)
        {
            return;
        }

        RemoteLog.ConnectionState_StateMismatch_LoginStatus(this.logger, level, currentLoginStatus, this.lastConnectionState.LoginStatus);
    }

    private void On_ConnectionState_StateMismatch(RunResponse currentButtonsState, LogLevel level = LogLevel.Warning)
    {
        if (this.lastConnectionState.ButtonsState == currentButtonsState)
        {
            return;
        }

        RemoteLog.ConnectionState_StateMismatch_ButtonsState(this.logger, level, currentButtonsState, this.lastConnectionState.ButtonsState);
    }

    private void On_ConnectionState_StateMismatch(Kind currentRunningKind, LogLevel level = LogLevel.Warning)
    {
        if (this.lastConnectionState.RunningKind == currentRunningKind)
        {
            return;
        }

        RemoteLog.ConnectionState_StateMismatch_RunningKind(this.logger, level, currentRunningKind, this.lastConnectionState.RunningKind);
    }

    private void On_ConnectionState_StateMismatch(VmVersion currentRunningVersion, LogLevel level = LogLevel.Warning)
    {
        if (this.lastConnectionState.RunningVersion == currentRunningVersion)
        {
            return;
        }

        RemoteLog.ConnectionState_StateMismatch_RunningVersion(this.logger, level, currentRunningVersion, this.lastConnectionState.RunningVersion);
    }

    #endregion

    #region Get Connection State

    private void On_GetConnectionState_Start()
        => RemoteLog.GetConnectionState_Start(this.logger);

    private KindMismatchException On_GetConnectionState_KindMismatch(Kind returnedKind, VmVersion returnedVersion)
    {
        RemoteLog.GetConnectionState_KindMismatch(this.logger, returnedKind, returnedVersion);

        return new KindMismatchException(returnedKind, returnedVersion);
    }

    #endregion

    #region Dispose

    private void On_Dispose_Start()
        => RemoteLog.Dispose_Start(this.logger);

    private void On_Dispose_LoggedIn(LoginResponse loginStatus)
        => RemoteLog.Dispose_LoggedIn(this.logger, loginStatus);

    private void On_Dispose_Success()
        => RemoteLog.Dispose_Success(this.logger);

    #endregion
}
