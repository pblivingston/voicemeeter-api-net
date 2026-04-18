// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using Microsoft.Extensions.Logging;

namespace PBLivingston.VoicemeeterAPI.EventManagement.Logging;

internal static partial class RemoteLog
{
    [LoggerMessage(
        EventId = (int)Event.GetParam_Start,
        EventName = nameof(Event.GetParam_Start),
        Level = LogLevel.Trace,
        Message = "Getting Voicemeeter parameter '{VmParam}'..."
    )]
    public static partial void GetParam_Start(ILogger logger, string vmParam);

    [LoggerMessage(
        EventId = (int)Event.GetParam_Success,
        EventName = nameof(Event.GetParam_Success),
        Level = LogLevel.Trace,
        Message = "GetParam successful. Requested Voicemeeter parameter: {VmParam}; Returned value: {ValueString}"
    )]
    public static partial void GetParam_Success(ILogger logger, string vmParam, string valueString);

    [LoggerMessage(
        EventId = (int)Event.GetParam_Error,
        EventName = nameof(Event.GetParam_Error),
        Level = LogLevel.Error,
        Message = "GetParam failed. Response: {Response}; Requested Voicemeeter parameter: {VmParam}; Returned value: {ValueString}; Expected type: {ExpectedType}"
    )]
    public static partial void GetParam_Error(ILogger logger, string response, string vmParam, string valueString, string expectedType);
}