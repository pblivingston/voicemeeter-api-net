// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

public partial class Remote
{
    #region MacroButtons Is Dirty

    /// <inheritdoc cref="IRemote.IsButtonsDirty()"/>
    internal bool ButtonsDirty_i(string executionPath)
    {
        Response result;
        using (this.bDirtyLock.EnterScope())
        {
            result = this.wrapper.MacroButtonIsDirty();
        }

        return this.HandleResponse(result, Utilities.BuildPath(executionPath));
    }

    /// <inheritdoc/>
    public bool IsButtonsDirty()
    {
        using var scope = this.BeginCallScope();

        return this.ButtonsDirty_i(nameof(this.IsButtonsDirty));
    }

    #endregion
}
