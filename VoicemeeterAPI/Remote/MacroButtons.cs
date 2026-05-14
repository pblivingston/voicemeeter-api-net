// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using PBLivingston.VoicemeeterAPI.Types;
using PBLivingston.VoicemeeterAPI.EventManagement;
using Microsoft.Extensions.Logging;

namespace PBLivingston.VoicemeeterAPI;

partial class Remote
{
    #region MacroButtons Is Running

    internal bool Query_ButtonsRunning(bool nested)
    {
        LoginGuard(requiredStatus: LoginResponse.VoicemeeterNotRunning);

        var info = nested ? LogLevel.Trace : LogLevel.Information;
        var warning = nested ? LogLevel.Trace : LogLevel.Warning;

        RemoteDispatch.Query_Start(_logger, info);

        var result = _vmrApi.MacroButtonIsRunning();

        bool running;
        switch (result)
        {
            case RunResponse.Ok:
                RemoteDispatch.Query_Success(_logger, info, RunResponse.Ok);
                running = true;
                break;

            case RunResponse.NotRunning:
                RemoteDispatch.MbRunning_NotRunning(_logger, warning);
                running = false;
                break;

            default:
                throw RemoteDispatch.Method_Error(_logger, result);
        }

        if (running != _lastState.MacroButtonsIsRunning)
            RemoteDispatch.ConnectionState_StateMismatch(_logger, warning, nameof(_lastState.MacroButtonsIsRunning), _lastState.MacroButtonsIsRunning);

        return running;
    }

    public bool IsButtonsRunning()
    {
        using var scope = BeginInstanceScope();

        return Query_ButtonsRunning(false);
    }

    #endregion

    #region MacroButtons Is Dirty

    /// <inheritdoc cref="IRemote.IsButtonsDirty()"/>
    internal bool Query_ButtonsDirty()
    {
        LoginGuard();

        var level = LogLevel.Trace;

        RemoteDispatch.Query_Start(_logger, level);

        var result = _vmrApi.MacroButtonIsDirty();

        switch (result)
        {
            case Response.Ok:
                RemoteDispatch.Query_Success(_logger, level, Response.Ok);
                return false;

            case Response.Dirty:
                OnButtonsDirty(level);
                return true;

            default:
                throw RemoteDispatch.Method_Error(_logger, result);
        }
    }

    /// <inheritdoc/>
    public bool IsButtonsDirty()
    {
        using var scope = BeginInstanceScope();

        return Query_ButtonsDirty();
    }

    #endregion

    private void OnButtonsDirty(LogLevel level)
    {
        RemoteDispatch.Dirty_Success(_logger, level, this, ButtonsDirty, nameof(IsButtonsDirty));
    }
}
