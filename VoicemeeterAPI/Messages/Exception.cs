// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;
using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI.Messages;

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

internal sealed class RunException(RunResponse r, App a)
    : RemoteException($"Run failed - {r}; requested app: {a}")
{
    public RunResponse Response { get; } = r;
    public App App { get; } = a;
}

internal class GetParamException<T>(Response r, string p, T v, Type t)
    : RemoteException($"GetParam failed - {r}; requested param: {p}, returned value: {v}, expected type: {t}")
{
    public Response Response { get; } = r;
    public string Param { get; } = p;
    public T Value { get; } = v;
    public Type ExpectedType { get; } = t;
}

#endregion

#region Voicemeeter Exceptions

internal class VoicemeeterException(string m)
    : VmApiException($"Voicemeeter Error: {m}")
{
}

#endregion
