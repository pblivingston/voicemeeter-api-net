// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

public partial class Remote
{
    /// <summary>
    ///   Accesses <see cref="connectionState"/> - must be within <see cref="stateLock"/> scope!
    /// </summary>
    /// <param name="currentLoginStatus"></param>
    /// <param name="executionPath"></param>
    /// <param name="methodName"></param>
    private void HandleStaleCache(
        LoginResponse currentLoginStatus,
        string executionPath,
        [CallerMemberName] string methodName = ""
    )
    {
        if (currentLoginStatus == this.connectionState.LoginStatus)
        {
            return;
        }

        var payload = CacheLogArgs.New(
            this.logger,
            LogLevel.Warning,
            this.connectionState,
            currentLoginStatus: currentLoginStatus
        );

        Log.StaleConnectionState(
            this.logger,
            methodName,
            payload,
            executionPath
        );
    }

    /// <summary>
    ///   Accesses <see cref="connectionState"/> - must be within <see cref="stateLock"/> scope!
    /// </summary>
    /// <param name="currentVoicemeeterKind"></param>
    /// <param name="executionPath"></param>
    /// <param name="methodName"></param>
    private void HandleStaleCache(
        Kind currentVoicemeeterKind,
        string executionPath,
        [CallerMemberName] string methodName = ""
    )
    {
        if (currentVoicemeeterKind == this.connectionState.VoicemeeterKind)
        {
            return;
        }

        var payload = CacheLogArgs.New(
            this.logger,
            LogLevel.Warning,
            this.connectionState,
            currentVoicemeeterKind: currentVoicemeeterKind
        );

        Log.StaleConnectionState(
            this.logger,
            methodName,
            payload,
            executionPath
        );
    }

    /// <summary>
    ///   Accesses <see cref="connectionState"/> - must be within <see cref="stateLock"/> scope!
    /// </summary>
    /// <param name="voicemeeterState"></param>
    /// <param name="executionPath"></param>
    /// <param name="methodName"></param>
    private void HandleStaleCache(
        (App, RunResponse) currentVoicemeeterAppState,
        string executionPath,
        [CallerMemberName] string methodName = ""
    )
    {
        (var app, var state) = currentVoicemeeterAppState;
        var previousState = this.connectionState;

        if ((app == previousState.VoicemeeterApp
                && state == previousState.VoicemeeterState)
            || (app != previousState.VoicemeeterApp
                && app is not App.None && state is RunResponse.NotRunning))
        {
            return;
        }

        var payload = CacheLogArgs.New(
            this.logger,
            LogLevel.Warning,
            previousState,
            currentVoicemeeterApp: app,
            currentVoicemeeterState: state
        );

        Log.StaleConnectionState(
            this.logger,
            methodName,
            payload,
            executionPath
        );
    }

    /// <summary>
    ///   Accesses <see cref="connectionState"/> - must be within <see cref="stateLock"/> scope!
    /// </summary>
    /// <param name="currentVoicemeeterVersion"></param>
    /// <param name="executionPath"></param>
    /// <param name="methodName"></param>
    private void HandleStaleCache(
        VmVersion currentVoicemeeterVersion,
        string executionPath,
        [CallerMemberName] string methodName = ""
    )
    {
        if (currentVoicemeeterVersion == this.connectionState.VoicemeeterVersion)
        {
            return;
        }

        var payload = CacheLogArgs.New(
            this.logger,
            LogLevel.Warning,
            this.connectionState,
            currentVoicemeeterVersion: currentVoicemeeterVersion
        );

        Log.StaleConnectionState(
            this.logger,
            methodName,
            payload,
            executionPath
        );
    }

    /// <summary>
    ///   Accesses <see cref="connectionState"/> - must be within <see cref="stateLock"/> scope!
    /// </summary>
    /// <param name="currentMacroButtonsState"></param>
    /// <param name="executionPath"></param>
    /// <param name="methodName"></param>
    private void HandleStaleCache(
        RunResponse currentMacroButtonsState,
        string executionPath,
        [CallerMemberName] string methodName = ""
    )
    {
        if (currentMacroButtonsState == this.connectionState.MacroButtonsState)
        {
            return;
        }

        var payload = CacheLogArgs.New(
            this.logger,
            LogLevel.Warning,
            this.connectionState,
            currentMacroButtonsState: currentMacroButtonsState
        );

        Log.StaleConnectionState(
            this.logger,
            methodName,
            payload,
            executionPath
        );
    }

    /// <summary>
    ///   Accesses <see cref="connectionState"/> - must be within <see cref="stateLock"/> scope!
    /// </summary>
    /// <param name="currentState"></param>
    /// <param name="executionPath"></param>
    /// <param name="methodName"></param>
    /// <returns>
    ///   Previously cached state
    /// </returns>
    private void UpdateStaleCache(
        ConnectionState currentState,
        string executionPath,
        [CallerMemberName] string methodName = ""
    )
    {
        var previousState = this.connectionState;

        if (currentState == previousState)
        {
            return;
        }

        this.connectionState = currentState;

        var payload = CacheLogArgs.New(
            this.logger,
            LogLevel.Information,
            previousState,
            currentState: currentState
        );

        Log.ConnectionStateChanged(
            this.logger,
            methodName,
            payload,
            executionPath
        );
    }
}
