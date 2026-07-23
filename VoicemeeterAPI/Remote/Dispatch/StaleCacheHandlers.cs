// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

public partial class Remote
{
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

    private void HandleStaleCache(
        RunResponse currentButtonsState,
        string executionPath,
        [CallerMemberName] string methodName = ""
    )
    {
        if (currentButtonsState == this.lastConnectionState.ButtonsState)
        {
            return;
        }

        var payload = CacheLogArgs.New(
            this.logger,
            LogLevel.Warning,
            this.lastConnectionState,
            currentButtonsState: currentButtonsState
        );

        Log.StaleConnectionState(
            this.logger,
            methodName,
            payload,
            executionPath
        );
    }

    private void HandleStaleCache(
        Kind currentRunningKind,
        string executionPath,
        [CallerMemberName] string methodName = ""
    )
    {
        if (currentRunningKind == this.lastConnectionState.RunningKind)
        {
            return;
        }

        var payload = CacheLogArgs.New(
            this.logger,
            LogLevel.Warning,
            this.lastConnectionState,
            currentRunningKind: currentRunningKind
        );

        Log.StaleConnectionState(
            this.logger,
            methodName,
            payload,
            executionPath
        );
    }

    private void HandleStaleCache(
        VmVersion currentRunningVersion,
        string executionPath,
        [CallerMemberName] string methodName = ""
    )
    {
        if (currentRunningVersion == this.lastConnectionState.RunningVersion)
        {
            return;
        }

        var payload = CacheLogArgs.New(
            this.logger,
            LogLevel.Warning,
            this.lastConnectionState,
            currentRunningVersion: currentRunningVersion
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
