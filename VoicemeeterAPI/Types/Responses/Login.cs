// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using AtgDev.Voicemeeter;

namespace VoicemeeterAPI.Types.Responses
{
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
}