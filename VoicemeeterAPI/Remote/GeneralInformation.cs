// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using PBLivingston.VoicemeeterAPI.Types;
using Microsoft.Extensions.Logging;

public partial class Remote
{
    #region Get Voicemeeter Kind

    /// <inheritdoc cref="IRemote.GetKind()"/>
    internal Kind GetInfo_Kind(bool nested)
    {
        this.LoginGuard(requiredStatus: LoginResponse.VoicemeeterNotRunning);

        var info = nested ? LogLevel.Trace : LogLevel.Information;
        var warning = nested ? LogLevel.Trace : LogLevel.Warning;

        this.On_GetInfo_Start(typeof(Kind), info);

        var result = this.vmrApi.GetVoicemeeterType(out var k);

        var kind = (Kind)k;
        if (result == InfoResponse.Ok && kind.IsValid())
        {
            this.loginStatus = LoginResponse.Ok;
        }
        else if (result == InfoResponse.NoServer)
        {
            this.loginStatus = LoginResponse.VoicemeeterNotRunning;
            kind = Kind.None;
        }
        else
        {
            throw this.On_GetInfo_Error(result, k);
        }

        this.On_GetInfo_Success(kind, info);

        this.On_ConnectionState_StateMismatch(kind, warning);

        return kind;
    }

    /// <inheritdoc/>
    public Kind GetKind()
    {
        using var scope = this.BeginInstanceScope();

        return this.GetInfo_Kind(false);
    }

    #endregion

    #region Get Voicemeeter Version

    /// <inheritdoc cref="IRemote.GetVersion()"/>
    internal VmVersion GetInfo_Version(bool nested)
    {
        this.LoginGuard(requiredStatus: LoginResponse.VoicemeeterNotRunning);

        var info = nested ? LogLevel.Trace : LogLevel.Information;
        var warning = nested ? LogLevel.Trace : LogLevel.Warning;

        this.On_GetInfo_Start(typeof(VmVersion), info);

        var result = this.vmrApi.GetVoicemeeterVersion(out var v);

        VmVersion version = default;
        if (result == InfoResponse.Ok && VmVersion.IsValid(v))
        {
            this.loginStatus = LoginResponse.Ok;
            version = (VmVersion)v;
        }
        else if (result == InfoResponse.NoServer)
        {
            this.loginStatus = LoginResponse.VoicemeeterNotRunning;
        }
        else
        {
            throw this.On_GetInfo_Error(result, v);
        }

        this.On_GetInfo_Success(version, info);

        this.On_ConnectionState_StateMismatch(version, warning);

        return version;
    }

    /// <inheritdoc/>
    public VmVersion GetVersion()
    {
        using var scope = this.BeginInstanceScope();

        return this.GetInfo_Version(false);
    }

    #endregion
}
