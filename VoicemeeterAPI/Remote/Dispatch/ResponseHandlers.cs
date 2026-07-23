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
    /// <param name="response"></param>
    /// <param name="executionPath"></param>
    /// <param name="methodName"></param>
    /// <returns>
    ///   Value for this.loginStatus
    /// </returns>
    private LoginResponse HandleResponse(
        LoginResponse response,
        string executionPath,
        [CallerMemberName] string methodName = ""
    )
    {
        LogArgs payload;

        switch (response)
        {
            case LoginResponse.VoicemeeterNotRunning:
            case LoginResponse.Ok:
                payload = LogArgs.New(this.logger, LogLevel.Information, loginResponse: response);
                Log.RemoteMethodSuccess(this.logger, LogLevel.Information, methodName, payload, executionPath);
                return response;

            case LoginResponse.NoClient:
                payload = LogArgs.New(this.logger, LogLevel.Error, loginResponse: response);
                var ex1 = new NoClientException(this.lastConnectionState);
                Log.RemoteLoginFailed(this.logger, ex1, methodName, payload, executionPath);
                throw ex1;

            case LoginResponse.AlreadyLoggedIn:
                payload = LogArgs.New(this.logger, LogLevel.Error, loginResponse: response);
                var ex2 = new AlreadyLoggedInException(this.lastConnectionState);
                Log.RemoteContractViolation(this.logger, ex2, methodName, "Already logged in.", payload, executionPath);
                throw ex2;

            case LoginResponse.Unknown:
            case LoginResponse.LoggedOut:
            default:
                payload = LogArgs.New(this.logger, LogLevel.Critical, loginResponse: response);
                var ex = new UnhandledResponseException(response, this.lastConnectionState);
                Log.UnhandledResponse(this.logger, ex, methodName, payload, executionPath);
                throw ex;
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="response"></param>
    /// <param name="executionPath"></param>
    /// <param name="methodName"></param>
    /// <returns>
    ///   Value for this.loginStatus
    /// </returns>
    private LoginResponse HandleLogoutResponse(
        LoginResponse response,
        string executionPath,
        [CallerMemberName] string methodName = ""
    )
    {
        LogArgs payload;

        switch (response)
        {
            case LoginResponse.Ok:
                payload = LogArgs.New(this.logger, LogLevel.Information, loginResponse: response);
                Log.RemoteMethodSuccess(this.logger, LogLevel.Information, methodName, payload, executionPath);
                return LoginResponse.LoggedOut;

            case LoginResponse.Unknown:
            case LoginResponse.LoggedOut:
            case LoginResponse.VoicemeeterNotRunning:
            case LoginResponse.NoClient:
            case LoginResponse.AlreadyLoggedIn:
            default:
                payload = LogArgs.New(this.logger, LogLevel.Critical, loginResponse: response);
                Log.UnhandledLogoutResponse(this.logger, methodName, payload, executionPath);
                return LoginResponse.Unknown;
        }
    }

    /// <summary>
    ///   Accesses this.lastConnectionState - must be within this.stateLock scope!
    /// </summary>
    /// <param name="response"></param>
    /// <param name="app"></param>
    /// <param name="executionPath"></param>
    /// <param name="paramName"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    private RunResponse HandleResponse(
        RunResponse response,
        App app,
        string executionPath,
        string paramName = "app",
        [CallerMemberName] string methodName = ""
    )
    {
        var aState = methodName is nameof(this.GetAppState_i);
        LogArgs payload;

        switch (response)
        {
            case RunResponse.NotResponding when aState:
                payload = LogArgs.New(this.logger, LogLevel.Warning, app: app, state: response);
                Log.RemoteMethodSuccess(this.logger, LogLevel.Warning, methodName, payload, executionPath);
                return response;

            case RunResponse.NotRunning when aState:
            case RunResponse.Hidden when aState:
            case RunResponse.Ok when aState:
                payload = LogArgs.New(this.logger, LogLevel.Debug, app: app, state: response);
                Log.RemoteMethodSuccess(this.logger, LogLevel.Debug, methodName, payload, executionPath);
                return response;

            case RunResponse.Ok:
                payload = LogArgs.New(this.logger, LogLevel.Information, app: app);
                Log.RemoteMethodSuccess(this.logger, LogLevel.Information, methodName, payload, executionPath);
                return response;

            case RunResponse.NotInstalled:
                payload = LogArgs.New(this.logger, LogLevel.Error, app: app);
                var ex1 = new AppNotInstalledException(app);
                Log.AppCriticalState(this.logger, ex1, methodName, payload, executionPath);
                throw ex1;

            case RunResponse.UnknownApp:
                payload = LogArgs.New(this.logger, LogLevel.Error, app: app);
                var ex2 = new VmArgumentException($"'{app}' is not a valid VB-Audio application.", paramName);
                Log.RemoteInvalidArgument(this.logger, ex2, methodName, payload, executionPath);
                throw ex2;

            case RunResponse.NotResponding:
            case RunResponse.NotRunning:
            case RunResponse.Hidden:
            case RunResponse.Error:
            case RunResponse.Timeout:
            case RunResponse.AlreadyShutDown:
            default:
                payload = LogArgs.New(this.logger, LogLevel.Critical, app: app);
                var ex = new UnhandledResponseException(response, this.lastConnectionState);
                Log.UnhandledResponse(this.logger, ex, methodName, payload, executionPath);
                throw ex;
        }
    }

    /// <summary>
    ///   Can raise this.ParamsDirty or this.ButtonsDirty - must be outside lock scope!
    /// </summary>
    /// <param name="response"></param>
    /// <param name="executionPath"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    private bool HandleResponse(
        Response response,
        string executionPath,
        [CallerMemberName] string methodName = ""
    )
    {
        var pDirty = methodName is nameof(this.ParamsDirty_i);
        var payload = LogArgs.Empty;

        switch (response)
        {
            case Response.Dirty when pDirty:
                this.OnParamsDirty();
                return true;

            case Response.Dirty:
                this.OnButtonsDirty();
                return true;

            case Response.Ok:
                return false;

            case Response.Error when !this.LoggedIn:
                var ex1a = new NoClientException(this.LastConnectionState);
                Log.RemoteContractViolation(this.logger, ex1a, methodName, "Not logged in.", payload, executionPath);
                throw ex1a;

            case Response.Error:
                var ex1b = new RemoteException("Operation could not be completed as requested.", response, this.LastConnectionState);
                Log.RemoteMethodError(this.logger, ex1b, methodName, payload, executionPath);
                throw ex1b;

            case Response.NoServer when !this.Connected:
                var ex2a = new EngineNotRunningException(this.LastConnectionState);
                Log.RemoteContractViolation(this.logger, ex2a, methodName, "Voicemeeter is not running.", payload, executionPath);
                throw ex2a;

            case Response.NoServer:
                var ex2b = new EngineNotRunningException(this.LastConnectionState);
                Log.RemoteLostConnection(this.logger, ex2b, methodName, "Voicemeeter", payload, executionPath);
                throw ex2b;

            case Response.UnknownParameter:
            case Response.StructureMismatch:
            case Response.UnknownApp:
            default:
                var ex = new UnhandledResponseException(response, this.LastConnectionState);
                Log.UnhandledResponse(this.logger, ex, methodName, payload, executionPath);
                throw ex;
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="response"></param>
    /// <param name="param"></param>
    /// <param name="value"></param>
    /// <param name="executionPath"></param>
    /// <param name="paramName"></param>
    /// <param name="methodName"></param>
    private void HandleResponse(
        Response response,
        string param,
        object value,
        string executionPath,
        string paramName = "param",
        [CallerMemberName] string methodName = ""
    )
    {
        LogArgs payload;

        switch (response)
        {
            case Response.Ok:
                payload = LogArgs.New(this.logger, LogLevel.Trace, param: param, value: value);
                Log.RemoteMethodSuccess(this.logger, LogLevel.Trace, methodName, payload, executionPath);
                return;

            case Response.Error when !this.LoggedIn:
                payload = LogArgs.New(this.logger, LogLevel.Error, param: param, value: value);
                var ex1a = new NoClientException(this.LastConnectionState);
                Log.RemoteContractViolation(this.logger, ex1a, methodName, "Not logged in.", payload, executionPath);
                throw ex1a;

            case Response.Error:
            // I'm not sure what structure mismatch (-5) actually indicates as I haven't seen it yet.
            // I've currently only interacted with the dll through existing wrappers, so it may be fully accounted for already.
            // Calling VBVMR_GetParameterFloat with a string parameter returns -1: Response.Error and
            // VoicemeeterRemote is perfectly happy to write float values to the string buffer, so calling VBVMR_GetParameterStringA
            // or VBVMR_GetParameterStringW with "Bus[2].Gain", "Strip[3].Mute", etc simply returns a string representation of the value.
            // Possibly returned when the pointer passed to receive the value doesn't match the expected type,
            // which shouldn't be encountered here unless the underlying wrapper or the dll itself has changed or is broken.
            case Response.StructureMismatch:
                payload = LogArgs.New(this.logger, LogLevel.Error, param: param, value: value);
                var ex1b = new RemoteException("Operation could not be completed as requested.", response, this.LastConnectionState);
                Log.RemoteMethodError(this.logger, ex1b, methodName, payload, executionPath);
                throw ex1b;

            case Response.NoServer when !this.Connected:
                payload = LogArgs.New(this.logger, LogLevel.Error, param: param, value: value);
                var ex2a = new EngineNotRunningException(this.LastConnectionState);
                Log.RemoteContractViolation(this.logger, ex2a, methodName, "Voicemeeter is not running.", payload, executionPath);
                throw ex2a;

            case Response.NoServer:
                payload = LogArgs.New(this.logger, LogLevel.Error, param: param, value: value);
                var ex2b = new EngineNotRunningException(this.LastConnectionState);
                Log.RemoteLostConnection(this.logger, ex2b, methodName, "Voicemeeter", payload, executionPath);
                throw ex2b;

            case Response.UnknownParameter:
                payload = LogArgs.New(this.logger, LogLevel.Error, param: param, value: value);
                var ex3 = new VmArgumentException($"'{param}' is not a valid Voicemeeter parameter.", paramName);
                Log.RemoteInvalidArgument(this.logger, ex3, methodName, payload, executionPath);
                throw ex3;

            case Response.Dirty:
            case Response.UnknownApp:
            default:
                payload = LogArgs.New(this.logger, LogLevel.Critical, param: param, value: value);
                var ex = new UnhandledResponseException(response, this.LastConnectionState);
                Log.UnhandledResponse(this.logger, ex, methodName, payload, executionPath);
                throw ex;
        }
    }

    /// <summary>
    ///   Accesses this.lastConnectionState - must be within this.stateLock scope!
    /// </summary>
    /// <param name="response"></param>
    /// <param name="kind"></param>
    /// <param name="executionPath"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    private bool HandleResponse(
        Response response,
        Kind kind,
        string executionPath,
        [CallerMemberName] string methodName = ""
    )
    {
        LogArgs payload;

        switch (response)
        {
            case Response.Ok:
                payload = LogArgs.New(this.logger, LogLevel.Debug, kind: kind);
                Log.RemoteMethodSuccess(this.logger, LogLevel.Debug, methodName, payload, executionPath);
                return true;

            case Response.Error:
                payload = LogArgs.New(this.logger, LogLevel.Error, kind: kind);
                var ex1 = new NoClientException(this.lastConnectionState);
                Log.RemoteContractViolation(this.logger, ex1, methodName, "Not logged in.", payload, executionPath);
                throw ex1;

            case Response.NoServer:
                payload = LogArgs.New(this.logger, LogLevel.Warning, kind: kind);
                Log.RemoteMethodSuccess(this.logger, LogLevel.Warning, methodName, payload, executionPath);
                return false;

            case Response.Dirty:
            case Response.UnknownParameter:
            case Response.StructureMismatch:
            case Response.UnknownApp:
            default:
                payload = LogArgs.New(this.logger, LogLevel.Critical, kind: kind);
                var ex = new UnhandledResponseException(response, this.lastConnectionState);
                Log.UnhandledResponse(this.logger, ex, methodName, payload, executionPath);
                throw ex;
        }
    }

    /// <summary>
    ///   Accesses this.lastConnectionState - must be within this.stateLock scope!
    /// </summary>
    /// <param name="response"></param>
    /// <param name="version"></param>
    /// <param name="executionPath"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    private bool HandleResponse(
        Response response,
        VmVersion version,
        string executionPath,
        [CallerMemberName] string methodName = ""
    )
    {
        LogArgs payload;

        switch (response)
        {
            case Response.Ok:
                payload = LogArgs.New(this.logger, LogLevel.Debug, version: version);
                Log.RemoteMethodSuccess(this.logger, LogLevel.Debug, methodName, payload, executionPath);
                return true;

            case Response.Error:
                payload = LogArgs.New(this.logger, LogLevel.Error, version: version);
                var ex1 = new NoClientException(this.lastConnectionState);
                Log.RemoteContractViolation(this.logger, ex1, methodName, "Not logged in.", payload, executionPath);
                throw ex1;

            case Response.NoServer:
                payload = LogArgs.New(this.logger, LogLevel.Warning, version: version);
                Log.RemoteMethodSuccess(this.logger, LogLevel.Warning, methodName, payload, executionPath);
                return false;

            case Response.Dirty:
            case Response.UnknownParameter:
            case Response.StructureMismatch:
            case Response.UnknownApp:
            default:
                payload = LogArgs.New(this.logger, LogLevel.Critical, version: version);
                var ex = new UnhandledResponseException(response, this.lastConnectionState);
                Log.UnhandledResponse(this.logger, ex, methodName, payload, executionPath);
                throw ex;
        }
    }

    /// <summary>
    ///   Logs warnings when Voicemeeter or MacroButtons are not running
    /// </summary>
    /// <param name="login"></param>
    /// <param name="version"></param>
    /// <param name="mbState"></param>
    /// <param name="executionPath"></param>
    /// <param name="methodName"></param>
    /// <returns>
    ///   Current state
    /// </returns>
    private ConnectionState HandleResponse(
        LoginResponse login,
        VmVersion version,
        RunResponse mbState,
        bool loggedOut,
        string executionPath,
        [CallerMemberName] string methodName = ""
    )
    {
        LogArgs payload;

        if (!(version == default || version.IsValid()))
        {
            var ex = new VersionNotSupportedException(version, this.lastConnectionState);
            payload = LogArgs.New(this.logger, LogLevel.Error, version: version);
            Log.RemoteContractViolation(this.logger, ex, methodName, "Voicemeeter version not supported.", payload, executionPath);
            throw ex;
        }

        var state = new ConnectionState(login, mbState, version.K, version);

        payload = LogArgs.New(this.logger, LogLevel.Information, connectionState: state);
        Log.RemoteMethodSuccess(this.logger, LogLevel.Information, methodName, payload, executionPath);

        if (loggedOut)
        {
            return state;
        }

        if (!state.Connected)
        {
            Log.RemoteNotConnected(this.logger, methodName, "Voicemeeter", executionPath);
        }

        if (!state.ButtonsRunning)
        {
            Log.RemoteNotConnected(this.logger, methodName, "MacroButtons", executionPath);
        }

        return state;
    }
}
