// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using VoicemeeterAPI.Messages;

namespace VoicemeeterAPI.Types;

/// <summary>
///   Responses from <see cref="Remote.ParamsDirty()"/>, <see cref="Remote.ButtonsDirty()"/> ... (more to be added)
/// </summary>
public enum Response
{
    StructureMismatch = -5,
    UnknownParameter = -3,
    NoServer = -2,
    Error = -1,
    Ok = 0,
    Dirty
}

/// <summary>
///   Responses from <see cref="Remote.Login()"/> and <see cref="Remote.Logout()"/> for <see cref="LoginException"/>
///   and values for <see cref="IRemote.LoginStatus"/>.
/// </summary>
public enum LoginResponse
{
    AlreadyLoggedIn = -2,
    NoClient = -1,
    Ok = 0,
    VoicemeeterNotRunning = 1,
    LoggedOut,
    Timeout,
    Unknown
}

/// <summary>
///   Responses from <see cref="Remote.Run(int)"/> for <see cref="RunException"/>.
/// </summary>
public enum RunResponse
{
    UnknownApp = -2,
    NotInstalled = -1,
    Ok = 0,
    Timeout
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
