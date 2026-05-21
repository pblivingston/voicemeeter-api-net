// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using PBLivingston.VoicemeeterAPI.Types;
using Microsoft.Extensions.Logging;

public partial class Remote
{
    #region MacroButtons Is Running

    internal bool Query_ButtonsRunning(bool nested)
    {
        this.LoginGuard(requiredStatus: LoginResponse.VoicemeeterNotRunning);

        var info = nested ? LogLevel.Trace : LogLevel.Information;
        var warning = nested ? LogLevel.Trace : LogLevel.Warning;

        this.On_Query_Start(info);

        var result = this.vmrApi.MacroButtonIsRunning();

        bool running;
        switch (result)
        {
            case RunResponse.Ok:
                this.On_Query_Success(RunResponse.Ok, info);
                running = true;
                break;

            case RunResponse.NotRunning:
                this.On_MbRunning_NotRunning(warning);
                running = false;
                break;

            default:
                throw this.On_Method_Error(result);
        }

        this.On_ConnectionState_StateMismatch(running, warning);

        return running;
    }

    public bool IsButtonsRunning()
    {
        using var scope = this.BeginInstanceScope();

        return this.Query_ButtonsRunning(false);
    }

    #endregion

    #region MacroButtons Is Dirty

    /// <inheritdoc cref="IRemote.IsButtonsDirty()"/>
    internal bool Query_ButtonsDirty()
    {
        this.LoginGuard();

        var level = LogLevel.Trace;

        this.On_Query_Start(level);

        var result = this.vmrApi.MacroButtonIsDirty();

        switch (result)
        {
            case Response.Ok:
                this.On_Query_Success(Response.Ok, level);
                return false;

            case Response.Dirty:
                this.On_ButtonsDirty(level);
                return true;

            default:
                throw this.On_Method_Error(result);
        }
    }

    /// <inheritdoc/>
    public bool IsButtonsDirty()
    {
        using var scope = this.BeginInstanceScope();

        return this.Query_ButtonsDirty();
    }

    #endregion
}
