// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;
using VoicemeeterAPI.Types;

namespace VoicemeeterAPI.Messages
{
    internal abstract class VmApiException(string m)
        : Exception($"[VoicemeeterAPI] {m}")
    {
    }

    #region Remote Exceptions

    internal class RemoteException(string m)
        : VmApiException($"Remote Error: {m}")
    {
    }

    internal sealed class RemoteAccessException(string method, LoginResponse status)
        : RemoteException($"Access to {method} denied - LoginStatus: {status}")
    {
        public string Method { get; } = method;
        public LoginResponse LoginStatus { get; } = status;
    }

    internal sealed class LoginException(LoginResponse r)
        : RemoteException($"Login failed - {r}")
    {
        public LoginResponse Response { get; } = r;
    }

    internal sealed class GetVmKindException(InfoResponse r, Kind k)
        : RemoteException($"GetVoicemeeterKind failed - {r}; returned kind: {k}")
    {
        public InfoResponse Response { get; } = r;
        public Kind Kind { get; } = k;
    }

    #endregion

    #region Voicemeeter Exceptions

    internal class VoicemeeterException(string m)
        : VmApiException($"Voicemeeter Error: {m}")
    {
    }

    #endregion
}