// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Threading;
using PBLivingston.VoicemeeterAPI.Types;
using PBLivingston.VoicemeeterAPI.Messages;
using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Utilities;

namespace PBLivingston.VoicemeeterAPI;

partial class Remote
{
    #region Login

    /// <inheritdoc/>
    public void Login(int timeoutMs = 2000, int sleepMs = 100)
    {
        LoginGuard(requiredStatus: LoginResponse.LoggedOut);

        RemoteInfo.Write("Logging in...");

        if (LoggedIn)
            throw new RemoteException($"Already logged in - LoginStatus: {LoginStatus}");

        LoginStatus = _vmrApi.Login();

        switch (LoginStatus)
        {
            case LoginResponse.Ok:
                if (!WaitForRunning(timeoutMs, sleepMs))
                    throw new LoginException(LoginResponse.Timeout);

                RemoteInfo.Write("Login successful");
                return;

            case LoginResponse.VoicemeeterNotRunning:
                RemoteWarning.Write("Login successful but Voicemeeter is not running");
                return;

            default:
                throw new LoginException(LoginStatus);
        }
    }

    #endregion

    #region Logout

    /// <inheritdoc/>
    public void Logout(int timeoutMs = 1000, int sleepMs = 100)
    {
        LoginGuard(requiredStatus: LoginResponse.Unknown);

        RemoteInfo.Write("Logging out...");

        if (LoginStatus == LoginResponse.LoggedOut)
            throw new RemoteException("Already logged out");

        bool Attempt(out LoginResponse response)
        {
            response = _vmrApi.Logout();

            LoginStatus = response is LoginResponse.Ok
                ? LoginResponse.LoggedOut : LoginResponse.Unknown;

            if (LoginStatus == LoginResponse.LoggedOut)
            {
                RemoteInfo.Write("Logout successful");
                return true;
            }

            return false;
        }

        var result = LoginResponse.Unknown;
        if (Retry(() => Attempt(out result), timeoutMs, sleepMs)) return;

        RemoteWarning.Write($"Logout timed out; last result: {result}");
    }

    #endregion

    #region Run Voicemeeter

    /// <inheritdoc cref="IRemote.Run{T}(T, int, int)"/>
    public void Run(int app, int timeoutMs = 2000, int sleepMs = 100)
    {
        LoginGuard(requiredStatus: LoginResponse.VoicemeeterNotRunning);

        app = AppUtils.BitAdjust(app);

        RemoteInfo.Write($"Running application: {(App)app}...");

        var result = _vmrApi.RunVoicemeeter(app);

        if (result != RunResponse.Ok) throw new RunException(result, (App)app);

        if (app <= (int)App.Potatox64 && !WaitForRunning(timeoutMs, sleepMs))
            throw new RunException(RunResponse.Timeout, (App)app);

        if (app == (int)App.MacroButtons)
            while (ButtonsDirty()) Thread.Yield();
    }

    /// <inheritdoc cref="IRemote.Run{T}(T, int, int)"/>
    public void Run(App app, int timeoutMs = 2000, int sleepMs = 100)
        => Run((int)app, timeoutMs, sleepMs);

    /// <inheritdoc cref="IRemote.Run{T}(T, int, int)"/>
    public void Run(Kind kind, int timeoutMs = 2000, int sleepMs = 100)
        => Run((int)kind, timeoutMs, sleepMs);

    /// <inheritdoc cref="IRemote.Run{T}(T, int, int)"/>
    public void Run(string app, int timeoutMs = 2000, int sleepMs = 100)
    {
        if (!Enum.TryParse(app, true, out App a))
            throw new ArgumentException($"Invalid app: {app}", nameof(app));

        Run((int)a, timeoutMs, sleepMs);
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
                throw new TypeNotSupportedException<T>(nameof(app), SupportedTypes.RunTypes);
        }
    }

    #endregion

    #region Helpers

    private bool WaitForRunning(int timeoutMs = 2000, int sleepMs = 100)
    {
        RemoteInfo.Write("Waiting for running Voicemeeter...");

        bool Attempt()
        {
            var kind = GetKind();
            var version = GetVersion();

            if (kind.IsValid() && version.IsValid())
            {
                RemoteInfo.Write($"Voicemeeter {kind} v{version} is running");
                return true;
            }

            return false;
        }

        if (Retry(Attempt, timeoutMs, sleepMs))
        {
            while (ParamsDirty() || ButtonsDirty()) Thread.Yield();
            return true;
        }

        RemoteWarning.Write("Timed out waiting for Voicemeeter");
        return false;
    }

    #endregion
}
