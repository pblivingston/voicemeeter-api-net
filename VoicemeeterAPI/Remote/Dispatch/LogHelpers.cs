// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

public partial class Remote
{
    private IDisposable? BeginCallScope()
    {
#if NET7_0_OR_GREATER
        ObjectDisposedException.ThrowIf(this.IsDisposed, this);
#else
        if (this.IsDisposed)
        {
            throw new ObjectDisposedException(nameof(Remote));
        }
#endif

        return Log.CallScope(this.logger, this.instanceId, Guid.NewGuid());
    }

    #region Entry

    private void MethodStart(
        App? app = null,
        Kind? kind = null,
        [CallerMemberName] string methodName = "",
        [CallerMemberName] string executionPath = ""
    )
    {
        var payload = LogArgs.New(this.logger, LogLevel.Information, app: app, kind: kind);
        Log.RemoteMethodStart(this.logger, methodName, payload, executionPath);
    }

    private void WrapperCall(
        string wrapperMethodName,
        string executionPath,
        string? param = null,
        App? app = null,
        bool trace = false,
        [CallerMemberName] string methodName = ""
    )
    {
        var level = trace ? LogLevel.Trace : LogLevel.Debug;
        var payload = LogArgs.New(this.logger, level, param: param, app: app);
        Log.RemoteWrapperCall(this.logger, level, methodName, wrapperMethodName, payload, executionPath);
    }

    private void WaitForRunningStart(
        string targetDescription,
        string executionPath,
        App? app = null,
        [CallerMemberName] string methodName = ""
    )
    {
        var payload = LogArgs.New(this.logger, LogLevel.Information, app: app);
        Log.WaitForRunningStart(this.logger, methodName, targetDescription, payload, executionPath);
    }

    private void YieldForEngineSettle(
        string targetDescription,
        string executionPath,
        [CallerMemberName] string methodName = ""
    ) => Log.YieldForEngineSettle(this.logger, methodName, targetDescription, executionPath);

    #endregion

    #region Exit

    private void WaitForRunningDetected(
        string targetDescription,
        string executionPath,
        RunResponse state,
        VmVersion? version = null,
        App? app = null,
        [CallerMemberName] string methodName = ""
    )
    {
        var payload = LogArgs.New(this.logger, LogLevel.Information, state: state, version: version, app: app);
        Log.WaitForRunningDetected(this.logger, methodName, targetDescription, payload, executionPath);
    }

    #endregion

    #region Dev Error

    private ArgumentException CannotConvertToType<T>(
        string voicemeeterParam,
        float returnedValue,
        string executionPath,
        string paramName = "param",
        [CallerMemberName] string methodName = ""
    )
    {
        var payload = LogArgs.New(this.logger, LogLevel.Error, param: voicemeeterParam, value: returnedValue);
        var message = $"Cannot convert '{voicemeeterParam}' value to '{typeof(T).Name}'.";
        var ex = new ArgumentException(message, paramName);
        Log.RemoteInvalidArgument(this.logger, ex, methodName, payload, executionPath);
        return ex;
    }

    private ArgumentException TypeNotSupported<T>(
        Type[] supportedTypes,
        string executionPath,
        string paramName = "T",
        [CallerMemberName] string methodName = ""
    )
    {
        var ex = SupportedTypes.CreateArgumentException<T>(paramName, supportedTypes);
        Log.RemoteInvalidArgument(this.logger, ex, methodName, LogArgs.Empty, executionPath);
        return ex;
    }

    private InvalidOperationException CannotWaitForEngine(
        App voicemeeterApp,
        string executionPath,
        [CallerMemberName] string methodName = ""
    )
    {
        var payload = LogArgs.New(this.logger, LogLevel.Error, app: voicemeeterApp);
        var ex = new InvalidOperationException($"Cannot wait for Voicemeeter when not logged in. Log in first or use '{nameof(Run)}'.");
        Log.RemoteContractViolation(this.logger, ex, methodName, "Not logged in.", payload, executionPath);
        return ex;
    }

    #endregion
}
