// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using PBLivingston.VoicemeeterAPI.Types;
using PBLivingston.VoicemeeterAPI.EventManagement;
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

        RemoteDispatch.Login_Start(_logger);

        if (_lastState.LoggedIn)
            throw RemoteDispatch.Method_Error(_logger, LoginResponse.AlreadyLoggedIn);

        _loginStatus = _vmrApi.Login();

        var kind = Kind.None;
        VmVersion version = default;

        switch (_loginStatus)
        {
            case LoginResponse.Ok:
                if (!WaitForRunning(out kind, out version, timeoutMs, sleepMs))
                    throw RemoteDispatch.Method_Error(_logger, LoginResponse.Timeout);

                RemoteDispatch.Method_Success(_logger);
                break;

            case LoginResponse.VoicemeeterNotRunning:
                RemoteDispatch.Login_VmNotRunning(_logger);
                break;

            default:
                throw RemoteDispatch.Method_Error(_logger, _loginStatus);
        }

        ConnectionStateEventArgs state = new(_loginStatus, IsMacroButtonsRunning(), kind, version);

        if (state != _lastState)
            OnConnectionStateChanged(state);

        return _loginStatus;
    }

    #endregion

    #region Logout

    /// <inheritdoc cref="IRemote.Logout(int, int)"/>
    internal LoginResponse Logout(bool nested, int timeoutMs = 1000, int sleepMs = 100)
    {
        LoginGuard(requiredStatus: LoginResponse.Unknown);

        RemoteDispatch.Logout_Start(_logger);

        if (_loginStatus == LoginResponse.LoggedOut)
        {
            RemoteDispatch.Method_Error(_logger, LoginResponse.AlreadyLoggedOut);
            return _loginStatus;
        }

        (LoginResponse, bool) Attempt()
        {
            var result = _vmrApi.Logout();

            _loginStatus = result is LoginResponse.Ok
                ? LoginResponse.LoggedOut : LoginResponse.Unknown;

            if (_loginStatus == LoginResponse.LoggedOut)
            {
                RemoteDispatch.Method_Success(_logger, nameof(Logout));
                return (result, true);
            }

            return (result, false);
        }

        (LoginResponse response, bool success) = Retry(Attempt, timeoutMs, sleepMs);
        if (!success)
            RemoteDispatch.Logout_Timeout(_logger, response);

        if (!nested)
        {
            ConnectionStateEventArgs state = new(
                _loginStatus,
                _lastState.MacroButtonsIsRunning,
                _lastState.RunningKind,
                _lastState.RunningVersion
            );
            if (state != _lastState)
                OnConnectionStateChanged(state);
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

        var appAdjusted = app.BitAdjust();

        if (appAdjusted != app)
            GeneralDispatch.BitAdjust(_logger, app, appAdjusted);

        RemoteDispatch.Run_Start(_logger, appAdjusted);

        var result = _vmrApi.RunVoicemeeter((int)appAdjusted);

        if (result != RunResponse.Ok)
            throw RemoteDispatch.Run_Error(_logger, result, appAdjusted);

        if (app <= App.Potatox64)
        {
            if (!WaitForRunning(out Kind kind, out VmVersion version, timeoutMs, sleepMs))
                throw RemoteDispatch.Run_Error(_logger, RunResponse.Timeout, appAdjusted);

            ConnectionStateEventArgs state = new(_loginStatus, IsMacroButtonsRunning(), kind, version);
            if (state != _lastState)
                OnConnectionStateChanged(state);
        }

        if (app == App.MacroButtons)
        {
            RemoteDispatch.YieldForSettle(_logger);
            while (IsButtonsDirty()) Thread.Yield();

            var state = GetConnectionState();
            if (state != _lastState)
                OnConnectionStateChanged(state);
        }
    }

    /// <inheritdoc cref="IRemote.Run{T}(T, int, int)"/>
    public void Run(int app, int timeoutMs = 2000, int sleepMs = 100)
        => Run((App)app, timeoutMs, sleepMs);

    /// <inheritdoc cref="IRemote.Run{T}(T, int, int)"/>
    public void Run(Kind kind, int timeoutMs = 2000, int sleepMs = 100)
        => Run(kind.ToApp(), timeoutMs, sleepMs);

    /// <inheritdoc cref="IRemote.Run{T}(T, int, int)"/>
    public void Run(string app, int timeoutMs = 2000, int sleepMs = 100)
    {
        if (!Enum.TryParse(app, true, out App a))
            throw GeneralDispatch.CannotParseAsType(_logger, app, typeof(App), nameof(app));

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
                    throw GeneralDispatch.TypeNotSupported(_logger, typeof(T), nameof(app), SupportedTypes.RunTypes);
        }
    }

    #endregion

    #region Helpers

    private bool WaitForRunning(out Kind kind, out VmVersion version, int timeoutMs = 2000, int sleepMs = 100, [CallerMemberName] string methodName = "")
    {
        using var scope = BeginMethodScope(methodName);

        RemoteDispatch.WaitForRunning_Start(_logger);

        ((Kind, VmVersion), bool) Attempt()
        {
            var k = GetKind(true);
            var v = GetVersion(true);

            if (k.IsValid() && v.IsValid() && k == v.K)
            {
                RemoteDispatch.WaitForRunning_Detected(_logger, k, v);
                return ((k, v), true);
            }

            return ((k, v), false);
        }

        ((kind, version), bool success) = Retry(Attempt, timeoutMs, sleepMs);
        if (success)
        {
            RemoteDispatch.YieldForSettle(_logger);
            while (IsParamsDirty() || IsButtonsDirty()) Thread.Yield();
            return true;
        }

        RemoteDispatch.WaitForRunning_Timeout(_logger);
        return false;
    }

    #endregion
}
