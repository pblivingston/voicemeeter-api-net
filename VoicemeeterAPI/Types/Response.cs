// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using AtgDev.Voicemeeter;

namespace VoicemeeterAPI.Types
{
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
    ///   Represents responses from <see cref="RemoteApiWrapper.Login()"/> and <see cref="RemoteApiWrapper.Logout()"/>
    ///   as well as values for <see cref="IRemote.LoginStatus"/>.
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

    public enum RunResponse
    {
        UnknownKind = -2,
        NotInstalled = -1,
        Ok = 0,
        Timeout
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
}