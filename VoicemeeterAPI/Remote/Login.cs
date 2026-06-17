// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using PBLivingston.VoicemeeterAPI.Types;
using PBLivingston.VoicemeeterAPI.Utilities;
using System.Runtime.CompilerServices;

public partial class Remote
{
    #region Login

    private LoginResponse Login_p()
    {
        this.On_Login_Start();

        var result = this.wrapper.Login();

        if (result < LoginResponse.Ok)
        {
            throw this.On_Method_Error(result);
        }

        return result;
    }

    /// <inheritdoc/>
    public LoginResponse Login()
    {
        using var scope = this.BeginInstanceScope();
        using var lk = this.stateLock.EnterScope();

        var result = this.Login_p();

        if (result is LoginResponse.VoicemeeterNotRunning)
        {
            this.On_Login_VmNotRunning();
        }
        else
        {
            this.On_Method_Success();
        }

        ConnectionState state;
        using (this.BeginMethodScope())
        {
            (this.loginStatus, state) = this.GetConnectionState_i();
        }

        this.On_ConnectionState_Changed(state);

        if (!state.ButtonsRunning)
        {
            this.On_Login_MbNotRunning();
        }

        return this.loginStatus;
    }

    /// <inheritdoc/>
    public async Task<LoginResponse> LoginAsync(CancellationToken cancellationToken = default)
    {
        using var scope = this.BeginInstanceScope();
        using var lk = await this.stateLock.EnterScopeAsync(cancellationToken);

        var result = this.Login_p();

        if (result is LoginResponse.VoicemeeterNotRunning)
        {
            this.On_Login_VmNotRunning();
        }
        else if (await this.WaitForVoicemeeter(cancellationToken) is RunResponse.Timeout)
        {
            throw this.On_Method_Error(LoginResponse.Timeout);
        }
        else
        {
            this.On_Method_Success();
        }

        ConnectionState state;
        using (this.BeginMethodScope())
        {
            (this.loginStatus, state) = this.GetConnectionState_i();
        }

        this.On_ConnectionState_Changed(state);

        if (!state.ButtonsRunning)
        {
            this.On_Login_MbNotRunning();
        }

        return this.loginStatus;
    }

    #endregion

    #region Logout

    /// <inheritdoc cref="IRemote.Logout()"/>
    private LoginResponse Logout_i()
    {
        this.On_Logout_Start();

        var result = this.wrapper.Logout();

        LoginResponse login;
        if (result == LoginResponse.Ok)
        {
            login = LoginResponse.LoggedOut;
            this.On_Method_Success();
        }
        else
        {
            login = LoginResponse.Unknown;
            this.On_Method_Error(result);
        }

        return login;
    }

    /// <inheritdoc/>
    public LoginResponse Logout()
    {
        using var scope = this.BeginInstanceScope();
        using var lk = this.stateLock.EnterScope();

        if (this.loginStatus == LoginResponse.LoggedOut)
        {
            this.On_Method_Error(LoginResponse.AlreadyLoggedOut);
        }
        else
        {
            this.loginStatus = this.Logout_i();

            ConnectionState state = new(
                this.loginStatus,
                this.lastConnectionState.ButtonsState,
                this.lastConnectionState.RunningKind,
                this.lastConnectionState.RunningVersion
            );
            this.On_ConnectionState_Changed(state);
        }

        return this.loginStatus;
    }

    #endregion

    #region Run Voicemeeter

    private App Run_p(App app)
    {
        App a;
        if (app.IsVoicemeeter())
        {
            a = app.BitAdjust(this.wrapper.Is64Bit);

            GeneralDispatch.On_BitAdjust(this.logger, app, a);
        }
        else
        {
            RunResponse state;
            using (this.BeginMethodScope())
            {
                state = this.GetAppState_i(app, false);
            }

            if (state is RunResponse.NotResponding)
            {
                throw this.On_Run_Error(state, app);
            }

            a = app;
        }

        this.On_Run_Start(a);

        var result = this.wrapper.RunVoicemeeter((int)a);

        if (result != RunResponse.Ok)
        {
            throw this.On_Run_Error(result, a);
        }

        return a;
    }

