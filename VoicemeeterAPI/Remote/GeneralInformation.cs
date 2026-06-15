// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using PBLivingston.VoicemeeterAPI.Types;
using Microsoft.Extensions.Logging;

public partial class Remote
{
    #region Get Voicemeeter Kind

    /// <inheritdoc cref="IRemote.GetKind()"/>
    internal (LoginResponse, Kind) GetKind_i(bool trace)
    {
        var info = trace ? LogLevel.Trace : LogLevel.Information;

        this.On_GetInfo_Start(typeof(Kind), info);

        (var result, var k) = this.wrapper.GetVoicemeeterType();

        LoginResponse login;
        var kind = (Kind)k;
        if (result == InfoResponse.Ok && kind.IsValid())
        {
            login = LoginResponse.Ok;
        }
        else if (result == InfoResponse.NoServer)
        {
            login = LoginResponse.VoicemeeterNotRunning;
            kind = Kind.None;
        }
        else
        {
            throw this.On_GetInfo_Error(result, k);
        }

        this.On_GetInfo_Success(kind, info);

        return (login, kind);
    }

    /// <inheritdoc/>
    public Kind GetKind()
    {
        using var scope = this.BeginInstanceScope();
        using var lk = this.stateLock.EnterScope();

        (this.loginStatus, var kind) = this.GetKind_i(false);

        this.On_ConnectionState_StateMismatch(kind);
        this.On_ConnectionState_StateMismatch(this.loginStatus);

        return kind;
    }

    #endregion

    #region Get Voicemeeter Version

    /// <inheritdoc cref="IRemote.GetVersion()"/>
    internal (LoginResponse, VmVersion) GetVersion_i(bool trace)
    {
        var info = trace ? LogLevel.Trace : LogLevel.Information;

        this.On_GetInfo_Start(typeof(VmVersion), info);

        (var result, var v) = this.wrapper.GetVoicemeeterVersion();

        LoginResponse login;
        VmVersion version = default;
        if (result == InfoResponse.Ok && VmVersion.IsValid(v))
        {
            login = LoginResponse.Ok;
            version = (VmVersion)v;
        }
        else if (result == InfoResponse.NoServer)
        {
            login = LoginResponse.VoicemeeterNotRunning;
        }
        else
        {
            throw this.On_GetInfo_Error(result, v);
        }

        this.On_GetInfo_Success(version, info);

        return (login, version);
    }

    /// <inheritdoc/>
    public VmVersion GetVersion()
    {
        using var scope = this.BeginInstanceScope();
        using var lk = this.stateLock.EnterScope();

        (this.loginStatus, var version) = this.GetVersion_i(false);

        this.On_ConnectionState_StateMismatch(version);
        this.On_ConnectionState_StateMismatch(this.loginStatus);

        return version;
    }

    #endregion

    #region Get Application State

    /// <inheritdoc cref="IRemote.GetAppState(App)"/>
    internal RunResponse GetAppState_i(App app, bool trace)
    {
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

        return result;
    }

    /// <inheritdoc/>
    public RunResponse GetAppState(App app)
    {
        using var scope = this.BeginInstanceScope();
        using var lk = this.stateLock.EnterScope();

        var result = this.GetAppState_i(app, false);

        if (this.loginStatus >= LoginResponse.LoggedOut)
        {
            return result;
        }

        if (app.IsVoicemeeter())
        {
            if (result < RunResponse.NotRunning)
            {
                this.loginStatus = LoginResponse.Ok;
                this.On_ConnectionState_StateMismatch(app.ToKind());
            }
            else
            {
                (this.loginStatus, var running) = this.GetKind_i(true);

                if (app == running.ToApp(this.wrapper.Is64Bit))
                {
                    throw this.On_GetInfo_Error(RunResponse.Error, app);
                }
            }

            this.On_ConnectionState_StateMismatch(this.loginStatus);
        }

        if (app is App.MacroButtons)
        {
            this.On_ConnectionState_StateMismatch(result);
        }

        return result;
    }

    #endregion

    #region Get Connection State

    /// <inheritdoc cref="IRemote.GetConnectionState()"/>
    internal (LoginResponse, ConnectionState) GetConnectionState_i()
    {
        this.On_GetConnectionState_Start();

        LoginResponse login;
        LoginResponse l;
        Kind kind;
        VmVersion version;
        RunResponse mbState;
        using (this.BeginMethodScope())
        {
            (login, kind) = this.GetKind_i(true);
            (l, version) = this.GetVersion_i(true);
            mbState = this.GetAppState_i(App.MacroButtons, true);
        }

        if (login != l || kind != version.K)
        {
            throw this.On_Method_Error(Response.Error);
        }

        ConnectionState state = new(login, mbState, kind, version);

        this.On_Method_Success();

        return (login, state);
    }

    /// <inheritdoc/>
    public ConnectionState GetConnectionState()
    {
        using var scope = this.BeginInstanceScope();
        using var lk = this.stateLock.EnterScope();

        ConnectionState state;
        if (this.loginStatus >= LoginResponse.LoggedOut)
        {
            this.On_GetConnectionState_Start();

            state = new(
                this.loginStatus,
                this.lastConnectionState.ButtonsState,
                this.lastConnectionState.RunningKind,
                this.lastConnectionState.RunningVersion
            );

            this.On_Method_Success();
        }
        else
        {
            (this.loginStatus, state) = this.GetConnectionState_i();
        }

        this.On_ConnectionState_Changed(state);

        return state;
    }

    #endregion
}
