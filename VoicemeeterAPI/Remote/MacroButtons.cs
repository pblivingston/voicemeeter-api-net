// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using PBLivingston.VoicemeeterAPI.Types;
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

        On_Query_Start(info);

        var result = _vmrApi.MacroButtonIsRunning();

        bool running;
        switch (result)
        {
            case RunResponse.Ok:
                On_Query_Success(RunResponse.Ok, info);
                running = true;
                break;

            case RunResponse.NotRunning:
                On_MbRunning_NotRunning(warning);
                running = false;
                break;

            default:
                throw On_Method_Error(result);
        }

        if (running != _lastState.MacroButtonsIsRunning)
            On_ConnectionState_StateMismatch(running, warning);

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

        On_Query_Start(level);

        var result = _vmrApi.MacroButtonIsDirty();

        switch (result)
        {
            case Response.Ok:
                On_Query_Success(Response.Ok, level);
                return false;

            case Response.Dirty:
                On_ButtonsDirty(level);
                return true;

            default:
                throw On_Method_Error(result);
        }
    }

    /// <inheritdoc/>
    public bool IsButtonsDirty()
    {
        using var scope = BeginInstanceScope();

        return Query_ButtonsDirty();
    }

    #endregion
}
