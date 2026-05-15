// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using PBLivingston.VoicemeeterAPI.Types;
using Microsoft.Extensions.Logging;

namespace PBLivingston.VoicemeeterAPI;

partial class Remote
{
    #region Get Voicemeeter Kind

    /// <inheritdoc cref="IRemote.GetKind()"/>
    internal Kind GetInfo_Kind(bool nested)
    {
        LoginGuard(requiredStatus: LoginResponse.VoicemeeterNotRunning);

        var info = nested ? LogLevel.Trace : LogLevel.Information;
        var warning = nested ? LogLevel.Trace : LogLevel.Warning;

        On_GetInfo_Start(typeof(Kind), info);

        var result = _vmrApi.GetVoicemeeterType(out int k);

        var kind = (Kind)k;
        if (result == InfoResponse.Ok && kind.IsValid())
        {
            _loginStatus = LoginResponse.Ok;
        }
        else if (result == InfoResponse.NoServer)
        {
            _loginStatus = LoginResponse.VoicemeeterNotRunning;
            kind = Kind.None;
        }
        else throw On_GetInfo_Error(result, k);

        On_GetInfo_Success(kind, info);

        if (kind != _lastState.RunningKind)
            On_ConnectionState_StateMismatch(kind, warning);

        return kind;
    }

    /// <inheritdoc/>
    public Kind GetKind()
    {
        using var scope = BeginInstanceScope();

        return GetInfo_Kind(false);
    }

    #endregion

    #region Get Voicemeeter Version

    /// <inheritdoc cref="IRemote.GetVersion()"/>
    internal VmVersion GetInfo_Version(bool nested)
    {
        LoginGuard(requiredStatus: LoginResponse.VoicemeeterNotRunning);

        var info = nested ? LogLevel.Trace : LogLevel.Information;
        var warning = nested ? LogLevel.Trace : LogLevel.Warning;

        On_GetInfo_Start(typeof(VmVersion), info);

        var result = _vmrApi.GetVoicemeeterVersion(out int v);

        VmVersion version = default;
        if (result == InfoResponse.Ok && VmVersion.IsValid(v))
        {
            _loginStatus = LoginResponse.Ok;
            version = (VmVersion)v;
        }
        else if (result == InfoResponse.NoServer)
        {
            _loginStatus = LoginResponse.VoicemeeterNotRunning;
        }
        else throw On_GetInfo_Error(result, v);

        On_GetInfo_Success(version, info);

        if (version != _lastState.RunningVersion)
            On_ConnectionState_StateMismatch(version, warning);

        return version;
    }

    /// <inheritdoc/>
    public VmVersion GetVersion()
    {
        using var scope = BeginInstanceScope();

        return GetInfo_Version(false);
    }

    #endregion
}
