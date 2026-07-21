// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

public partial class Remote
{
    #region Login

    private LoginResponse Login_p(string executionPath)
    {
        var e = Utilities.BuildPath(executionPath);

        this.WrapperCall(nameof(this.wrapper.Login), e);

        var result = this.wrapper.Login();

        return this.HandleResponse(result, e);
    }

    /// <inheritdoc/>
    public LoginResponse Login()
    {
        var e = nameof(this.Login);

        using var scope = this.BeginCallScope();

        this.MethodStart();

        LoginResponse result;
        ConnectionState previousState;
        ConnectionState currentState;
        using (this.stateLock.EnterScope())
        {
            result = this.Login_p(e);
            (previousState, currentState) = this.GetConnectionState_i(e);
        }

        this.OnConnectionStateChanged(previousState, currentState);

        return result;
    }

    #endregion

    #region Logout

    /// <inheritdoc cref="IRemote.Logout()"/>
    private LoginResponse Logout_i(string executionPath)
    {
        var e = Utilities.BuildPath(executionPath);

        this.WrapperCall(nameof(this.wrapper.Logout), e);

        var result = this.wrapper.Logout();

        return this.HandleResponse(result, e);
    }

    /// <inheritdoc/>
    public LoginResponse Logout()
    {
        var e = nameof(this.Logout);

        using var scope = this.BeginCallScope();

        this.MethodStart();

        ConnectionState previousState;
        ConnectionState currentState;
        using (this.stateLock.EnterScope())
        {
            this.loginStatus = this.Logout_i(e);
            (previousState, currentState) = this.GetConnectionState_i(e, true);
        }

        this.OnConnectionStateChanged(previousState, currentState);

        return this.loginStatus;
    }

    #endregion

    #region Run Voicemeeter

    private RunResponse Run_p(App app, string executionPath)
    {
        var e = Utilities.BuildPath(executionPath);

        var state = this.GetAppState_i(app, e);

        if (state is RunResponse.NotResponding)
        {
            this.AppUnexpectedState(e, new(app, state));
            return state;
        }

        this.WrapperCall(nameof(this.wrapper.RunVoicemeeter), e, new(app));

        var result = this.wrapper.RunVoicemeeter(app);

        return this.HandleResponse(app, result, e);
    }

    #region Run

    /// <inheritdoc cref="IRemote.Run{T}(T)"/>
    internal RunResponse Run_i(App app, string executionPath)
    {
        var e = Utilities.BuildPath(executionPath);

        using var lk = this.stateLock.EnterScope();

        var result = this.Run_p(app, e);

        if (result is RunResponse.NotResponding)
        {
            return result;
        }

        if (app.IsVoicemeeter() && this.LoggedIn)
        {
            this.HandleStaleCache(LoginResponse.Ok, e);
        }

        if (app is App.MacroButtons && this.LoggedIn)
        {
            this.HandleStaleCache(RunResponse.Ok, e);
        }

        return result;
    }

    /// <inheritdoc cref="IRemote.Run{T}(T)"/>
    internal RunResponse Run_i(Kind kind, string executionPath)
        => this.Run_i(kind.ToApp(this.wrapper.Is64Bit), executionPath);

    /// <inheritdoc cref="IRemote.Run{T}(T)"/>
    public RunResponse Run(App app)
    {
        using var scope = this.BeginCallScope();

        this.MethodStart();

        return this.Run_i(app, nameof(this.Run));
    }

    /// <inheritdoc cref="IRemote.Run{T}(T)"/>
    public RunResponse Run(Kind kind)
        => this.Run(kind.ToApp(this.wrapper.Is64Bit));

    /// <inheritdoc/>
    RunResponse IRemote.Run<T>(T app)
    {
        var e = nameof(IRemote.Run);

        using var scope = this.BeginCallScope();

        this.MethodStart();

        return app switch
        {
            App a => this.Run_i(a, e),
            Kind k => this.Run_i(k, e),
            _ => throw this.TypeNotSupported<T>(SupportedTypes.RunTypes, e, new(app))
        };
    }

    #endregion

    #region RunAsync