    #region Run

    /// <inheritdoc cref="IRemote.Run{T}(T)"/>
    public void Run(App app)
    {
        using var scope = this.BeginInstanceScope();

        var a = this.Run_p(app);

        this.On_Method_Success();

        if (a.IsVoicemeeter() && this.LoggedIn)
        {
            using var lk = this.stateLock.EnterScope();

            this.On_ConnectionState_StateMismatch(LoginResponse.Ok);
        }

        if (a is App.MacroButtons && this.LoggedIn)
        {
            using var lk = this.stateLock.EnterScope();

            this.On_ConnectionState_StateMismatch(RunResponse.Ok);
        }
    }

    /// <inheritdoc cref="IRemote.Run{T}(T)"/>
    public void Run(int app)
        => this.Run((App)app);

    /// <inheritdoc cref="IRemote.Run{T}(T)"/>
    public void Run(Kind kind)
        => this.Run(kind.ToApp(this.wrapper.Is64Bit));

    /// <inheritdoc cref="IRemote.Run{T}(T)"/>
    public void Run(string app)
    {
        if (!Enum.TryParse(app, true, out App a))
        {
            throw GeneralDispatch.On_CannotParseAsType(this.logger, app, typeof(App), nameof(app));
        }

        this.Run(a);
    }

    /// <inheritdoc/>
    void IRemote.Run<T>(T app)
    {
        switch (app)
        {
            case App a:
                this.Run(a);
                break;

            case int i:
                this.Run(i);
                break;

            case Kind k:
                this.Run(k);
                break;

            case string s:
                this.Run(s);
                break;

            default:
                using (this.BeginInstanceScope())
                {
                    throw GeneralDispatch.On_TypeNotSupported(this.logger, typeof(T), nameof(app), SupportedTypes.RunTypes);
                }
        }
    }

    #endregion

    #region RunAsync

    /// <inheritdoc cref="IRemote.RunAsync{T}(T, CancellationToken)"/>
    public async Task<RunResponse> RunAsync(App app, CancellationToken cancellationToken = default)
    {
        using var scope = this.BeginInstanceScope();

        App a;
        RunResponse result;
        if (app is App.MacroButtons || app.IsVoicemeeter())
        {
            using var lk = await this.stateLock.EnterScopeAsync(cancellationToken);

            var loggedIn = this.loginStatus < LoginResponse.LoggedOut;

            a = this.Run_p(app);

            result = a is App.MacroButtons
                ? await this.WaitForRunning(a, cancellationToken)
                : loggedIn
                    ? await this.WaitForVoicemeeter(cancellationToken)
                    : this.On_Run_LoggedOut();

            if (result < RunResponse.Ok)
            {
                throw this.On_Run_Error(result, a);
            }

            if (loggedIn)
            {
                ConnectionState state;
                using (this.BeginMethodScope())
                {
                    (this.loginStatus, state) = this.GetConnectionState_i();
                }

                if ((a is App.MacroButtons && !state.ButtonsRunning)
                    || (a.IsVoicemeeter() && !state.Connected))
                {
                    throw this.On_Run_Error(RunResponse.Error, a);
                }

                this.On_ConnectionState_Changed(state);
            }
        }
        else
        {
            a = this.Run_p(app);

            result = await this.WaitForRunning(a, cancellationToken);

            if (result < RunResponse.Ok)
            {
                throw this.On_Run_Error(result, a);
            }
        }

        this.On_Method_Success();

        return result;
    }

    /// <inheritdoc cref="IRemote.RunAsync{T}(T, CancellationToken)"/>
    public async Task<RunResponse> RunAsync(int app, CancellationToken cancellationToken = default)
        => await this.RunAsync((App)app, cancellationToken);

