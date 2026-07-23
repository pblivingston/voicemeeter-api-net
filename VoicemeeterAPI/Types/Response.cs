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
    LoggedOut = 2,
    /// <summary>
    ///   from VoicemeeterAPI
    /// </summary>
    Unknown = 3
}

public enum RunResponse
{
    /// <summary>
    ///   from VoicemeeterAPI
    /// </summary>
    AlreadyShutDown = -103,
    /// <summary>
    ///   from VoicemeeterAPI
    /// </summary>
    Timeout = -102,
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
    /// <summary>
    ///   from VoicemeeterAPI
    /// </summary>
    UnknownApp = -101,

    StructureMismatch = -5,
    UnknownParameter = -3,
    NoServer = -2,
    Error = -1,
    Ok = 0,
    Dirty = 1
}

public enum ScriptResponse
{
    Error4 = -4,
    Error3 = -3,
    NoServer = -2,
    Error = -1,
    Ok = 0,
    ScriptError
}

public enum LevelResponse
{
    OutOfRange = -4,
    NoLevel = -3,
    NoServer = -2,
    Error = -1,
    Ok = 0
}

public enum GetMidiResponse
{
    NoData6 = -6,
    NoData5 = -5,
    NoServer = -2,
    Error = -1
}

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
