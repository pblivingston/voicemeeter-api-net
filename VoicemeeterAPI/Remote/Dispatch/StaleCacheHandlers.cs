// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using System.Runtime.CompilerServices;

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

        Log.StaleConnectionState(
            this.logger,
            methodName,
            new(this.lastConnectionState, currentLoginStatus),
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

        Log.StaleConnectionState(
            this.logger,
            methodName,
            new(this.lastConnectionState, currentButtonsState),
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

        Log.StaleConnectionState(
            this.logger,
            methodName,
            new(this.lastConnectionState, currentRunningKind),
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

        Log.StaleConnectionState(
            this.logger,
            methodName,
            new(this.lastConnectionState, currentRunningVersion),
            executionPath
        );
    }

    /// <summary>
    ///   Updates this.lastConnectionState
    /// </summary>
    /// <param name="currentState"></param>
    /// <param name="executionPath"></param>
    /// <param name="methodName"></param>
    /// <returns>
    ///   Previously cached state
    /// </returns>
    private ConnectionState HandleStaleCache(
        ConnectionState currentState,
        string executionPath,
        [CallerMemberName] string methodName = ""
    )
    {
        var previousState = this.lastConnectionState;

        if (currentState != previousState)
        {
            this.lastConnectionState = currentState;

            Log.ConnectionStateChanged(
                this.logger,
                methodName,
                new(previousState, currentState),
                executionPath
            );
        }

        return previousState;
    }
}
