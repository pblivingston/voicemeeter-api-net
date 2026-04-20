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

        if (LoggedIn)
            RemoteDispatch.Method_Error(_logger, LoginResponse.AlreadyLoggedIn);

        LoginStatus = _vmrApi.Login();

        switch (LoginStatus)
        {
            case LoginResponse.Ok:
                if (!WaitForRunning(timeoutMs, sleepMs))
                    RemoteDispatch.Method_Error(_logger, LoginResponse.Timeout);

                RemoteDispatch.Method_Success(_logger);
                break;

            case LoginResponse.VoicemeeterNotRunning:
                RemoteDispatch.Login_VmNotRunning(_logger);
                break;

            default:
                RemoteDispatch.Method_Error(_logger, LoginStatus);
                break;
        }

        var currentState = ConnectionState;
        if (_lastState != currentState)
            OnConnectionStateChanged(currentState);

        return LoginStatus;
    }

    #endregion

    #region Logout

    /// <inheritdoc cref="IRemote.Logout(int, int)"/>
    internal LoginResponse Logout(bool nested, int timeoutMs = 1000, int sleepMs = 100)
    {
        LoginGuard(requiredStatus: LoginResponse.Unknown);

        RemoteDispatch.Logout_Start(_logger);

        if (LoginStatus == LoginResponse.LoggedOut)
            RemoteDispatch.Logout_AlreadyLoggedOut(_logger);

        bool Attempt(out LoginResponse response)
        {
            response = _vmrApi.Logout();

            LoginStatus = response is LoginResponse.Ok
                ? LoginResponse.LoggedOut : LoginResponse.Unknown;

            if (LoginStatus == LoginResponse.LoggedOut)
            {
                RemoteDispatch.Method_Success(_logger, nameof(Logout));
                return true;
            }

            return false;
        }

        var result = LoginResponse.Unknown;
        if (!Retry(() => Attempt(out result), timeoutMs, sleepMs))
            RemoteDispatch.Logout_Timeout(_logger, result);

        var currentState = ConnectionState;
        if (_lastState != currentState)
            OnConnectionStateChanged(currentState);

        return LoginStatus;
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
            RemoteDispatch.Run_Error(_logger, result, appAdjusted);

        if (app <= App.Potatox64)
        {
            if (!WaitForRunning(timeoutMs, sleepMs))
                RemoteDispatch.Run_Error(_logger, RunResponse.Timeout, appAdjusted);

            var currentState = ConnectionState;
            if (_lastState != currentState)
                OnConnectionStateChanged(currentState);
        }

        if (app == App.MacroButtons)
        {
            RemoteDispatch.YieldForSettle(_logger);
            while (IsButtonsDirty()) Thread.Yield();
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
            GeneralDispatch.CannotParseAsType(_logger, app, typeof(App), nameof(app));

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
                {
                    GeneralDispatch.TypeNotSupported(_logger, typeof(T), nameof(app), SupportedTypes.RunTypes);
                }
                break;
        }
    }

    #endregion

    #region Helpers

    private bool WaitForRunning(int timeoutMs = 2000, int sleepMs = 100, [CallerMemberName] string methodName = "")
    {
        using var scope = BeginMethodScope(methodName);

        RemoteDispatch.WaitForRunning_Start(_logger);

        bool Attempt()
        {
            var kind = GetKind(true);
            var version = GetVersion(true);

            if (kind.IsValid() && version.IsValid() && kind == version.K)
            {
                RemoteDispatch.WaitForRunning_Detected(_logger, kind, version);
                return true;
            }

            return false;
        }

        if (Retry(Attempt, timeoutMs, sleepMs))
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
