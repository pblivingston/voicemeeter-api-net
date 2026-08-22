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
    /// <param name="response"></param>
    /// <param name="app"></param>
    /// <param name="executionPath"></param>
    /// <param name="paramName"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    private void HandleRunResponse(
        RunResponse response,
        App app,
        string executionPath,
        string paramName = "app",
        [CallerMemberName] string methodName = ""
    )
    {
        LogArgs payload;

        switch (response)
        {
            case RunResponse.Ok:
                payload = LogArgs.New(this.logger, LogLevel.Information, app: app);
                Log.RemoteMethodSuccess(this.logger, LogLevel.Information, methodName, payload, executionPath);
                break;

            case RunResponse.NotInstalled:
                payload = LogArgs.New(this.logger, LogLevel.Error, app: app);
                var ex1 = new AppNotInstalledException(app);
                Log.AppCriticalState(this.logger, ex1, methodName, payload, executionPath);
                throw ex1;

            case RunResponse.UnknownApp:
                payload = LogArgs.New(this.logger, LogLevel.Error, app: app);
                var ex2 = new ArgumentException($"'{app}' is not a valid VB-Audio application.", paramName);
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
                var ex = new UnhandledResponseException(response, this.connectionState);
                Log.UnhandledResponse(this.logger, ex, methodName, payload, executionPath);
                throw ex;
        }
    }

    /// <summary>
    ///   Accesses <see cref="connectionState"/> - must be within <see cref="stateLock"/> scope!
    /// </summary>
    /// <param name="response"></param>
    /// <param name="app"></param>
    /// <param name="executionPath"></param>
    /// <param name="paramName"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    private RunResponse HandleAppStateResponse(
        RunResponse response,
        App app,
        string executionPath,
        string paramName = "app",
        [CallerMemberName] string methodName = ""
    )
    {
        LogArgs payload;

        switch (response)
        {
            case RunResponse.NotResponding:
            case RunResponse.NotRunning:
            case RunResponse.Hidden:
            case RunResponse.Ok:
                payload = LogArgs.New(this.logger, LogLevel.Debug, app: app, state: response);
                Log.RemoteMethodSuccess(this.logger, LogLevel.Debug, methodName, payload, executionPath);
                return response;

            case RunResponse.NotInstalled:
                payload = LogArgs.New(this.logger, LogLevel.Error, app: app);
                var ex1 = new AppNotInstalledException(app);
                Log.AppCriticalState(this.logger, ex1, methodName, payload, executionPath);
                throw ex1;

            case RunResponse.UnknownApp:
                payload = LogArgs.New(this.logger, LogLevel.Error, app: app);
                var ex2 = new ArgumentException($"'{app}' is not a valid VB-Audio application.", paramName);
                Log.RemoteInvalidArgument(this.logger, ex2, methodName, payload, executionPath);
                throw ex2;

            case RunResponse.Error:
            case RunResponse.Timeout:
            case RunResponse.AlreadyShutDown:
            default:
                payload = LogArgs.New(this.logger, LogLevel.Critical, app: app);
                var ex = new UnhandledResponseException(response, this.connectionState);
                Log.UnhandledResponse(this.logger, ex, methodName, payload, executionPath);
                throw ex;
        }
    }

    /// <summary>
    ///   Accesses <see cref="connectionState"/> - must be within <see cref="stateLock"/> scope!
    /// </summary>
    /// <param name="response"></param>
    /// <param name="app"></param>
    /// <param name="executionPath"></param>
    /// <param name="paramName"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    private (App, RunResponse) HandleVmStateResponse(
        (App, RunResponse) result,
        string executionPath,
        [CallerMemberName] string methodName = ""
    )
    {
        (var app, var response) = result;
        LogArgs payload;

        switch (response)
        {
            case RunResponse.NotResponding:
            case RunResponse.NotRunning:
            case RunResponse.Hidden:
            case RunResponse.Ok:
                payload = LogArgs.New(this.logger, LogLevel.Debug, app: app, state: response);
                Log.RemoteMethodSuccess(this.logger, LogLevel.Debug, methodName, payload, executionPath);
                return result;

            case RunResponse.NotInstalled:
                payload = LogArgs.New(this.logger, LogLevel.Error, app: app);
                var ex1 = new AppNotInstalledException(app);
                Log.AppCriticalState(this.logger, ex1, methodName, payload, executionPath);
                throw ex1;

            case RunResponse.UnknownApp:
            case RunResponse.Error:
            case RunResponse.Timeout:
            case RunResponse.AlreadyShutDown:
            default:
                payload = LogArgs.New(this.logger, LogLevel.Critical, app: app);
                var ex = new UnhandledResponseException(response, this.connectionState);
                Log.UnhandledResponse(this.logger, ex, methodName, payload, executionPath);
                throw ex;
        }
    }
}
