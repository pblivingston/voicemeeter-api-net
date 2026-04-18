// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using Microsoft.Extensions.Logging;

namespace PBLivingston.VoicemeeterAPI.EventManagement.Logging;

internal static partial class RemoteLog
{
    #region Login

    [LoggerMessage(
        EventId = (int)Event.Login_Start,
        EventName = nameof(Event.Login_Start),
        Level = LogLevel.Information,
        Message = "Logging in..."
    )]
    public static partial void Login_Start(ILogger logger);

    [LoggerMessage(
        EventId = (int)Event.Login_VmNotRunning,
        EventName = nameof(Event.Login_VmNotRunning),
        Level = LogLevel.Warning,
        Message = "Login successful but Voicemeeter is not running."
    )]
    public static partial void Login_VmNotRunning(ILogger logger);

    #endregion

    #region Logout

    [LoggerMessage(
        EventId = (int)Event.Logout_Start,
        EventName = nameof(Event.Logout_Start),
        Level = LogLevel.Information,
        Message = "Logging out..."
    )]
    public static partial void Logout_Start(ILogger logger);

    [LoggerMessage(
        EventId = (int)Event.Logout_Timeout,
        EventName = nameof(Event.Logout_Timeout),
        Level = LogLevel.Error,
        Message = "Logout timed out. Last reponse: {LastResponse}"
    )]
    public static partial void Logout_Timeout(ILogger logger, string lastResponse);

    #endregion

    #region Run

    [LoggerMessage(
        EventId = (int)Event.Run_Start,
        EventName = nameof(Event.Run_Start),
        Level = LogLevel.Information,
        Message = "Running application: {AppName}..."
    )]
    public static partial void Run_Start(ILogger logger, string appName);

    [LoggerMessage(
        EventId = (int)Event.Run_Error,
        EventName = nameof(Event.Run_Error),
        Level = LogLevel.Error,
        Message = "Run failed. Response: {Response}; Requested application: {AppName}"
    )]
    public static partial void Run_Error(ILogger logger, string response, string appName);

    #endregion

    #region WaitForRunning

    [LoggerMessage(
        EventId = (int)Event.WaitForRunning_Start,
        EventName = nameof(Event.WaitForRunning_Start),
        Level = LogLevel.Information,
        Message = "Waiting for running Voicemeeter..."
    )]
    public static partial void WaitForRunning_Start(ILogger logger);

    [LoggerMessage(
        EventId = (int)Event.WaitForRunning_Detected,
        EventName = nameof(Event.WaitForRunning_Detected),
        Level = LogLevel.Information,
        Message = "Voicemeeter {KindName} v{VersionString} is running."
    )]
    public static partial void WaitForRunning_Detected(ILogger logger, string kindName, string versionString);

    [LoggerMessage(
        EventId = (int)Event.WaitForRunning_Timeout,
        EventName = nameof(Event.WaitForRunning_Timeout),
        Level = LogLevel.Warning,
        Message = "Timed out waiting for Voicemeeter."
    )]
    public static partial void WaitForRunning_Timeout(ILogger logger);

    #endregion
}