// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using Microsoft.Extensions.Logging;

public partial class Remote
{
    #region MacroButtons Is Dirty

    /// <inheritdoc cref="IRemote.IsButtonsDirty()"/>
    internal bool ButtonsDirty_i()
    {
        var level = LogLevel.Trace;

        this.On_Query_Start(level);

        Response result;
        using (this.bDirtyLock.EnterScope())
        {
            result = this.wrapper.MacroButtonIsDirty();
        }

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

        return this.ButtonsDirty_i();
    }

    #endregion
}
