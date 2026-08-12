// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

public partial class Remote
{
    private const string NotLoggedInMessage = "Operation cannot be performed while not logged in to VoicemeeterRemote.";
    private const string NotConnectedMessage = "Operation cannot be performed while Voicemeeter is not running.";
    private const string AmbiguousMessage = "VoicemeeterRemote operation could not be completed for an unknown reason.";

    /// <summary>
    ///   Can raise this.ParamsDirty or this.ButtonsDirty - must be outside lock scope!
    /// </summary>
    /// <param name="response"></param>
    /// <param name="executionPath"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    private Result<Response, bool> HandleDirtyResponse(
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
                return (response, true);

            case Response.Dirty:
                this.OnButtonsDirty();
                return (response, true);

            case Response.Ok:
                return (response, false);

            case Response.Error when !this.LoginStatus.IsLoggedIn():
                var ex1a = new InvalidOperationException(NotLoggedInMessage);
                Log.RemoteContractViolation(this.logger, ex1a, methodName, "Not logged in.", payload, executionPath);
                throw ex1a;

            case Response.Error:
                var ex1b = new RemoteException(AmbiguousMessage, response, this.LastConnectionState);
                Log.RemoteMethodError(this.logger, ex1b, methodName, payload, executionPath);
                throw ex1b;

            case Response.NoServer when !this.ConnectedToVoicemeeter:
                var ex2a = new InvalidOperationException(NotConnectedMessage);
                Log.RemoteContractViolation(this.logger, ex2a, methodName, "Voicemeeter is not running.", payload, executionPath);
                throw ex2a;

            case Response.NoServer:
                Log.RemoteLostConnection(this.logger, methodName, "Voicemeeter", payload, executionPath);
                this.LoginStatus = LoginResponse.VoicemeeterNotRunning;
                return response;

            case Response.UnknownParameter:
            case Response.StructureMismatch:
            default:
                var ex = new UnhandledResponseException(response, this.LastConnectionState);
                Log.UnhandledResponse(this.logger, ex, methodName, payload, executionPath);
                throw ex;
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="response"></param>
    /// <param name="param"></param>
    /// <param name="value"></param>
    /// <param name="executionPath"></param>
    /// <param name="paramName"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    private Result<Response, T> HandleGetParamResponse<T>(
        Response response,
        string param,
        T value,
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
                return (response, value);

            case Response.Error when !this.LoginStatus.IsLoggedIn():
                payload = LogArgs.New(this.logger, LogLevel.Error, param: param, value: value);
                var ex1a = new InvalidOperationException(NotLoggedInMessage);
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
                var ex1b = new RemoteException(AmbiguousMessage, response, this.LastConnectionState);
                Log.RemoteMethodError(this.logger, ex1b, methodName, payload, executionPath);
                throw ex1b;

            case Response.NoServer when !this.ConnectedToVoicemeeter:
                payload = LogArgs.New(this.logger, LogLevel.Error, param: param, value: value);
                var ex2a = new InvalidOperationException(NotConnectedMessage);
                Log.RemoteContractViolation(this.logger, ex2a, methodName, "Voicemeeter is not running.", payload, executionPath);
                throw ex2a;

            case Response.NoServer:
                payload = LogArgs.New(this.logger, LogLevel.Warning, param: param, value: value);
                Log.RemoteLostConnection(this.logger, methodName, "Voicemeeter", payload, executionPath);
                this.LoginStatus = LoginResponse.VoicemeeterNotRunning;
                return response;

            case Response.UnknownParameter:
                payload = LogArgs.New(this.logger, LogLevel.Error, param: param, value: value);
                var ex3 = new ArgumentException($"'{param}' is not a valid Voicemeeter parameter.", paramName);
                Log.RemoteInvalidArgument(this.logger, ex3, methodName, payload, executionPath);
                throw ex3;

            case Response.Dirty:
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
    private Kind HandleGetKindResponse(
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
                return kind;

            case Response.Error:
                payload = LogArgs.New(this.logger, LogLevel.Error, kind: kind);
                var ex1 = new InvalidOperationException(NotLoggedInMessage);
                Log.RemoteContractViolation(this.logger, ex1, methodName, "Not logged in.", payload, executionPath);
                throw ex1;

            case Response.NoServer:
                payload = LogArgs.New(this.logger, LogLevel.Warning, kind: kind);
                Log.RemoteMethodSuccess(this.logger, LogLevel.Warning, methodName, payload, executionPath);
                this.loginStatus = LoginResponse.VoicemeeterNotRunning;
                return default;

            case Response.Dirty:
            case Response.UnknownParameter:
            case Response.StructureMismatch:
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
    private VmVersion HandleGetVersionResponse(
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
                return version;

            case Response.Error:
                payload = LogArgs.New(this.logger, LogLevel.Error, version: version);
                var ex1 = new InvalidOperationException(NotLoggedInMessage);
                Log.RemoteContractViolation(this.logger, ex1, methodName, "Not logged in.", payload, executionPath);
                throw ex1;

            case Response.NoServer:
                payload = LogArgs.New(this.logger, LogLevel.Warning, version: version);
                Log.RemoteMethodSuccess(this.logger, LogLevel.Warning, methodName, payload, executionPath);
                this.loginStatus = LoginResponse.VoicemeeterNotRunning;
                return default;

            case Response.Dirty:
            case Response.UnknownParameter:
            case Response.StructureMismatch:
            default:
                payload = LogArgs.New(this.logger, LogLevel.Critical, version: version);
                var ex = new UnhandledResponseException(response, this.lastConnectionState);
                Log.UnhandledResponse(this.logger, ex, methodName, payload, executionPath);
                throw ex;
        }
    }
}
