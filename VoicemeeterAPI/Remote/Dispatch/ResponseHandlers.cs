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
        var login = methodName is nameof(this.Login_p);
        var payload = new LogArgs(response);

        switch (response)
        {
            case LoginResponse.VoicemeeterNotRunning when login:
            case LoginResponse.Ok:
                Log.RemoteMethodSuccess(this.logger, LogLevel.Information, methodName, payload, executionPath);
                return login ? response : LoginResponse.LoggedOut;

            case LoginResponse.NoClient when login:
                var ex1 = new NoClientException(this.lastConnectionState);
                Log.RemoteLoginFailed(this.logger, ex1, methodName, payload, executionPath);
                throw ex1;

            case LoginResponse.AlreadyLoggedIn when login:
                var ex2 = new AlreadyLoggedInException(this.lastConnectionState);
                Log.RemoteContractViolation(this.logger, ex2, methodName, "Already logged in.", payload, executionPath);
                throw ex2;

            case LoginResponse.Unknown:
            case LoginResponse.LoggedOut:
            case LoginResponse.VoicemeeterNotRunning:
            case LoginResponse.NoClient:
            case LoginResponse.AlreadyLoggedIn:
            case LoginResponse.AlreadyLoggedOut:
            case LoginResponse.Timeout:
            default:
                if (login)
                {
                    var ex = new RemoteException("Unhandled response!", response, this.lastConnectionState);
                    Log.UnhandledResponse(this.logger, ex, methodName, payload, executionPath);
                    throw ex;
                } // never throw during Logout; just log
                Log.UnhandledLogoutResponse(this.logger, methodName, payload, executionPath);
                return LoginResponse.Unknown;
        }
    }

    /// <summary>
    ///   Accesses this.lastConnectionState - must be within this.stateLock scope!
    /// </summary>
    /// <param name="response"></param>
    /// <param name="payload"></param>
    /// <param name="executionPath"></param>
    /// <param name="trace"></param>
    /// <param name="paramName"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    private RunResponse HandleResponse(
        RunResponse response,
        LogArgs payload,
        string executionPath,
        bool trace = false,
        string paramName = "app",
        [CallerMemberName] string methodName = ""
    )
    {
        var info = trace ? LogLevel.Trace : LogLevel.Information;
        var warning = trace ? LogLevel.Trace : LogLevel.Warning;
        var aState = methodName is nameof(this.GetAppState_i);

        switch (response)
        {
            case RunResponse.NotResponding when aState:
                Log.RemoteMethodSuccess(this.logger, warning, methodName, payload, executionPath);
                break;

            case RunResponse.NotRunning when aState:
            case RunResponse.Hidden when aState:
            case RunResponse.Ok when aState:
                Log.RemoteMethodSuccess(this.logger, info, methodName, payload, executionPath);
                break;

            case RunResponse.Ok:
                Log.RemoteMethodSuccess(this.logger, LogLevel.Information, methodName, payload, executionPath);
                break;

            case RunResponse.NotInstalled:
                var ex1 = new AppNotInstalledException(payload.App ?? default);
                Log.AppCriticalState(this.logger, ex1, methodName, payload, executionPath);
                throw ex1;

            case RunResponse.UnknownApp:
                var ex2 = new VmArgumentException($"'{payload.App ?? default}' is not a valid VB-Audio application.", paramName);
                Log.RemoteInvalidArgument(this.logger, ex2, methodName, payload, executionPath);
                throw ex2;

            case RunResponse.NotResponding:
            case RunResponse.NotRunning:
            case RunResponse.Hidden:
            case RunResponse.AlreadyShutDown:
            case RunResponse.Timeout:
            case RunResponse.Error:
            default:
                var ex = new RemoteException("Unhandled response!", response, this.lastConnectionState);
                Log.UnhandledResponse(this.logger, ex, methodName, payload, executionPath);
                throw ex;
        }

        return response;
    }

    /// <summary>
    ///   Can raise this.ParamsDirty or this.ButtonsDirty - must be outside lock scope in this.IsParamsDirty and this.IsButtonsDirty!
    /// </summary>
    /// <param name="response"></param>
    /// <param name="executionPath"></param>
    /// <param name="payload"></param>
    /// <param name="trace"></param>
    /// <param name="paramName"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    private bool HandleResponse(
        Response response,
        string executionPath,
        LogArgs payload = default,
        bool trace = false,
        string paramName = "param",
        [CallerMemberName] string methodName = ""
    )
    {
        var info = trace ? LogLevel.Trace : LogLevel.Information;
        var warning = trace ? LogLevel.Trace : LogLevel.Warning;
        var pDirty = methodName is nameof(this.ParamsDirty_i);
        var bDirty = methodName is nameof(this.ButtonsDirty_i);

        switch (response)
        {
            case Response.Dirty when pDirty:
                this.OnParamsDirty();
                return true;

            case Response.Dirty when bDirty:
                this.OnButtonsDirty();
                return true;

            case Response.Ok when pDirty || bDirty:
                return false;

            case Response.Ok:
                Log.RemoteMethodSuccess(this.logger, info, methodName, payload, executionPath);
                return true;

            case Response.Error when !this.LoggedIn:
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
                var ex1b = new RemoteException("Operation could not be completed as requested.", response, this.LastConnectionState);
                Log.RemoteMethodError(this.logger, ex1b, methodName, payload, executionPath);
                throw ex1b;

            case Response.NoServer when methodName is (nameof(this.GetKind_i)) or (nameof(this.GetVersion_i)):
                Log.RemoteMethodSuccess(this.logger, warning, methodName, payload, executionPath);
                return false;

            case Response.NoServer when !this.Connected:
                var ex2a = new EngineNotRunningException(this.LastConnectionState);
                Log.RemoteContractViolation(this.logger, ex2a, methodName, "Voicemeeter is not running.", payload, executionPath);
                throw ex2a;

            case Response.NoServer:
                var ex2b = new EngineNotRunningException(this.LastConnectionState);
                Log.RemoteLostConnection(this.logger, ex2b, methodName, "Voicemeeter", payload, executionPath);
                throw ex2b;

            case Response.UnknownParameter:
                var ex3 = new VmArgumentException($"'{payload.Param}' is not a valid Voicemeeter parameter.", paramName);
                Log.RemoteInvalidArgument(this.logger, ex3, methodName, payload, executionPath);
                throw ex3;

            case Response.Dirty:
            case Response.UnknownApp:
            case Response.TypeMismatch:
            default:
                var ex = new RemoteException("Unhandled response!", response, this.LastConnectionState);
                Log.UnhandledResponse(this.logger, ex, methodName, payload, executionPath);
                throw ex;
        }
    }

    /// <summary>
    ///   this.HandleStaleCache accesses this.lastConnectionState - must be within this.stateLock scope!
    /// </summary>
    /// <param name="login"></param>
    /// <param name="version"></param>
    /// <param name="mbState"></param>
    /// <param name="executionPath"></param>
    /// <param name="methodName"></param>
    /// <returns>
    ///   Previously cached state
    /// </returns>
    private ConnectionState HandleResponse(
        LoginResponse login,
        VmVersion version,
        RunResponse mbState,
        string executionPath,
        [CallerMemberName] string methodName = ""
    )
    {
        var state = new ConnectionState(login, mbState, version.K, version);

        Log.RemoteMethodSuccess(this.logger, LogLevel.Information, methodName, new(state), executionPath);

        if (!state.Connected)
        {
            Log.RemoteNotConnected(this.logger, methodName, "Voicemeeter", executionPath);
        }

        if (!state.ButtonsRunning)
        {
            Log.RemoteNotConnected(this.logger, methodName, "MacroButtons", executionPath);
        }

        return this.HandleStaleCache(state, executionPath, methodName);
    }
}
