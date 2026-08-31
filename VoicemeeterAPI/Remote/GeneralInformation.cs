// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

public partial class Remote
{
    #region Get Login Status

    /// <inheritdoc cref="IRemote.GetLoginStatus()"/>
    internal LoginResponse GetLoginStatus_i(string executionPath)
    {
        var e = Utilities.BuildPath(executionPath);

        this.WrapperCall(nameof(this.wrapper.GetVoicemeeterKind), e);

        (var response, _) = this.wrapper.GetVoicemeeterKind();

        return this.HandleLoginStatusResponse(response, e);
    }

    /// <inheritdoc/>
    public LoginResponse GetLoginStatus()
    {
        var e = nameof(this.GetLoginStatus);

        using var scope = this.BeginCallScope();
        using var lk = this.stateLock.EnterScope();

        var status = this.GetLoginStatus_i(e);

        this.HandleStaleCache(status, e);

        return status;
    }

    #endregion

    #region Get Voicemeeter Kind

    /// <inheritdoc cref="IRemote.GetKind()"/>
    internal Kind GetKind_i(string executionPath)
    {
        var e = Utilities.BuildPath(executionPath);

        this.WrapperCall(nameof(this.wrapper.GetVoicemeeterKind), e);

        (var response, var kind) = this.wrapper.GetVoicemeeterKind();

        return this.HandleGetKindResponse(response, kind, e);
    }

    /// <inheritdoc/>
    public Kind GetKind()
    {
        var e = nameof(this.GetKind);

        using var scope = this.BeginCallScope();
        using var lk = this.stateLock.EnterScope();

        var kind = this.GetKind_i(e);

        this.HandleStaleCache(kind, e);

        return kind;
    }

    #endregion

    #region Get Voicemeeter Version

    /// <inheritdoc cref="IRemote.GetVersion()"/>
    internal VmVersion GetVersion_i(string executionPath)
    {
        var e = Utilities.BuildPath(executionPath);

        this.WrapperCall(nameof(this.wrapper.GetVoicemeeterVersion), e);

        (var response, var version) = this.wrapper.GetVoicemeeterVersion();

        return this.HandleGetVersionResponse(response, version, e);
    }

    /// <inheritdoc/>
    public VmVersion GetVersion()
    {
        var e = nameof(this.GetVersion);

        using var scope = this.BeginCallScope();
        using var lk = this.stateLock.EnterScope();

        var version = this.GetVersion_i(e);

        this.HandleStaleCache(version, e);

        return version;
    }

    #endregion

    #region Get Application State

    /// <inheritdoc cref="IRemote.GetAppState(App)"/>
    internal RunResponse GetAppState_i(App app, string executionPath)
    {
        var e = Utilities.BuildPath(executionPath);

        this.WrapperCall(nameof(this.wrapper.GetApplicationState), e, app: app);

        var response = this.wrapper.GetApplicationState(app);

        return this.HandleAppStateResponse(response, app, e);
    }

    /// <inheritdoc/>
    public RunResponse GetAppState(App app)
    {
        var e = nameof(this.GetAppState);

        using var scope = this.BeginCallScope();
        using var lk = this.stateLock.EnterScope();

        var result = this.GetAppState_i(app, e);

        if (app is App.MacroButtons)
        {
            this.HandleStaleCache(result, e);
        }

        if (app.IsVoicemeeter())
        {
            this.HandleStaleCache((app, result), e);
        }

        return result;
    }

    #endregion

    #region Get Voicemeeter State

    /// <inheritdoc cref="IRemote.GetVoicemeeterState()"/>
    internal (App App, RunResponse State) GetVoicemeeterState_i(string executionPath)
    {
        var e = Utilities.BuildPath(executionPath);

        this.WrapperCall(nameof(this.wrapper.GetVoicemeeterState), e);

        return this.HandleVmStateResponse(this.wrapper.GetVoicemeeterState(), e);
    }

    /// <inheritdoc/>
    public (App App, RunResponse State) GetVoicemeeterState()
    {
        var e = nameof(this.GetVoicemeeterState);

        using var scope = this.BeginCallScope();
        using var lk = this.stateLock.EnterScope();

        var result = this.GetVoicemeeterState_i(e);

        this.HandleStaleCache(result, e);

        return result;
    }

    #endregion

    #region Refresh Connection State

    /// <inheritdoc cref="IRemote.RefreshConnectionState()"/>
    internal (ConnectionState PreviousState, ConnectionState CurrentState) RefreshConnectionState_i(string executionPath)
    {
        var e = Utilities.BuildPath(executionPath);

        var previousState = this.connectionState;

        var loginStatus = this.GetLoginStatus_i(e);

        RunResponse vmState;
        App vmApp;
        VmVersion vmVersion;
        RunResponse mbState;
        if (loginStatus.IsLoggedIn())
        {
            (vmApp, vmState) = this.GetVoicemeeterState_i(e);
            vmVersion = this.GetVersion_i(e);
            mbState = this.GetAppState_i(App.MacroButtons, e);
        }
        else
        {
            vmApp = previousState.VoicemeeterApp;
            vmState = previousState.VoicemeeterState;
            vmVersion = previousState.VoicemeeterVersion;
            mbState = previousState.MacroButtonsState;
        }

        var currentState = this.CreateConnectionState(loginStatus, vmState, vmApp, vmVersion, mbState, e);
        this.UpdateStaleCache(currentState, e);

        return (previousState, currentState);
    }

    /// <inheritdoc/>
    public ConnectionState RefreshConnectionState()
    {
        using var scope = this.BeginCallScope();

        this.MethodStart();

        ConnectionState previousState;
        ConnectionState currentState;
        using (this.stateLock.EnterScope())
        {
            (previousState, currentState) = this.RefreshConnectionState_i(nameof(this.RefreshConnectionState));
        }

        this.OnConnectionStateChanged(previousState, currentState);

        return currentState;
    }

    #endregion
}
