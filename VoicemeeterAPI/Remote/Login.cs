// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using PBLivingston.VoicemeeterAPI.Types;
using PBLivingston.VoicemeeterAPI.Utilities;
using System.Runtime.CompilerServices;

public partial class Remote
{
    #region Login

    /// <inheritdoc/>
    public LoginResponse Login(int timeoutMs = 2000, int sleepMs = 100)
    {
        using var scope = this.BeginInstanceScope();

        this.LoginGuard(requiredStatus: LoginResponse.LoggedOut);

        this.On_Login_Start();

        if (this.lastState.LoggedIn)
        {
            throw this.On_Method_Error(LoginResponse.AlreadyLoggedIn);
        }

        this.loginStatus = this.vmrApi.Login();

        var kind = Kind.None;
        VmVersion version = default;

        switch (this.loginStatus)
        {
            case LoginResponse.Ok:
                if (!this.WaitForRunning(out kind, out version, timeoutMs, sleepMs))
                {
                    throw this.On_Method_Error(LoginResponse.Timeout);
                }

                this.On_Method_Success();
                break;

            case LoginResponse.VoicemeeterNotRunning:
                this.On_Login_VmNotRunning();
                break;

            default:
                throw this.On_Method_Error(this.loginStatus);
        }

        ConnectionStateEventArgs state;
        using (this.BeginMethodScope())
        {
            state = new(
                this.loginStatus,
                this.Query_ButtonsRunning(false),
                kind,
                version
            );
        }

        if (state != this.lastState)
        {
            this.On_ConnectionState_Changed(state);
        }

        return this.loginStatus;
    }

    #endregion

    #region Logout

    /// <inheritdoc cref="IRemote.Logout(int, int)"/>
    internal LoginResponse Logout(bool nested, int timeoutMs = 1000, int sleepMs = 100)
    {
        this.LoginGuard(requiredStatus: LoginResponse.Unknown);

        this.On_Logout_Start();

        if (this.loginStatus == LoginResponse.LoggedOut)
        {
            this.On_Method_Error(LoginResponse.AlreadyLoggedOut);
            return this.loginStatus;
        }

        (LoginResponse, bool) Attempt()
        {
            var result = this.vmrApi.Logout();

            this.loginStatus = result is LoginResponse.Ok
                ? LoginResponse.LoggedOut
                : LoginResponse.Unknown;

            if (this.loginStatus == LoginResponse.LoggedOut)
            {
                this.On_Method_Success(methodName: nameof(Logout));
                return (result, true);
            }

            return (result, false);
        }

        (var response, var success) = this.Retry(Attempt, timeoutMs, sleepMs);
        if (!success)
        {
            this.On_Logout_Timeout(response);
        }

        if (!nested)
        {
            ConnectionStateEventArgs state = new(
                this.loginStatus,
                this.lastState.MacroButtonsIsRunning,
                this.lastState.RunningKind,
                this.lastState.RunningVersion
            );
            if (state != this.lastState)
            {
                this.On_ConnectionState_Changed(state);
            }
        }

        return this.loginStatus;
    }

    /// <inheritdoc/>
    public LoginResponse Logout(int timeoutMs = 1000, int sleepMs = 100)
    {
        using var scope = this.BeginInstanceScope();

        return this.Logout(false, timeoutMs, sleepMs);
    }

    #endregion

    #region Run Voicemeeter

    /// <inheritdoc cref="IRemote.Run{T}(T, int, int)"/>
    public void Run(App app, int timeoutMs = 2000, int sleepMs = 100)
    {
        using var scope = this.BeginInstanceScope();

        this.LoginGuard(requiredStatus: LoginResponse.VoicemeeterNotRunning);

        var appAdjusted = app.BitAdjust(this.vmrApi.Is64Bit);

        if (appAdjusted != app)
        {
            GeneralDispatch.On_BitAdjust(this.logger, app, appAdjusted);
        }

        this.On_Run_Start(appAdjusted);

        var result = this.vmrApi.RunVoicemeeter((int)appAdjusted);

        if (result != RunResponse.Ok)
        {
            throw this.On_Run_Error(result, appAdjusted);
        }

        if (app <= App.Potatox64)
        {
            if (!this.WaitForRunning(out var kind, out var version, timeoutMs, sleepMs))
            {
                throw this.On_Run_Error(RunResponse.Timeout, appAdjusted);
            }

            ConnectionStateEventArgs state = new(
                this.loginStatus,
                this.Query_ButtonsRunning(false),
                kind,
                version
            );
            if (state != this.lastState)
            {
                this.On_ConnectionState_Changed(state);
            }
        }

        if (app == App.MacroButtons)
        {
            this.On_Method_YieldForSettle();
            while (this.Query_ButtonsDirty())
            {
                Thread.Yield();
            }

            var state = this.GetConnectionState();
            if (state != this.lastState)
            {
                this.On_ConnectionState_Changed(state);
            }
        }
    }

    /// <inheritdoc cref="IRemote.Run{T}(T, int, int)"/>
    public void Run(int app, int timeoutMs = 2000, int sleepMs = 100)
        => this.Run((App)app, timeoutMs, sleepMs);

    /// <inheritdoc cref="IRemote.Run{T}(T, int, int)"/>
    public void Run(Kind kind, int timeoutMs = 2000, int sleepMs = 100)
        => this.Run(kind.ToApp(this.vmrApi.Is64Bit), timeoutMs, sleepMs);

    /// <inheritdoc cref="IRemote.Run{T}(T, int, int)"/>
    public void Run(string app, int timeoutMs = 2000, int sleepMs = 100)
    {
        if (!Enum.TryParse(app, true, out App a))
        {
            throw GeneralDispatch.On_CannotParseAsType(this.logger, app, typeof(App), nameof(app));
        }

        this.Run(a, timeoutMs, sleepMs);
    }

    /// <inheritdoc/>
    void IRemote.Run<T>(T app, int timeoutMs, int sleepMs)
    {
        switch (app)
        {
            case int i:
                this.Run(i, timeoutMs, sleepMs);
                break;

            case App a:
                this.Run(a, timeoutMs, sleepMs);
                break;

            case Kind k:
                this.Run(k, timeoutMs, sleepMs);
                break;

            case string s:
                this.Run(s, timeoutMs, sleepMs);
                break;

            default:
                using (this.BeginInstanceScope())
                {
                    throw GeneralDispatch.On_TypeNotSupported(this.logger, typeof(T), nameof(app), SupportedTypes.RunTypes);
                }
        }
    }

    #endregion

    #region Helpers

    private bool WaitForRunning(out Kind kind, out VmVersion version, int timeoutMs = 2000, int sleepMs = 100, [CallerMemberName] string methodName = "")
    {
        using var scope = this.BeginMethodScope(methodName);

        this.On_WaitForRunning_Start();

        ((Kind, VmVersion), bool) Attempt()
        {
            var k = this.GetInfo_Kind(true);
            var v = this.GetInfo_Version(true);

            if (k.IsValid() && v.IsValid() && k == v.K)
            {
                this.On_WaitForRunning_Detected(k, v);
                return ((k, v), true);
            }

            return ((k, v), false);
        }

        ((kind, version), var success) = this.Retry(Attempt, timeoutMs, sleepMs);
        if (success)
        {
            this.On_Method_YieldForSettle();
            while (this.Query_ParamsDirty() || this.Query_ButtonsDirty())
            {
                Thread.Yield();
            }

            return true;
        }

        this.On_WaitForRunning_Timeout();
        return false;
    }

    #endregion
}
