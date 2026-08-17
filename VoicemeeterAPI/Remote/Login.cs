// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

public partial class Remote
{
    #region Login

    internal (ConnectionState previousState, ConnectionState currentState) Login_i(string executionPath)
    {
        var e = Utilities.BuildPath(executionPath);

        this.WrapperCall(nameof(this.wrapper.Login), e);

        var response = this.wrapper.Login();

        this.HandleLoginResponse(response, e);

        var states = this.GetConnectionState_i(e);

        this.HandleConnectionState(states.currentState, e);

        return states;
    }

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
            (previousState, currentState) = this.GetConnectionState_i(e);
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

    /// <inheritdoc cref="IRemote.Run{T}(T)"/>
    internal void Run_i(App app, string executionPath)
    {
        var e = Utilities.BuildPath(executionPath);

        using var lk = this.stateLock.EnterScope();

        this.Run_p(app, e);

        if (app.IsVoicemeeter())
        {
            this.HandleStaleCache(this.loginStatus, e);
        }

        if (app is App.MacroButtons)
        {
            this.HandleStaleCache(RunResponse.Ok, e);
        }
    }

    /// <inheritdoc cref="IRemote.Run{T}(T)"/>
    public void Run(App app)
    {
        using var scope = this.BeginCallScope();

        this.MethodStart(app: app);

        this.Run_i(app, nameof(this.Run));
    }

    /// <inheritdoc cref="IRemote.Run{T}(T)"/>
    public void Run(Kind kind)
    {
        using var scope = this.BeginCallScope();

        this.MethodStart(kind: kind);

        this.Run_i(kind.ToApp(this.wrapper.Is64Bit), nameof(this.Run));
    }

    /// <inheritdoc/>
    void IRemote.Run<T>(T app)
    {
        switch (app)
        {
            case App a:
                this.Run(a);
                break;

            case Kind k:
                this.Run(k);
                break;

            default:
                throw this.TypeNotSupported<T>(SupportedTypes.RunTypes, nameof(IRemote.Run));
        }
    }

    #endregion

    #region RunAsync

    /// <inheritdoc cref="IRemote.RunAsync{T}(T, CancellationToken)"/>
    internal async Task<Result<RunResponse, App>> RunAsync_i(App app, string executionPath, CancellationToken cancellationToken)
    {
        var e = Utilities.BuildPath(executionPath);

        Result<RunResponse, App> result;
        ConnectionState previousState;
        ConnectionState currentState;
        using (await this.stateLock.EnterScopeAsync(cancellationToken))
        {
            if (app.IsVoicemeeter() && !this.loginStatus.IsLoggedIn())
            {
                throw this.CannotWaitForEngine(app, e);
            }

            this.Run_p(app, e);

            result = app.IsVoicemeeter()
                    ? await this.WaitForEngine(e, cancellationToken)
                    : await this.WaitForRunning(app, e, cancellationToken);

            (previousState, currentState) = result.IsSuccess && (app.IsVoicemeeter() || app is App.MacroButtons)
                ? this.GetConnectionState_i(e)
                : (default, default);
        }

        this.OnConnectionStateChanged(previousState, currentState);

        return result;
    }

    /// <inheritdoc cref="IRemote.RunAsync{T}(T, CancellationToken)"/>
    public async Task<Result<RunResponse, App>> RunAsync(App app, CancellationToken cancellationToken = default)
    {
        using var scope = this.BeginCallScope();

        this.MethodStart(app: app);

        return await this.RunAsync_i(app, nameof(this.RunAsync), cancellationToken);
    }

    /// <inheritdoc cref="IRemote.RunAsync{T}(T, CancellationToken)"/>
    public async Task<Result<RunResponse, App>> RunAsync(Kind kind, CancellationToken cancellationToken = default)
    {
        using var scope = this.BeginCallScope();

        this.MethodStart(kind: kind);

        return await this.RunAsync_i(kind.ToApp(this.wrapper.Is64Bit), nameof(this.RunAsync), cancellationToken);
    }

    /// <inheritdoc/>
    async Task<Result<RunResponse, App>> IRemote.RunAsync<T>(T app, CancellationToken cancellationToken)
        => app switch
        {
            App a => await this.RunAsync(a, cancellationToken),
            Kind k => await this.RunAsync(k, cancellationToken),
            _ => throw this.TypeNotSupported<T>(SupportedTypes.RunTypes, nameof(IRemote.RunAsync))
        };

    #endregion

    #endregion

    #region Helpers

    private async Task<Result<RunResponse, App>> WaitForEngine(string executionPath, CancellationToken cancellationToken)
    {
        var e = Utilities.BuildPath(executionPath);
        var target = "Voicemeeter";

        this.WaitForRunningStart(target, e);

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(15));

        try
        {
            VmVersion version;
            do
            {
                await Task.Delay(100, cts.Token);

                version = this.GetVersion_i(e);
            }
            while (!version.IsValid());

            this.YieldForEngineSettle(target, e);
            Result<Response, bool> pDirty;
            Result<Response, bool> bDirty;
            do
            {
                await Task.Delay(50, cts.Token);

                pDirty = this.ParamsDirty_i(e);
                bDirty = this.ButtonsDirty_i(e);
            }
            while (pDirty.IsFailure || pDirty
                || bDirty.IsFailure || bDirty);

            (var app, var state) = this.GetVoicemeeterState_i(e);

            this.WaitForRunningDetected(target, e, state, version, app);

            return (state, app, true);
        }
        catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            this.OperationTimeout(ex, e);
            return RunResponse.Timeout;
        }
    }

    private async Task<Result<RunResponse, App>> WaitForRunning(App app, string executionPath, CancellationToken cancellationToken)
    {
        var e = Utilities.BuildPath(executionPath);
        var target = app.ToString();

        this.WaitForRunningStart(target, e, app: app);

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(15));

        try
        {
            await this.wrapper.WaitForApplicationInputIdle(app, cts.Token);

            if (app is App.MacroButtons
                && this.loginStatus == LoginResponse.Ok)
            {
                this.YieldForEngineSettle(target, e);
                Result<Response, bool> dirty;
                do
                {
                    await Task.Delay(50, cts.Token);

                    dirty = this.ButtonsDirty_i(e);
                }
                while (dirty.IsFailure || dirty);
            }

            var state = this.GetAppState_i(app, e);

            this.WaitForRunningDetected(target, e, state, app: app);

            return (state, app, true);
        }
        catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            this.OperationTimeout(ex, e, app: app);
            return (RunResponse.Timeout, app, false);
        }
    }

    #endregion
}
