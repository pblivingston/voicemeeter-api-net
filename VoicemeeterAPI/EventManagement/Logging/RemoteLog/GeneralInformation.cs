// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using Microsoft.Extensions.Logging;

namespace PBLivingston.VoicemeeterAPI.EventManagement.Logging;

internal static partial class RemoteLog
{
    [LoggerMessage(
        EventId = (int)Event.GetInfo_Start,
        EventName = nameof(Event.GetInfo_Start),
        Level = LogLevel.Trace,
        Message = "Getting running {InfoType}..."
    )]
    public static partial void GetInfo_Start(ILogger logger, string infoType);

    [LoggerMessage(
        EventId = (int)Event.GetInfo_Success,
        EventName = nameof(Event.GetInfo_Success),
        Level = LogLevel.Trace,
        Message = "{MethodName} successful. Returned {InfoType}: {ValueString}"
    )]
    public static partial void GetInfo_Success(ILogger logger, string methodName, string infoType, string valueString);

    [LoggerMessage(
        EventId = (int)Event.GetInfo_Error,
        EventName = nameof(Event.GetInfo_Error),
        Level = LogLevel.Error,
        Message = "{MethodName} failed. Response: {Response}; Returned value: {Value}"
    )]
    public static partial void GetInfo_Error(ILogger logger, string methodName, string response, int value);
}