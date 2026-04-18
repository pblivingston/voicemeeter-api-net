// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;
using Microsoft.Extensions.Logging;
using PBLivingston.VoicemeeterAPI.EventManagement.Exceptions;
using PBLivingston.VoicemeeterAPI.EventManagement.Logging;
using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI.EventManagement;

internal static partial class RemoteDispatch
{
    public static void GetParam_Start(ILogger logger, string vmParam)
    {
        RemoteLog.GetParam_Start(logger, vmParam);
    }

    public static void GetParam_Success<T>(ILogger logger, string vmParam, T value)
        where T : notnull
    {
        RemoteLog.GetParam_Success(logger, vmParam, value.ToString());
    }

    public static void GetParam_Error<T>(ILogger logger, Response response, string vmParam, T value, Type expectedType)
        where T : notnull
    {
        RemoteLog.GetParam_Error(logger, response.ToString(), vmParam, value.ToString(), expectedType.Name);

        throw new GetParamException<T>(response, vmParam, value, expectedType);
    }
}