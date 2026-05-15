// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using Microsoft.Extensions.Logging;
using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI.Logging;

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
        EventId = (int)Event.GetParam_Success_Float,
        EventName = nameof(Event.GetParam_Success_Float),
        Level = LogLevel.Trace,
        Message = "GetParam successful. Requested Voicemeeter parameter: {VmParam}; Returned value: {Value}"
    )]
    public static partial void GetParam_Success_Float(ILogger logger, string vmParam, float value);

    [LoggerMessage(
        EventId = (int)Event.GetParam_Success_Int,
        EventName = nameof(Event.GetParam_Success_Int),
        Level = LogLevel.Trace,
        Message = "GetParam successful. Requested Voicemeeter parameter: {VmParam}; Returned value: {Value}"
    )]
    public static partial void GetParam_Success_Int(ILogger logger, string vmParam, int value);

    [LoggerMessage(
        EventId = (int)Event.GetParam_Success_Bool,
        EventName = nameof(Event.GetParam_Success_Bool),
        Level = LogLevel.Trace,
        Message = "GetParam successful. Requested Voicemeeter parameter: {VmParam}; Returned value: {Value}"
    )]
    public static partial void GetParam_Success_Bool(ILogger logger, string vmParam, bool value);

    [LoggerMessage(
        EventId = (int)Event.GetParam_Success_String,
        EventName = nameof(Event.GetParam_Success_String),
        Level = LogLevel.Trace,
        Message = "GetParam successful. Requested Voicemeeter parameter: {VmParam}; Returned value: {Value}"
    )]
    public static partial void GetParam_Success_String(ILogger logger, string vmParam, string value);

    [LoggerMessage(
        EventId = (int)Event.GetParam_Error_Float,
        EventName = nameof(Event.GetParam_Error_Float),
        Level = LogLevel.Error,
        Message = "GetParam failed. Response: {Response}; Requested Voicemeeter parameter: {VmParam}; Returned value: {Value}; Expected type: {ExpectedType}"
    )]
    public static partial void GetParam_Error_Float(ILogger logger, Response response, string vmParam, float value, string expectedType);

    [LoggerMessage(
        EventId = (int)Event.GetParam_Error_String,
        EventName = nameof(Event.GetParam_Error_String),
        Level = LogLevel.Error,
        Message = "GetParam failed. Response: {Response}; Requested Voicemeeter parameter: {VmParam}; Returned value: {Value}; Expected type: {ExpectedType}"
    )]
    public static partial void GetParam_Error_String(ILogger logger, Response response, string vmParam, string value, string expectedType);
}