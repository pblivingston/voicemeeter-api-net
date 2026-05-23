// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI.Types;

using PBLivingston.VoicemeeterAPI.Exceptions;

/// <summary>
///   Responses from <see cref="Remote.ParamsDirty()"/>, <see cref="Remote.ButtonsDirty()"/> ... (more to be added)
/// </summary>
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

/// <summary>
///   Responses from <see cref="Remote.Login(int, int)"/> and <see cref="Remote.Logout(int, int)"/> for <see cref="LoginException"/>
///   and values for <see cref="IRemote.LoginStatus"/>.
/// </summary>
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

/// <summary>
///   Responses from <see cref="Remote.Run(int, int, int)"/> for <see cref="RunException"/>.
/// </summary>
public enum RunResponse
{
    Error = -5, //
    Timeout = -4, //
    AlreadyShutDown = -3, //
    UnknownApp = -2,
    NotInstalled = -1,
    Ok = 0,
    Hidden = 1, //
    NotResponding = 2, //
    NotRunning = 3 //
}

/// <summary>
///   Responses from <see cref="Remote.GetKind()"/> and <see cref="Remote.GetVersion()"/>.
/// </summary>
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
