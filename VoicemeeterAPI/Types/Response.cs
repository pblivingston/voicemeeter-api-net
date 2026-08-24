// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

public enum LoginResponse
{
    AlreadyLoggedIn = -2,
    NoClient = -1,
    Ok = 0,
    VoicemeeterNotRunning = 1,

    /// <summary>
    ///   from VoicemeeterAPI
    /// </summary>
    LoggedOut = 2
}

public enum RunResponse
{
    /// <summary>
    ///   from VoicemeeterAPI
    /// </summary>
    AlreadyShutDown = -102,
    /// <summary>
    ///   from VoicemeeterAPI
    /// </summary>
    Error = -101,

    UnknownApp = -2,
    NotInstalled = -1,
    Ok = 0,

    /// <summary>
    ///   from VoicemeeterAPI
    /// </summary>
    Hidden = 1,
    /// <summary>
    ///   from VoicemeeterAPI
    /// </summary>
    NotRunning = 2,
    /// <summary>
    ///   from VoicemeeterAPI
    /// </summary>
    NotResponding = 3
}

public enum Response
{
    StructureMismatch = -5,
    UnknownParameter = -3,
    NoServer = -2,
    Error = -1,
    Ok = 0,
    Dirty = 1
}

/// <summary>
///   >0: script line causing error
/// </summary>
public enum ScriptResponse
{
    Error4 = -4,
    Error3 = -3,
    NoServer = -2,
    Error = -1,
    Ok = 0
}

public enum LevelResponse
{
    OutOfRange = -4,
    NoLevel = -3,
    NoServer = -2,
    Error = -1,
    Ok = 0
}

/// <summary>
///   >0: number of bytes placed in buffer
/// </summary>
public enum GetMidiResponse
{
    NoData6 = -6,
    NoData5 = -5,
    NoServer = -2,
    Error = -1
}

/// <summary>
///   >0: number of bytes sent
/// </summary>
public enum SendMidiResponse
{
    CannotSend = -5,
    NoServer = -2,
    Error = -1
}

public enum CallbackResponse
{
    NotRegistered = -2,
    Error = -1,
    Ok = 0,
    AlreadyRegistered = 1
}

public static class ResponseExt
{
    public static bool IsLoggedIn(this LoginResponse response)
        => response is LoginResponse.Ok or LoginResponse.VoicemeeterNotRunning;

    public static bool IsResponding(this RunResponse response)
        => response is RunResponse.Ok or RunResponse.Hidden;

    public static bool IsRunning(this RunResponse response)
        => response is RunResponse.Ok or RunResponse.Hidden or RunResponse.NotResponding;
}
