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
    public static void GetInfo_Start(ILogger logger, LogLevel level, Type infoType)
    {
        RemoteLog.GetInfo_Start(logger, level, infoType.Name);
    }

    public static void GetInfo_Success<T>(ILogger logger, LogLevel level, T value, [CallerMemberName] string methodName = "")
        where T : unmanaged
    {
        RemoteLog.GetInfo_Success(logger, level, methodName, typeof(T).Name, value.ToString());
    }

    public static GetInfoException GetInfo_Error(ILogger logger, InfoResponse response, int value, [CallerMemberName] string methodName = "")
    {
        RemoteLog.GetInfo_Error(logger, methodName, response.ToString(), value);

        return new GetInfoException(response, value);
    }

    public static void GetInfo_StateMismatch<T>(ILogger logger, LogLevel level, T lastValue)
        where T : unmanaged
    {
        RemoteLog.GetInfo_StateMismatch(logger, level, typeof(T).Name, lastValue.ToString());
    }
}