// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using PBLivingston.VoicemeeterAPI.Types;
using Microsoft.Extensions.Logging;

public partial class Remote
{
    #region Get Voicemeeter Kind

    /// <inheritdoc cref="IRemote.GetKind()"/>
    internal Kind GetInfo_Kind(bool trace)
    {
        this.LoginGuard(requiredStatus: LoginResponse.VoicemeeterNotRunning);

        var info = trace ? LogLevel.Trace : LogLevel.Information;
        var warning = trace ? LogLevel.Trace : LogLevel.Warning;

        this.On_GetInfo_Start(typeof(Kind), info);

        var result = this.wrapper.GetVoicemeeterType(out var k);

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

        this.On_ConnectionState_StateMismatch(this.loginStatus, warning);
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
    internal VmVersion GetInfo_Version(bool trace)
    {
        this.LoginGuard(requiredStatus: LoginResponse.VoicemeeterNotRunning);

        var info = trace ? LogLevel.Trace : LogLevel.Information;
        var warning = trace ? LogLevel.Trace : LogLevel.Warning;

        this.On_GetInfo_Start(typeof(VmVersion), info);

        var result = this.wrapper.GetVoicemeeterVersion(out var v);

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

        this.On_ConnectionState_StateMismatch(this.loginStatus, warning);
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

    #region Get Application State

    /// <inheritdoc cref="IRemote.GetAppState(App)"/>
    internal RunResponse GetInfo_AppState(App app, bool trace)
    {
        this.LoginGuard(requiredStatus: LoginResponse.VoicemeeterNotRunning);

        var info = trace ? LogLevel.Trace : LogLevel.Information;
        var warning = trace ? LogLevel.Trace : LogLevel.Warning;

        this.On_GetInfo_Start(app, info);

        var result = this.wrapper.GetApplicationState(app);

        if (result < RunResponse.Ok)
        {
            throw this.On_GetInfo_Error(result, app);
        }

        if (result is RunResponse.NotResponding)
        {
            this.On_GetInfo_NotResponding(app, warning);
        }
        else
        {
            this.On_GetInfo_Success(app, result, info);
        }

        if (app.IsVoicemeeter())
        {
            var running = this.LastConnectionState.RunningKind.ToApp(this.wrapper.Is64Bit);
            if (app == running || running is App.None)
            {
                this.loginStatus = result < RunResponse.NotRunning
                    ? LoginResponse.Ok
                    : LoginResponse.VoicemeeterNotRunning;

                this.On_ConnectionState_StateMismatch(this.loginStatus, warning);
            }
            else if (result < RunResponse.NotRunning)
            {
                this.On_ConnectionState_StateMismatch(app.ToKind(), warning);
            }
        }

        if (app is App.MacroButtons)
        {
            this.On_ConnectionState_StateMismatch(result, warning);
        }

        return result;
    }

    /// <inheritdoc/>
    public RunResponse GetAppState(App app)
    {
        using var scope = this.BeginInstanceScope();

        return this.GetInfo_AppState(app, false);
    }

    #endregion
}
