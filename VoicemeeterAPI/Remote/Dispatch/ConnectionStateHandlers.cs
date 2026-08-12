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
        InvalidOperationException ex;
        LogArgs payload;

        if (login is LoginResponse.Unknown)
        {
            ex = new($"Cannot get Connection State when LoginStatus is Unknown.");
            payload = LogArgs.New(this.logger, LogLevel.Error, loginResponse: login);
            Log.RemoteContractViolation(this.logger, ex, methodName, "LoginStatus is Unknown.", payload, executionPath);
            throw ex;
        }

        if (!(vmVersion == default || vmVersion.IsValid()))
        {
            ex = new($"This library does not support Voicemeeter version '{vmVersion}' (currently running).");
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

        if (!connectionState.ConnectedToMacroButtons)
        {
            var payload = LogArgs.New(this.logger, LogLevel.Warning, connectionState: connectionState);
            Log.RemoteNotConnected(this.logger, methodName, "MacroButtons", payload, executionPath);
        }
    }
}