    /// <inheritdoc cref="IRemote.RunAsync{T}(T, CancellationToken)"/>
    internal async Task<RunResponse> RunAsync_i(App app, string executionPath, CancellationToken cancellationToken)
    {
        var e = Utilities.BuildPath(executionPath);

        RunResponse result;
        ConnectionState previousState;
        ConnectionState currentState;
        using (await this.stateLock.EnterScopeAsync(cancellationToken))
        {
            var r = this.Run_p(app, e);

            if (r is RunResponse.NotResponding)
            {
                return r;
            }

            var vm = app.IsVoicemeeter();
            var loggedOut = this.loginStatus >= LoginResponse.LoggedOut;

            if (vm && loggedOut)
            {
                this.CannotWaitForVoicemeeter(e);
                result = r;
            }
            else
            {
                result = vm
                    ? await this.WaitForVoicemeeter(e, cancellationToken)
                    : await this.WaitForRunning(app, e, cancellationToken);
            }

            (previousState, currentState) = this.GetConnectionState_i(e, loggedOut);
        }

        this.OnConnectionStateChanged(previousState, currentState);

        return result;
    }

    /// <inheritdoc cref="IRemote.RunAsync{T}(T, CancellationToken)"/>
    internal async Task<RunResponse> RunAsync_i(Kind kind, string executionPath, CancellationToken cancellationToken)
        => await this.RunAsync_i(kind.ToApp(this.wrapper.Is64Bit), executionPath, cancellationToken);

    /// <inheritdoc cref="IRemote.RunAsync{T}(T, CancellationToken)"/>
    public async Task<RunResponse> RunAsync(App app, CancellationToken cancellationToken = default)
    {
        using var scope = this.BeginCallScope();

        this.MethodStart();

        return await this.RunAsync_i(app, nameof(this.RunAsync), cancellationToken);
    }

    /// <inheritdoc cref="IRemote.RunAsync{T}(T, CancellationToken)"/>
    public async Task<RunResponse> RunAsync(Kind kind, CancellationToken cancellationToken = default)
        => await this.RunAsync(kind.ToApp(this.wrapper.Is64Bit), cancellationToken);

    /// <inheritdoc/>
    async Task<RunResponse> IRemote.RunAsync<T>(T app, CancellationToken cancellationToken)
    {
        var e = nameof(IRemote.RunAsync);

        using var scope = this.BeginCallScope();

        this.MethodStart();

        return app switch
        {
            App a => await this.RunAsync_i(a, e, cancellationToken),
            Kind k => await this.RunAsync_i(k, e, cancellationToken),
            _ => throw this.TypeNotSupported<T>(SupportedTypes.RunTypes, e, new(app))
        };
    }

    #endregion

    #endregion

    #region Helpers

    private async Task<RunResponse> WaitForVoicemeeter(string executionPath, CancellationToken cancellationToken)
    {
        var e = Utilities.BuildPath(executionPath);
        var target = "Voicemeeter";

        this.WaitForRunningStart(target, e);

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(15));

        try
        {
            LoginResponse login;
            VmVersion version;
            do
            {
                await Task.Delay(100, cts.Token);

                (login, version) = this.GetVersion_i(e);
            }
            while (!(login is LoginResponse.Ok && version.IsValid()));

            this.YieldForEngineSettle(target, e);
            bool pDirty;
            bool bDirty;
            do
            {
                await Task.Delay(50, cts.Token);

                pDirty = this.ParamsDirty_i(e);
                bDirty = this.ButtonsDirty_i(e);
            }
            while (pDirty || bDirty);

            var state = this.GetAppState_i(version.K.ToApp(this.wrapper.Is64Bit), e);

            if (this.wrapper.Is64Bit && state is RunResponse.NotRunning)
            {
                state = this.GetAppState_i(version.K.ToApp(false), e);
            }

            this.WaitForRunningDetected(target, e, new(version, state));

            return state;
        }
        catch (OperationCanceledException ex)
        {
            this.OperationCanceled(ex, e);
            return RunResponse.Timeout;
        }
    }

    private async Task<RunResponse> WaitForRunning(App app, string executionPath, CancellationToken cancellationToken)
    {
        var e = Utilities.BuildPath(executionPath);
        var target = app.ToString();
        LogArgs payload = new(app);

        this.WaitForRunningStart(target, e, payload);

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(15));

        try
        {
            Response idle;
            do
            {
                await Task.Delay(100, cts.Token);

                idle = this.wrapper.IsApplicationInputIdle(app);
            }
            while (idle is not Response.Ok);

            if (app is App.MacroButtons
                && this.loginStatus < LoginResponse.LoggedOut)
            {
                this.YieldForEngineSettle(target, e);
                bool dirty;
                do
                {
                    await Task.Delay(50, cts.Token);

                    dirty = this.ButtonsDirty_i(e);
                }
                while (dirty);
            }

            var state = this.GetAppState_i(app, e);

            this.WaitForRunningDetected(target, e, new(app, state));

            return state;
        }
        catch (OperationCanceledException ex)
        {
            this.OperationCanceled(ex, e, new(app));
            return RunResponse.Timeout;
        }
    }

    #endregion
}