    /// <inheritdoc cref="IRemote.RunAsync{T}(T, CancellationToken)"/>
    public async Task<RunResponse> RunAsync(Kind kind, CancellationToken cancellationToken = default)
        => await this.RunAsync(kind.ToApp(this.wrapper.Is64Bit), cancellationToken);

    /// <inheritdoc cref="IRemote.RunAsync{T}(T, CancellationToken)"/>
    public async Task<RunResponse> RunAsync(string app, CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse(app, true, out App a))
        {
            throw GeneralDispatch.On_CannotParseAsType(this.logger, app, typeof(App), nameof(app));
        }

        return await this.RunAsync(a, cancellationToken);
    }

    /// <inheritdoc/>
    async Task<RunResponse> IRemote.RunAsync<T>(T app, CancellationToken cancellationToken)
    {
        switch (app)
        {
            case App a:
                return await this.RunAsync(a, cancellationToken);

            case int i:
                return await this.RunAsync(i, cancellationToken);

            case Kind k:
                return await this.RunAsync(k, cancellationToken);

            case string s:
                return await this.RunAsync(s, cancellationToken);

            default:
                using (this.BeginInstanceScope())
                {
                    throw GeneralDispatch.On_TypeNotSupported(this.logger, typeof(T), nameof(app), SupportedTypes.RunTypes);
                }
        }
    }

    #endregion

    #endregion

    #region Helpers

    private async Task<RunResponse> WaitForVoicemeeter(CancellationToken cancellationToken = default, [CallerMemberName] string methodName = "")
    {
        using var scope = this.BeginMethodScope(methodName);

        this.On_WaitForVoicemeeter_Start();

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(15));

        try
        {
            LoginResponse login;
            LoginResponse l;
            Kind k;
            VmVersion v;
            do
            {
                await Task.Delay(100, cts.Token);

                using var s = this.BeginMethodScope();

                (login, k) = this.GetKind_i(true);
                (l, v) = this.GetVersion_i(true);
            }
            while (!(login is LoginResponse.Ok && login == l
                && k.IsValid() && v.IsValid() && k == v.K));

            this.On_WaitForVoicemeeter_Detected(k, v);

            this.On_Method_YieldForSettle();
            bool pDirty;
            bool bDirty;
            do
            {
                await Task.Delay(50, cts.Token);

                using var s = this.BeginMethodScope();

                pDirty = this.ParamsDirty_i();
                bDirty = this.ButtonsDirty_i();
            }
            while (pDirty || bDirty);

            using var ms = this.BeginMethodScope();

            return this.GetAppState_i(k.ToApp(this.wrapper.Is64Bit), false);
        }
        catch (OperationCanceledException)
        {
            this.On_WaitForVoicemeeter_Timeout();
            return RunResponse.Timeout;
        }
    }

    private async Task<RunResponse> WaitForRunning(App app, CancellationToken cancellationToken = default, [CallerMemberName] string methodName = "")
    {
        using var scope = this.BeginMethodScope(methodName);

        this.On_WaitForRunning_Start(app);

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(15));

        try
        {
            Response idle;
            do
            {
                await Task.Delay(100, cts.Token);

                idle = this.wrapper.IsApplicationInputIdle(app);

                if (idle is Response.Error)
                {
                    return RunResponse.Error;
                }
            }
            while (idle is not Response.Ok);

            if (app is App.MacroButtons
                && this.loginStatus < LoginResponse.LoggedOut)
            {
                this.On_Method_YieldForSettle();
                bool dirty;
                do
                {
                    await Task.Delay(50, cts.Token);

                    using var s = this.BeginMethodScope();

                    dirty = this.ButtonsDirty_i();
                }
                while (dirty);
            }

            RunResponse state;
            using (this.BeginMethodScope())
            {
                state = this.GetAppState_i(app, false);
            }

            this.On_WaitForRunning_Detected(app, state);

            return state;
        }
        catch (OperationCanceledException)
        {
            this.On_WaitForRunning_Timeout(app);
            return RunResponse.Timeout;
        }
    }

    #endregion
}
