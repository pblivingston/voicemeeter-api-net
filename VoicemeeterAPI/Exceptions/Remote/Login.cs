// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using VoicemeeterAPI.Types.Responses;

namespace VoicemeeterAPI.Exceptions.Remote
{
    /// <summary>
    ///   Exception thrown when a login attempt fails.
    /// </summary>
    /// <param name="r">API response</param>
    internal sealed class LoginException(LoginResponse r)
        : RemoteException($"Login failed - {r}")
    {
        public LoginResponse Response { get; } = r;
    }
}