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
        this.LoginGuard(requiredStatus: LoginResponse.LoggedOut);

        if (this.loginStatus < LoginResponse.LoggedOut)
        {
            throw this.On_Guard_AccessDenied(this.loginStatus);
        }

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

        this.loginStatus = this.Login_p();

        if (this.loginStatus is LoginResponse.VoicemeeterNotRunning)
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
            state = this.GetConnectionState_i();
        }

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

        this.loginStatus = this.Login_p();

        if (this.loginStatus is LoginResponse.VoicemeeterNotRunning)
        {
            this.On_Login_VmNotRunning();
        }
        else if (await this.WaitForVoicemeeter(cancellationToken))
        {
            this.On_Method_Success();
        }
        else
        {
            throw this.On_Method_Error(LoginResponse.Timeout);
        }

        ConnectionState state;
        using (this.BeginMethodScope())
        {
            state = this.GetConnectionState_i();
        }

        if (!state.ButtonsRunning)
        {
            this.On_Login_MbNotRunning();
        }

        return this.loginStatus;
    }

    #endregion

    #region Logout

    /// <inheritdoc cref="IRemote.Logout()"/>
    internal LoginResponse Logout_i(bool disposing)
    {
        this.LoginGuard(requiredStatus: LoginResponse.Unknown);

        this.On_Logout_Start();

        if (this.loginStatus == LoginResponse.LoggedOut)
        {
            this.On_Method_Error(LoginResponse.AlreadyLoggedOut);
            return this.loginStatus;
        }

        var result = this.wrapper.Logout();

        if (result == LoginResponse.Ok)
        {
            this.loginStatus = LoginResponse.LoggedOut;
            this.On_Method_Success();
        }
        else
        {
            this.loginStatus = LoginResponse.Unknown;
            this.On_Method_Error(result);
        }

        if (!disposing)
        {
            ConnectionState state = new(
                this.loginStatus,
                this.LastConnectionState.ButtonsState,
                this.LastConnectionState.RunningKind,
                this.LastConnectionState.RunningVersion
            );
            this.On_ConnectionState_Changed(state);
        }

        return this.loginStatus;
    }

    /// <inheritdoc/>
    public LoginResponse Logout()
    {
        using var scope = this.BeginInstanceScope();

        return this.Logout_i(false);
    }

    #endregion

    #region Run Voicemeeter

    private App Run_p(App app)
    {
        this.LoginGuard(requiredStatus: LoginResponse.VoicemeeterNotRunning);

        if (!app.IsValid())
        {
            throw this.On_Run_Error(RunResponse.UnknownApp, app);
        }

        var appAdjusted = app.BitAdjust(this.wrapper.Is64Bit);

        GeneralDispatch.On_BitAdjust(this.logger, app, appAdjusted);

        this.On_Run_Start(appAdjusted);

        if (!appAdjusted.IsVoicemeeter())
        {
            RunResponse state;
            using (this.BeginMethodScope())
            {
                state = this.GetInfo_AppState(appAdjusted, true);
            }

            if (state is RunResponse.Ok)
            {
                return appAdjusted;
            }

            if (state is RunResponse.NotResponding)
            {
                throw this.On_Run_Error(state, appAdjusted);
            }
        }

        var result = this.wrapper.RunVoicemeeter((int)appAdjusted);

        if (result != RunResponse.Ok)
        {
            throw this.On_Run_Error(result, appAdjusted);
        }

        return appAdjusted;
    }

    #region Run

    /// <inheritdoc cref="IRemote.Run{T}(T)"/>
    public void Run(App app)
    {
        using var scope = this.BeginInstanceScope();

        var a = this.Run_p(app);

        this.On_Method_Success();

        if (a.IsVoicemeeter())
        {
            this.On_ConnectionState_StateMismatch(LoginResponse.Ok);
        }

        if (a is App.MacroButtons)
        {
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

        var a = this.Run_p(app);

        var success = a.IsVoicemeeter()
            ? await this.WaitForVoicemeeter(cancellationToken)
            : await this.WaitForRunning(a, cancellationToken);

        if (!success)
        {
            throw this.On_Run_Error(RunResponse.Timeout, a);
        }

        this.On_Method_Success();

        return this.GetInfo_AppState(a, false);
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

    private async Task<bool> WaitForVoicemeeter(CancellationToken cancellationToken = default, [CallerMemberName] string methodName = "")
    {
        using var scope = this.BeginMethodScope(methodName);

        this.On_WaitForVoicemeeter_Start();

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(15));

        try
        {
            Kind k;
            VmVersion v;
            do
            {
                await Task.Delay(100, cts.Token);

                using (this.BeginMethodScope())
                {
                    k = this.GetInfo_Kind(true);
                    v = this.GetInfo_Version(true);
                }
            }
            while (!(k.IsValid() && v.IsValid() && k == v.K));

            this.On_WaitForVoicemeeter_Detected(k, v);

            this.On_Method_YieldForSettle();
            bool pDirty;
            bool bDirty;
            do
            {
                await Task.Delay(50, cts.Token);

                using (this.BeginMethodScope())
                {
                    pDirty = this.Query_ParamsDirty();
                    bDirty = this.Query_ButtonsDirty();
                }
            }
            while (pDirty || bDirty);

            return true;
        }
        catch (OperationCanceledException)
        {
            this.On_WaitForVoicemeeter_Timeout();
            return false;
        }
    }

    private async Task<bool> WaitForRunning(App app, CancellationToken cancellationToken = default, [CallerMemberName] string methodName = "")
    {
        using var scope = this.BeginMethodScope(methodName);

        this.On_WaitForRunning_Start(app);

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(15));

        try
        {
            RunResponse state;
            do
            {
                await Task.Delay(100, cts.Token);

                using (this.BeginMethodScope())
                {
                    state = this.GetInfo_AppState(app, true);
                }
            }
            while (!(state is RunResponse.Ok or RunResponse.Hidden
                        && this.wrapper.IsApplicationInputIdle(app) is Response.Ok));

            this.On_WaitForRunning_Detected(app, state);

            if (app is App.MacroButtons)
            {
                this.On_Method_YieldForSettle();
                bool dirty;
                do
                {
                    await Task.Delay(50, cts.Token);

                    using (this.BeginMethodScope())
                    {
                        dirty = this.Query_ButtonsDirty();
                    }
                }
                while (dirty);
            }

            return true;
        }
        catch (OperationCanceledException)
        {
            this.On_WaitForRunning_Timeout(app);
            return false;
        }
    }

    #endregion
}
