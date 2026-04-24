// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using PBLivingston.VoicemeeterAPI.EventManagement.Exceptions;
using PBLivingston.VoicemeeterAPI.EventManagement.Logging;
using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI.EventManagement;

internal static partial class RemoteDispatch
{
    #region Methods

    public static void Method_Success(ILogger logger, [CallerMemberName] string methodName = "", LogLevel level = LogLevel.Information)
    {
        RemoteLog.Method_Success(logger, level, methodName);
    }

    public static RemoteException<T> Method_Error<T>(ILogger logger, T response, [CallerMemberName] string methodName = "")
        where T : unmanaged
    {
        RemoteLog.Method_Error(logger, methodName, response.ToString());

        return new RemoteException<T>(response);
    }

    public static void YieldForSettle(ILogger logger)
    {
        RemoteLog.YieldForSettle(logger);
    }

    #endregion

    #region Query

    public static void Query_Start(ILogger logger, LogLevel level, [CallerMemberName] string methodName = "")
    {
        RemoteLog.Query_Start(logger, level, methodName);
    }

    public static void Query_Success<T>(ILogger logger, LogLevel level, T result, [CallerMemberName] string methodName = "")
        where T : unmanaged
    {
        RemoteLog.Query_Success(logger, level, methodName, result.ToString());
    }

    public static void Dirty_Success(ILogger logger, LogLevel level, Remote sender, EventHandler? eventTrigger, string methodName)
    {
        RemoteLog.Query_Success(logger, level, methodName, Response.Dirty.ToString());

        eventTrigger?.Invoke(sender, null);
    }

    #endregion

    #region Connection State

    public static void ConnectionState_Changed(ILogger logger, Remote sender, EventHandler<ConnectionStateEventArgs>? eventTrigger, ConnectionStateEventArgs lastState, ConnectionStateEventArgs currentState, string methodName)
    {
        RemoteLog.ConnectionState_Changed(logger, methodName, lastState.MemberString, currentState.MemberString);

        eventTrigger?.Invoke(sender, currentState);
    }

    public static KindMismatchException ConnectionState_KindMismatch(ILogger logger, Kind returnedKind, VmVersion returnedVersion)
    {
        RemoteLog.ConnectionState_KindMismatch(logger, returnedKind.ToString(), returnedVersion.ToString());

        return new KindMismatchException(returnedKind, returnedVersion);
    }

    public static void ConnectionState_StateMismatch<T>(ILogger logger, LogLevel level, string memberName, T lastValue)
        where T : unmanaged
    {
        RemoteLog.ConnectionState_StateMismatch(logger, level, memberName, lastValue.ToString());
    }

    #endregion

    #region Dispose

    public static void Dispose_Start(ILogger logger)
    {
        RemoteLog.Dispose_Start(logger);
    }

    public static void Dispose_LoggedIn(ILogger logger, LoginResponse loginStatus)
    {
        RemoteLog.Dispose_LoggedIn(logger, loginStatus.ToString());
    }

    public static void Dispose_AlreadyDisposed(ILogger logger)
    {
        RemoteLog.Dispose_AlreadyDisposed(logger);
    }

    public static void Dispose_Success(ILogger logger)
    {
        RemoteLog.Dispose_Success(logger);
    }

    #endregion

    #region Guard

    public static ObjectDisposedException Guard_ObjectDisposed(ILogger logger)
    {
        RemoteLog.Guard_ObjectDisposed(logger);

        return new ObjectDisposedException(nameof(Remote));
    }

    public static AccessDeniedException Guard_AccessDenied(ILogger logger, LoginResponse loginStatus)
    {
        RemoteLog.Guard_AccessDenied(logger, loginStatus.ToString());

        return new AccessDeniedException(loginStatus);
    }

    #endregion

    #region Retry

    public static void Retry_Start(ILogger logger)
    {
        RemoteLog.Retry_Start(logger);
    }

    public static void Retry_Attempt(ILogger logger, int attempt)
    {
        RemoteLog.Retry_Attempt(logger, attempt);
    }

    public static void Retry_Success(ILogger logger, int attempt)
    {
        RemoteLog.Retry_Success(logger, attempt);
    }

    public static void Retry_Timeout(ILogger logger, int attempts)
    {
        RemoteLog.Retry_Timeout(logger, attempts);
    }

    #endregion
}