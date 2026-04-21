// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using PBLivingston.VoicemeeterAPI.Types;
using PBLivingston.VoicemeeterAPI.EventManagement;
using Microsoft.Extensions.Logging;

namespace PBLivingston.VoicemeeterAPI;

partial class Remote
{
    #region Get Voicemeeter Kind

    /// <inheritdoc cref="IRemote.GetKind()"/>
    internal Kind GetKind(bool nested)
    {
        LoginGuard(requiredStatus: LoginResponse.VoicemeeterNotRunning);

        var info = nested ? LogLevel.Trace : LogLevel.Information;
        var warning = nested ? LogLevel.Trace : LogLevel.Warning;

        RemoteDispatch.GetInfo_Start(_logger, info, typeof(Kind));

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
        else throw RemoteDispatch.GetInfo_Error(_logger, result, k);

        RemoteDispatch.GetInfo_Success(_logger, info, kind);

        if (kind != _lastState.RunningKind)
            RemoteDispatch.GetInfo_StateMismatch(_logger, warning, _lastState.RunningKind);

        return kind;
    }

    /// <inheritdoc/>
    public Kind GetKind()
    {
        using var scope = BeginInstanceScope();

        return GetKind(false);
    }

    #endregion

    #region Get Voicemeeter Version

    /// <inheritdoc cref="IRemote.GetVersion()"/>
    internal VmVersion GetVersion(bool nested)
    {
        LoginGuard(requiredStatus: LoginResponse.VoicemeeterNotRunning);

        var info = nested ? LogLevel.Trace : LogLevel.Information;
        var warning = nested ? LogLevel.Trace : LogLevel.Warning;

        RemoteDispatch.GetInfo_Start(_logger, info, typeof(VmVersion));

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
        else throw RemoteDispatch.GetInfo_Error(_logger, result, v);

        RemoteDispatch.GetInfo_Success(_logger, info, version);

        if (version != _lastState.RunningVersion)
            RemoteDispatch.GetInfo_StateMismatch(_logger, warning, _lastState.RunningVersion);

        return version;
    }

    /// <inheritdoc/>
    public VmVersion GetVersion()
    {
        using var scope = BeginInstanceScope();

        return GetVersion(false);
    }

    #endregion
}
