// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using PBLivingston.VoicemeeterAPI.EventManagement.Exceptions;
using PBLivingston.VoicemeeterAPI.EventManagement.Logging;
using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI.EventManagement;

internal static partial class RemoteDispatch
{
    public static void GetInfo_Start(ILogger logger, Type infoType)
    {
        RemoteLog.GetInfo_Start(logger, infoType.Name);
    }

    public static void GetInfo_Success<T>(ILogger logger, T value, [CallerMemberName] string methodName = "")
        where T : unmanaged
    {
        RemoteLog.GetInfo_Success(logger, methodName, typeof(T).Name, value.ToString());
    }

    public static void GetInfo_Error(ILogger logger, InfoResponse response, int value, [CallerMemberName] string methodName = "")
    {
        RemoteLog.GetInfo_Error(logger, methodName, response.ToString(), value);

        throw new GetInfoException(response, value);
    }
}