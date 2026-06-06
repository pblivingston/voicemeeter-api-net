// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using PBLivingston.VoicemeeterAPI.Types;
using Microsoft.Extensions.Logging;

public partial class Remote
{
    #region MacroButtons Is Dirty

    /// <inheritdoc cref="IRemote.IsButtonsDirty()"/>
    internal bool Query_ButtonsDirty()
    {
        this.LoginGuard();

        var level = LogLevel.Trace;

        this.On_Query_Start(level);

        var result = this.wrapper.MacroButtonIsDirty();

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
