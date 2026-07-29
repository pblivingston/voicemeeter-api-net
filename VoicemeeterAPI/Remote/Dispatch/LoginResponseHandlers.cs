// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

public partial class Remote
{
    /// <summary>
    ///   Accesses this.loginStatus and this.lastConnectionState - must be within this.stateLock scope!
    /// </summary>
    /// <param name="response"></param>
    /// <param name="executionPath"></param>
    /// <param name="methodName"></param>
    private void HandleLoginResponse(
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
                this.loginStatus = response;
                break;

            case LoginResponse.NoClient:
                payload = LogArgs.New(this.logger, LogLevel.Error, loginResponse: response);
                var ex1 = new CannotGetClientException(this.lastConnectionState);
                Log.RemoteLoginFailed(this.logger, ex1, methodName, payload, executionPath);
                throw ex1;

            case LoginResponse.AlreadyLoggedIn:
                payload = LogArgs.New(this.logger, LogLevel.Error, loginResponse: response);
                var ex2 = new InvalidOperationException("Login should not be performed more than once in a single session.");
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
    ///   Accesses this.loginStatus - must be within this.stateLock scope!
    /// </summary>
    /// <param name="response"></param>
    /// <param name="executionPath"></param>
    /// <param name="methodName"></param>
    private void HandleLogoutResponse(
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
                this.loginStatus = LoginResponse.LoggedOut;
                break;

            case LoginResponse.Unknown:
            case LoginResponse.LoggedOut:
            case LoginResponse.VoicemeeterNotRunning:
            case LoginResponse.NoClient:
            case LoginResponse.AlreadyLoggedIn:
            default:
                payload = LogArgs.New(this.logger, LogLevel.Critical, loginResponse: response);
                Log.UnhandledLogoutResponse(this.logger, methodName, payload, executionPath);
                this.loginStatus = LoginResponse.Unknown;
                break;
        }
    }
}
