// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;
using VoicemeeterAPI.Types.Responses;

namespace VoicemeeterAPI.Exceptions
{
    /// <summary>
    ///   Base exception for all exceptions thrown by the <see cref="Remote"/> class.
    /// </summary>
    /// <param name="m">Message</param>
    internal class RemoteException(string m)
        : Exception($"VoicemeeterRemote Error: {m}")
    {
    }

    /// <summary>
    ///   Exception thrown when a <see cref="IRemote"/> method is called without a successful login.
    /// </summary>
    /// <param name="method">Name of the called method</param>
    /// <param name="status"><see cref="VoicemeeterAPI.Remote.LoginStatus"> at the time of the call</param>
    internal sealed class RemoteAccessException(string method, LoginResponse status)
        : RemoteException($"Access to {method} denied - LoginStatus: {status}")
    {
        public string Method { get; } = method;
        public LoginResponse LoginStatus { get; } = status;
    }
}