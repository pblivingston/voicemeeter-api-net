// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

public partial class Remote
{
    /// <summary>
    ///   Accesses this.lastConnectionState - must be within this.stateLock scope!
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
        if (currentLoginStatus == this.lastConnectionState.LoginStatus)
        {
            return;
        }

        var payload = CacheLogArgs.New(
            this.logger,
            LogLevel.Warning,
            this.lastConnectionState,
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
    ///   Accesses this.lastConnectionState - must be within this.stateLock scope!
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
        if (currentVoicemeeterKind == this.lastConnectionState.VoicemeeterKind)
        {
            return;
        }

        var payload = CacheLogArgs.New(
            this.logger,
            LogLevel.Warning,
            this.lastConnectionState,
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
    ///   Accesses this.lastConnectionState - must be within this.stateLock scope!
    /// </summary>
    /// <param name="currentVoicemeeterApp"></param>
    /// <param name="executionPath"></param>
    /// <param name="methodName"></param>
    private void HandleStaleCache(
        App currentVoicemeeterApp,
        RunResponse currentVoicemeeterState,
        string executionPath,
        [CallerMemberName] string methodName = ""
    )
    {
        var previousState = this.lastConnectionState;

        if (currentVoicemeeterApp == previousState.VoicemeeterApp
            && currentVoicemeeterState == previousState.VoicemeeterState)
        {
            return;
        }

        var payload = CacheLogArgs.New(
            this.logger,
            LogLevel.Warning,
            previousState,
            currentVoicemeeterApp: currentVoicemeeterApp,
            currentVoicemeeterState: currentVoicemeeterState
        );

        Log.StaleConnectionState(
            this.logger,
            methodName,
            payload,
            executionPath
        );
    }

    /// <summary>
    ///   Accesses this.lastConnectionState - must be within this.stateLock scope!
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
        if (currentVoicemeeterVersion == this.lastConnectionState.VoicemeeterVersion)
        {
            return;
        }

        var payload = CacheLogArgs.New(
            this.logger,
            LogLevel.Warning,
            this.lastConnectionState,
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
    ///   Accesses this.lastConnectionState - must be within this.stateLock scope!
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
        if (currentMacroButtonsState == this.lastConnectionState.MacroButtonsState)
        {
            return;
        }

        var payload = CacheLogArgs.New(
            this.logger,
            LogLevel.Warning,
            this.lastConnectionState,
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
    ///   Updates this.lastConnectionState - must be within this.stateLock scope!
    /// </summary>
    /// <param name="currentState"></param>
    /// <param name="executionPath"></param>
    /// <param name="methodName"></param>
    /// <returns>
    ///   Previously cached state
    /// </returns>
    private void HandleStaleCache(
        ConnectionState currentState,
        string executionPath,
        [CallerMemberName] string methodName = ""
    )
    {
        var previousState = this.lastConnectionState;

        if (currentState == previousState)
        {
            return;
        }

        this.lastConnectionState = currentState;

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
