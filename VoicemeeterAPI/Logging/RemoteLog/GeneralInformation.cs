// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI.Logging;

using Microsoft.Extensions.Logging;
using PBLivingston.VoicemeeterAPI.Types;

internal static partial class RemoteLog
{
    [LoggerMessage(
        EventId = (int)Event.GetInfo_Start,
        EventName = nameof(Event.GetInfo_Start),
        Message = "Getting running {InfoType}..."
    )]
    public static partial void GetInfo_Start(ILogger logger, LogLevel level, string infoType);

    [LoggerMessage(
        EventId = (int)Event.GetInfo_Start_AppState,
        EventName = nameof(Event.GetInfo_Start_AppState),
        Message = "Getting {App} state..."
    )]
    public static partial void GetInfo_Start_AppState(ILogger logger, LogLevel level, App app);

    [LoggerMessage(
        EventId = (int)Event.GetInfo_Success_Kind,
        EventName = nameof(Event.GetInfo_Success_Kind),
        Message = "GetKind successful. Returned Kind: {Value}"
    )]
    public static partial void GetInfo_Success_Kind(ILogger logger, LogLevel level, Kind value);

    [LoggerMessage(
        EventId = (int)Event.GetInfo_Success_Version,
        EventName = nameof(Event.GetInfo_Success_Version),
        Message = "GetVersion successful. Returned VmVersion: {Value}"
    )]
    public static partial void GetInfo_Success_Version(ILogger logger, LogLevel level, VmVersion value);

    [LoggerMessage(
        EventId = (int)Event.GetInfo_Success_AppState,
        EventName = nameof(Event.GetInfo_Success_AppState),
        Message = "GetAppState successful. {App} state: {State}"
    )]
    public static partial void GetInfo_Success_AppState(ILogger logger, LogLevel level, App app, RunResponse state);

    [LoggerMessage(
        EventId = (int)Event.GetInfo_Error,
        EventName = nameof(Event.GetInfo_Error),
        Level = LogLevel.Error,
        Message = "{MethodName} failed. Response: {Response}; Returned value: {Value}"
    )]
    public static partial void GetInfo_Error(ILogger logger, string methodName, InfoResponse response, int value);

    [LoggerMessage(
        EventId = (int)Event.GetInfo_Error_AppState,
        EventName = nameof(Event.GetInfo_Error_AppState),
        Level = LogLevel.Error,
        Message = "GetAppState failed. App: {App}; Response: {Response}"
    )]
    public static partial void GetInfo_Error_AppState(ILogger logger, RunResponse response, App app);

    [LoggerMessage(
        EventId = (int)Event.GetInfo_NotResponding,
        EventName = nameof(Event.GetInfo_NotResponding),
        Message = "GetAppState successful but {App} is not responding."
    )]
    public static partial void GetInfo_NotResponding(ILogger logger, LogLevel level, App app);
}
