// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;
using VoicemeeterAPI.Types.Responses;

namespace VoicemeeterAPI.Exceptions.Remote
{

    /// <summary>
    ///   Exception thrown when a login attempt fails.
    /// </summary>
    /// <param name="r">The login response that caused the exception</param>
    internal sealed class LoginException(LoginResponse r)
        : RemoteException($"Login failed - {r}")
    {
        public LoginResponse Response { get; } = r;
    }



    /// <summary>
    ///   Exception thrown when a logout attempt fails.
    /// </summary>
    /// <param name="r">The login response that caused the exception</param>
    /// <param name="c">The error code returned by the logout operation</param>
    /// <remarks>
    ///   Currently unused - logout only throws <see cref="ObjectDisposedException"/>
    ///   when the instance is already disposed.
    /// </remarks>
    internal sealed class LogoutException(LoginResponse r, int c)
        : RemoteException($"Logout failed with code {c}; response: {r}")
    {
        public LoginResponse Response { get; } = r;
        public int Code { get; } = c;
    }
}