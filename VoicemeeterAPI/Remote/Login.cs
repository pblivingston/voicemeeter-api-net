// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

public partial class Remote
{
    #region Login

    /// <inheritdoc cref="IRemote.Login()"/>
    internal (ConnectionState previousState, ConnectionState currentState) Login_i(string executionPath)
    {
        var e = Utilities.BuildPath(executionPath);

        this.WrapperCall(nameof(this.wrapper.Login), e);

        var response = this.wrapper.Login();

        this.HandleLoginResponse(response, e);

        var states = this.RefreshConnectionState_i(e);

        this.HandleConnectionState(states.CurrentState, e);

        return states;
    }

    #region Login

    /// <inheritdoc/>
    public ConnectionState Login()
    {
        var e = nameof(this.Login);

        using var scope = this.BeginCallScope();

        this.MethodStart();

        ConnectionState previousState;
        ConnectionState currentState;
        using (this.stateLock.EnterScope())
        {
            (previousState, currentState) = this.Login_i(e);
        }

        this.OnConnectionStateChanged(previousState, currentState);

        return currentState;
    }

    #endregion

    #region LoginAsync

    /// <inheritdoc cref="IRemote.LoginAsync(CancellationToken)"/>
    internal async Task<(ConnectionState previousState, ConnectionState currentState)> LoginAsync_i(string executionPath, CancellationToken cancellationToken)
    {
        var e = Utilities.BuildPath(executionPath);

        (var previousState, var currentState) = this.Login_i(e);

        if (currentState.ConnectedToVoicemeeter)
        {
            await this.WaitForEngineSettle(e, cancellationToken);

            (_, currentState) = this.RefreshConnectionState_i(e);
        }

        return (previousState, currentState);
    }

    /// <inheritdoc/>
    public async Task<ConnectionState> LoginAsync(CancellationToken cancellationToken = default)
    {
        var e = nameof(this.LoginAsync);

        using var scope = this.BeginCallScope();

        this.MethodStart();

        ConnectionState previousState;
        ConnectionState currentState;
        using (await this.stateLock.EnterScopeAsync(cancellationToken))
        {
            (previousState, currentState) = await this.LoginAsync_i(e, cancellationToken);
        }

        this.OnConnectionStateChanged(previousState, currentState);

        return currentState;
    }

    #endregion

    #endregion

    #region Logout

    /// <inheritdoc cref="IRemote.Logout()"/>
    private void Logout_i(string executionPath)
    {
        var e = Utilities.BuildPath(executionPath);

        this.WrapperCall(nameof(this.wrapper.Logout), e);

        var response = this.wrapper.Logout();

        this.HandleLogoutResponse(response, e);
    }

    /// <inheritdoc/>
    public void Logout()
    {
        var e = nameof(this.Logout);

        using var scope = this.BeginCallScope();

        this.MethodStart();

        ConnectionState previousState;
        ConnectionState currentState;
        using (this.stateLock.EnterScope())
        {
            this.Logout_i(e);
            (previousState, currentState) = this.RefreshConnectionState_i(e);
        }

        this.OnConnectionStateChanged(previousState, currentState);
    }

    #endregion

    #region Run Voicemeeter

    private void Run_p(App app, string executionPath)
    {
        var e = Utilities.BuildPath(executionPath);

        this.WrapperCall(nameof(this.wrapper.RunVoicemeeter), e, app: app);

        var response = this.wrapper.RunVoicemeeter(app);

        this.HandleRunResponse(response, app, e);
    }

    #region Run

    /// <inheritdoc cref="IRemote.Run(App)"/>
    internal void Run_i(App app, string executionPath)
    {
        var e = Utilities.BuildPath(executionPath);

        using var lk = this.stateLock.EnterScope();

        this.Run_p(app, e);

        if (app.IsVoicemeeter())
        {
            this.HandleStaleCache((app, RunResponse.Ok), e);
        }

        if (app is App.MacroButtons)
        {
            this.HandleStaleCache(RunResponse.Ok, e);
        }
    }

    /// <inheritdoc/>
    public void Run(App app)
    {
        using var scope = this.BeginCallScope();

        this.MethodStart(app: app);

        this.Run_i(app, nameof(this.Run));
    }

