// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI.Types;

public enum Response
{
    TypeMismatch = -6, //
    StructureMismatch = -5,
    UnknownApp = -4, //
    UnknownParameter = -3,
    NoServer = -2,
    Error = -1,
    Ok = 0,
    Dirty = 1
}

public enum LoginResponse
{
    Timeout = -4, //
    AlreadyLoggedOut = -3, //
    AlreadyLoggedIn = -2,
    NoClient = -1,
    Ok = 0,
    VoicemeeterNotRunning = 1,
    LoggedOut = 2, //
    Unknown = 3 //
}

public enum RunResponse
{
    Error = -5, //
    Timeout = -4, //
    AlreadyShutDown = -3, //
    UnknownApp = -2,
    NotInstalled = -1,
    Ok = 0,
    Hidden = 1, //
    NotRunning = 2, //
    NotResponding = 3 //
}

public enum InfoResponse
{
    NoServer = -2,
    NoClient = -1,
    Ok = 0
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
