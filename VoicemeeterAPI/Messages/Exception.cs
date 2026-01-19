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

    internal sealed class RemoteAccessException(string m, LoginResponse s)
        : RemoteException($"Access to {m} denied - LoginStatus: {s}")
    {
        public string Method { get; } = m;
        public LoginResponse LoginStatus { get; } = s;
    }

    internal sealed class LoginException(LoginResponse r)
        : RemoteException($"Login failed - {r}")
    {
        public LoginResponse Response { get; } = r;
    }

    internal sealed class RunException(RunResponse r, Kind k)
        : RemoteException($"RunVoicemeeter failed - {r}; requested kind: {k}")
    {
        public RunResponse Response { get; } = r;
        public Kind Kind { get; } = k;
    }

    internal sealed class GetKindException(InfoResponse r, Kind k)
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