    /// <inheritdoc/>
    public void Run(Kind kind)
    {
        using var scope = this.BeginCallScope();

        this.MethodStart(kind: kind);

        this.Run_i(kind.ToApp(this.wrapper.Is64Bit), nameof(this.Run));
    }

    #endregion

    #region RunAsync

    /// <inheritdoc cref="IRemote.RunAsync(App, CancellationToken)"/>
    internal async Task<Result<RunResponse, App>> RunAsync_i(App app, string executionPath, CancellationToken cancellationToken)
    {
        var e = Utilities.BuildPath(executionPath);

        Result<RunResponse, App> result;
        ConnectionState previousState;
        ConnectionState currentState;
        using (await this.stateLock.EnterScopeAsync(cancellationToken))
        {
            if (app.IsVoicemeeter() && !this.connectionState.LoggedIn)
            {
                throw this.CannotWaitForEngine(app, e);
            }

            this.Run_p(app, e);

            result = app.IsVoicemeeter()
                    ? await this.WaitForEngine(e, cancellationToken)
                    : await this.WaitForRunning(app, e, cancellationToken);

            (previousState, currentState) = result.IsSuccess && (app.IsVoicemeeter() || app is App.MacroButtons)
                ? this.RefreshConnectionState_i(e)
                : (default, default);
        }

        this.OnConnectionStateChanged(previousState, currentState);

        return result;
    }

    /// <inheritdoc/>
    public async Task<Result<RunResponse, App>> RunAsync(App app, CancellationToken cancellationToken = default)
    {
        using var scope = this.BeginCallScope();

        this.MethodStart(app: app);

        return await this.RunAsync_i(app, nameof(this.RunAsync), cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Result<RunResponse, App>> RunAsync(Kind kind, CancellationToken cancellationToken = default)
    {
        using var scope = this.BeginCallScope();

        this.MethodStart(kind: kind);

        return await this.RunAsync_i(kind.ToApp(this.wrapper.Is64Bit), nameof(this.RunAsync), cancellationToken);
    }

    #endregion

    #endregion

    #region Helpers

    private async Task<Result<RunResponse, App>> WaitForEngine(string executionPath, CancellationToken cancellationToken)
    {
        var e = Utilities.BuildPath(executionPath);
        var target = Wrapper.VmName;

        this.WaitForRunningStart(target, e);

        VmVersion version;
        do
        {
            await Task.Delay(100, cancellationToken);

            version = this.GetVersion_i(e);
        }
        while (!version.IsValid());

        await this.WaitForEngineSettle(e, cancellationToken);

        (var app, var state) = this.GetVoicemeeterState_i(e);

        this.WaitForRunningDetected(target, e, state, version, app);

        return (state, app);
    }

    private async Task<Result<RunResponse, App>> WaitForRunning(App app, string executionPath, CancellationToken cancellationToken)
    {
        var e = Utilities.BuildPath(executionPath);
        var target = app.ToString();

        this.WaitForRunningStart(target, e, app: app);

        await this.wrapper.WaitForApplicationInputIdle(app, cancellationToken);

        if (app is App.MacroButtons
            && this.connectionState.ConnectedToVoicemeeter)
        {
            await this.WaitForButtonsSettle(e, cancellationToken);
        }

        var state = this.GetAppState_i(app, e);

        this.WaitForRunningDetected(target, e, state, app: app);

        return (state, app);
    }

    private async Task WaitForEngineSettle(string executionPath, CancellationToken cancellationToken)
    {
        var e = Utilities.BuildPath(executionPath);

        this.YieldForEngineSettle(Wrapper.VmName, e);
        Result<Response, bool> pDirty;
        Result<Response, bool> bDirty;
        do
        {
            await Task.Delay(50, cancellationToken);

            pDirty = this.ParamsDirty_i(e);
            bDirty = this.ButtonsDirty_i(e);
        }
        while (pDirty.IsFailure || pDirty
            || bDirty.IsFailure || bDirty);
    }

    private async Task WaitForButtonsSettle(string executionPath, CancellationToken cancellationToken)
    {
        var e = Utilities.BuildPath(executionPath);

        this.YieldForEngineSettle(nameof(App.MacroButtons), e);
        Result<Response, bool> dirty;
        do
        {
            await Task.Delay(50, cancellationToken);

            dirty = this.ButtonsDirty_i(e);
        }
        while (dirty.IsFailure || dirty);
    }

    #endregion
}
