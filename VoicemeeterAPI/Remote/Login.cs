// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using PBLivingston.VoicemeeterAPI.Types;
using PBLivingston.VoicemeeterAPI.Utilities;
using System.Runtime.CompilerServices;

namespace PBLivingston.VoicemeeterAPI;

partial class Remote
{
    #region Login

    /// <inheritdoc/>
    public LoginResponse Login(int timeoutMs = 2000, int sleepMs = 100)
    {
        using var scope = BeginInstanceScope();

        LoginGuard(requiredStatus: LoginResponse.LoggedOut);

        On_Login_Start();

        if (_lastState.LoggedIn)
            throw On_Method_Error(LoginResponse.AlreadyLoggedIn);

        _loginStatus = _vmrApi.Login();

        var kind = Kind.None;
        VmVersion version = default;

        switch (_loginStatus)
        {
            case LoginResponse.Ok:
                if (!WaitForRunning(out kind, out version, timeoutMs, sleepMs))
                    throw On_Method_Error(LoginResponse.Timeout);

                On_Method_Success();
                break;

            case LoginResponse.VoicemeeterNotRunning:
                On_Login_VmNotRunning();
                break;

            default:
                throw On_Method_Error(_loginStatus);
        }

        ConnectionStateEventArgs state;
        using (BeginMethodScope())
        {
            state = new(_loginStatus, Query_ButtonsRunning(false), kind, version);
        }

        if (state != _lastState)
            On_ConnectionState_Changed(state);

        return _loginStatus;
    }

    #endregion

    #region Logout

    /// <inheritdoc cref="IRemote.Logout(int, int)"/>
    internal LoginResponse Logout(bool nested, int timeoutMs = 1000, int sleepMs = 100)
    {
        LoginGuard(requiredStatus: LoginResponse.Unknown);

        On_Logout_Start();

        if (_loginStatus == LoginResponse.LoggedOut)
        {
            On_Method_Error(LoginResponse.AlreadyLoggedOut);
            return _loginStatus;
        }

        (LoginResponse, bool) Attempt()
        {
            var result = _vmrApi.Logout();

            _loginStatus = result is LoginResponse.Ok
                ? LoginResponse.LoggedOut : LoginResponse.Unknown;

            if (_loginStatus == LoginResponse.LoggedOut)
            {
                On_Method_Success(methodName: nameof(Logout));
                return (result, true);
            }

            return (result, false);
        }

        (LoginResponse response, bool success) = Retry(Attempt, timeoutMs, sleepMs);
        if (!success)
            On_Logout_Timeout(response);

        if (!nested)
        {
            ConnectionStateEventArgs state = new(
                _loginStatus,
                _lastState.MacroButtonsIsRunning,
                _lastState.RunningKind,
                _lastState.RunningVersion
            );
            if (state != _lastState)
                On_ConnectionState_Changed(state);
        }

        return _loginStatus;
    }

    /// <inheritdoc/>
    public LoginResponse Logout(int timeoutMs = 1000, int sleepMs = 100)
    {
        using var scope = BeginInstanceScope();

        return Logout(false, timeoutMs, sleepMs);
    }

    #endregion

    #region Run Voicemeeter

    /// <inheritdoc cref="IRemote.Run{T}(T, int, int)"/>
    public void Run(App app, int timeoutMs = 2000, int sleepMs = 100)
    {
        using var scope = BeginInstanceScope();

        LoginGuard(requiredStatus: LoginResponse.VoicemeeterNotRunning);

        var appAdjusted = app.BitAdjust(_vmrApi.Is64Bit);

        if (appAdjusted != app)
            GeneralDispatch.On_BitAdjust(_logger, app, appAdjusted);

        On_Run_Start(appAdjusted);

        var result = _vmrApi.RunVoicemeeter((int)appAdjusted);

        if (result != RunResponse.Ok)
            throw On_Run_Error(result, appAdjusted);

        if (app <= App.Potatox64)
        {
            if (!WaitForRunning(out Kind kind, out VmVersion version, timeoutMs, sleepMs))
                throw On_Run_Error(RunResponse.Timeout, appAdjusted);

            ConnectionStateEventArgs state = new(_loginStatus, Query_ButtonsRunning(false), kind, version);
            if (state != _lastState)
                On_ConnectionState_Changed(state);
        }

        if (app == App.MacroButtons)
        {
            On_Method_YieldForSettle();
            while (Query_ButtonsDirty()) Thread.Yield();

            var state = GetConnectionState();
            if (state != _lastState)
                On_ConnectionState_Changed(state);
        }
    }

    /// <inheritdoc cref="IRemote.Run{T}(T, int, int)"/>
    public void Run(int app, int timeoutMs = 2000, int sleepMs = 100)
        => Run((App)app, timeoutMs, sleepMs);

    /// <inheritdoc cref="IRemote.Run{T}(T, int, int)"/>
    public void Run(Kind kind, int timeoutMs = 2000, int sleepMs = 100)
        => Run(kind.ToApp(_vmrApi.Is64Bit), timeoutMs, sleepMs);

    /// <inheritdoc cref="IRemote.Run{T}(T, int, int)"/>
    public void Run(string app, int timeoutMs = 2000, int sleepMs = 100)
    {
        if (!Enum.TryParse(app, true, out App a))
            throw GeneralDispatch.On_CannotParseAsType(_logger, app, typeof(App), nameof(app));

        Run(a, timeoutMs, sleepMs);
    }

    /// <inheritdoc/>
    void IRemote.Run<T>(T app, int timeoutMs, int sleepMs)
    {
        switch (app)
        {
            case int i:
                Run(i, timeoutMs, sleepMs);
                break;
            case App a:
                Run(a, timeoutMs, sleepMs);
                break;
            case Kind k:
                Run(k, timeoutMs, sleepMs);
                break;
            case string s:
                Run(s, timeoutMs, sleepMs);
                break;
            default:
                using (BeginInstanceScope())
                    throw GeneralDispatch.On_TypeNotSupported(_logger, typeof(T), nameof(app), SupportedTypes.RunTypes);
        }
    }

    #endregion

    #region Helpers

    private bool WaitForRunning(out Kind kind, out VmVersion version, int timeoutMs = 2000, int sleepMs = 100, [CallerMemberName] string methodName = "")
    {
        using var scope = BeginMethodScope(methodName);

        On_WaitForRunning_Start();

        ((Kind, VmVersion), bool) Attempt()
        {
            var k = GetInfo_Kind(true);
            var v = GetInfo_Version(true);

            if (k.IsValid() && v.IsValid() && k == v.K)
            {
                On_WaitForRunning_Detected(k, v);
                return ((k, v), true);
            }

            return ((k, v), false);
        }

        ((kind, version), bool success) = Retry(Attempt, timeoutMs, sleepMs);
        if (success)
        {
            On_Method_YieldForSettle();
            while (Query_ParamsDirty() || Query_ButtonsDirty()) Thread.Yield();
            return true;
        }

        On_WaitForRunning_Timeout();
        return false;
    }

    #endregion
}
