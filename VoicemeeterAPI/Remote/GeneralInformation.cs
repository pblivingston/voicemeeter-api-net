// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

public partial class Remote
{
    #region Get Voicemeeter Kind

    /// <inheritdoc cref="IRemote.GetKind()"/>
    internal (LoginResponse, Kind) GetKind_i(string executionPath)
    {
        var e = Utilities.BuildPath(executionPath);

        this.WrapperCall(nameof(this.wrapper.GetVoicemeeterKind), e);

        (var result, var kind) = this.wrapper.GetVoicemeeterKind();

        LoginResponse login;
        if (this.HandleResponse(result, kind, e))
        {
            login = LoginResponse.Ok;
        }
        else
        {
            login = LoginResponse.VoicemeeterNotRunning;
            kind = Kind.None;
        }

        return (login, kind);
    }

    /// <inheritdoc/>
    public Kind GetKind()
    {
        var e = nameof(this.GetKind);

        using var scope = this.BeginCallScope();
        using var lk = this.stateLock.EnterScope();

        (this.loginStatus, var kind) = this.GetKind_i(e);

        this.HandleStaleCache(kind, e);
        this.HandleStaleCache(this.loginStatus, e);

        return kind;
    }

    #endregion

    #region Get Voicemeeter Version

    /// <inheritdoc cref="IRemote.GetVersion()"/>
    internal (LoginResponse, VmVersion) GetVersion_i(string executionPath)
    {
        var e = Utilities.BuildPath(executionPath);

        this.WrapperCall(nameof(this.wrapper.GetVoicemeeterVersion), e);

        (var result, var version) = this.wrapper.GetVoicemeeterVersion();

        LoginResponse login;
        if (this.HandleResponse(result, version, e))
        {
            login = LoginResponse.Ok;
        }
        else
        {
            login = LoginResponse.VoicemeeterNotRunning;
            version = default;
        }

        return (login, version);
    }

    /// <inheritdoc/>
    public VmVersion GetVersion()
    {
        var e = nameof(this.GetVersion);

        using var scope = this.BeginCallScope();
        using var lk = this.stateLock.EnterScope();

        (this.loginStatus, var version) = this.GetVersion_i(e);

        this.HandleStaleCache(version, e);
        this.HandleStaleCache(this.loginStatus, e);

        return version;
    }

    #endregion

    #region Get Application State

    /// <inheritdoc cref="IRemote.GetAppState(App)"/>
    internal RunResponse GetAppState_i(App app, string executionPath)
    {
        var e = Utilities.BuildPath(executionPath);

        this.WrapperCall(nameof(this.wrapper.GetApplicationState), e, app: app);

        var result = this.wrapper.GetApplicationState(app);

        return this.HandleResponse(result, app, e);
    }

    /// <inheritdoc/>
    public RunResponse GetAppState(App app)
    {
        var e = nameof(this.GetAppState);

        using var scope = this.BeginCallScope();
        using var lk = this.stateLock.EnterScope();

        var result = this.GetAppState_i(app, e);

        if (this.loginStatus >= LoginResponse.LoggedOut)
        {
            return result;
        }

        if (app is App.MacroButtons)
        {
            this.HandleStaleCache(result, e);
        }

        if (app.IsVoicemeeter())
        {
            Kind kind;
            if (result < RunResponse.NotRunning)
            {
                this.loginStatus = LoginResponse.Ok;
                kind = app.ToKind();
            }
            else
            {
                this.loginStatus = LoginResponse.VoicemeeterNotRunning;
                kind = Kind.None;
            }

            this.HandleStaleCache(kind, e);
            this.HandleStaleCache(this.loginStatus, e);
        }

        return result;
    }

    #endregion

    #region Get Connection State

    /// <inheritdoc cref="IRemote.GetConnectionState()"/>
    internal (ConnectionState previousState, ConnectionState currentState) GetConnectionState_i(string executionPath, bool loggedOut = false)
    {
        var e = Utilities.BuildPath(executionPath);

        var previousState = this.lastConnectionState;

        VmVersion version;
        RunResponse mbState;
        if (loggedOut)
        {
            version = previousState.RunningVersion;
            mbState = previousState.ButtonsState;
        }
        else
        {
            (this.loginStatus, version) = this.GetVersion_i(e);
            mbState = this.GetAppState_i(App.MacroButtons, e);
        }

        var currentState = this.HandleResponse(this.loginStatus, version, mbState, loggedOut, e);
        this.HandleStaleCache(currentState, e);

        return (previousState, currentState);
    }

    /// <inheritdoc/>
    public ConnectionState GetConnectionState()
    {
        using var scope = this.BeginCallScope();

        this.MethodStart();

        ConnectionState previousState;
        ConnectionState currentState;
        using (this.stateLock.EnterScope())
        {
            (previousState, currentState) = this.GetConnectionState_i(
                nameof(this.GetConnectionState),
                this.loginStatus >= LoginResponse.LoggedOut
            );
        }

        this.OnConnectionStateChanged(previousState, currentState);

        return currentState;
    }

    #endregion
}
