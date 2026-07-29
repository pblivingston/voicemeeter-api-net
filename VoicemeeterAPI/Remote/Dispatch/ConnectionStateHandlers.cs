// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

public partial class Remote
{
    private ConnectionState HandleConnectionState(
        LoginResponse login,
        RunResponse vmState,
        App vmApp,
        VmVersion vmVersion,
        RunResponse mbState,
        string executionPath,
        [CallerMemberName] string methodName = ""
    )
    {
        LogArgs payload;

        if (!(vmVersion == default || vmVersion.IsValid()))
        {
            var ex = new InvalidOperationException($"This library does not support Voicemeeter version '{vmVersion}' (currently running).");
            payload = LogArgs.New(this.logger, LogLevel.Error, version: vmVersion);
            Log.RemoteContractViolation(this.logger, ex, methodName, "Voicemeeter version not supported.", payload, executionPath);
            throw ex;
        }

        var state = new ConnectionState(login, vmState, vmApp, vmVersion, mbState);

        payload = LogArgs.New(this.logger, LogLevel.Information, connectionState: state);
        Log.RemoteMethodSuccess(this.logger, LogLevel.Information, methodName, payload, executionPath);

        return state;
    }

    private void HandleConnectionState(
        ConnectionState connectionState,
        string executionPath,
        [CallerMemberName] string methodName = ""
    )
    {
        if (!connectionState.ConnectedToVoicemeeter)
        {
            var payload = LogArgs.New(this.logger, LogLevel.Warning, connectionState: connectionState);
            Log.RemoteNotConnected(this.logger, methodName, "Voicemeeter", payload, executionPath);
        }

        if (!connectionState.MacroButtonsIsRunning)
        {
            var payload = LogArgs.New(this.logger, LogLevel.Warning, connectionState: connectionState);
            Log.RemoteNotConnected(this.logger, methodName, "MacroButtons", payload, executionPath);
        }
    }
}
