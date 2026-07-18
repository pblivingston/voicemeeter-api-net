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
        string executionPath,
        LogArgs payload = default,
        [CallerMemberName] string methodName = ""
    ) => Log.RemoteMethodStart(this.logger, methodName, payload, executionPath);

    private void WrapperCall(
        string wrapperMethodName,
        string executionPath,
        LogArgs payload = default,
        bool trace = false,
        [CallerMemberName] string methodName = ""
    )
    {
        var info = trace ? LogLevel.Trace : LogLevel.Information;
        Log.RemoteWrapperCall(this.logger, info, methodName, wrapperMethodName, payload, executionPath);
    }

    private void WaitForRunningStart(
        string targetDescription,
        string executionPath,
        LogArgs payload = default,
        [CallerMemberName] string methodName = ""
    ) => Log.WaitForRunningStart(this.logger, methodName, targetDescription, payload, executionPath);

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
        LogArgs payload = default,
        [CallerMemberName] string methodName = ""
    ) => Log.WaitForRunningDetected(this.logger, methodName, targetDescription, payload, executionPath);

    #endregion

    #region Warning

    private void AppUnexpectedState(
        string executionPath,
        LogArgs payload = default,
        [CallerMemberName] string methodName = ""
    ) => Log.AppUnexpectedState(this.logger, methodName, payload, executionPath);

    private void WaitForVoicemeeterLoggedOut(
        string executionPath,
        [CallerMemberName] string methodName = ""
    ) => Log.WaitForVoicemeeterLoggedOut(this.logger, methodName, executionPath);

    private void OperationCanceled(
        string executionPath,
        LogArgs payload = default,
        [CallerMemberName] string methodName = ""
    ) => Log.RemoteOperationCanceled(this.logger, methodName, payload, executionPath);

    #endregion

    #region Dev Error

    private CannotConvertToTypeException CannotConvertToType<T>(
        string voicemeeterParam,
        float returnedValue,
        string executionPath,
        string paramName = "param",
        [CallerMemberName] string methodName = ""
    )
    {
        var ex = new CannotConvertToTypeException(typeof(T), voicemeeterParam, returnedValue, paramName);
        Log.RemoteInvalidArgument(this.logger, ex, methodName, new(voicemeeterParam, returnedValue), executionPath);
        return ex;
    }

    private TypeNotSupportedException TypeNotSupported<T>(
        Type[] supportedTypes,
        string executionPath,
        LogArgs payload = default,
        string paramName = "T",
        [CallerMemberName] string methodName = ""
    )
    {
        var ex = new TypeNotSupportedException(typeof(T), paramName, supportedTypes);
        Log.RemoteInvalidArgument(this.logger, ex, methodName, payload, executionPath);
        return ex;
    }

    #endregion
}
