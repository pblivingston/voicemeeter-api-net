// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

public partial class Remote
{
    #region MacroButtons Is Dirty

    /// <inheritdoc cref="IRemote.IsButtonsDirty()"/>
    internal Result<Response, bool> ButtonsDirty_i(string executionPath)
    {
        Response response;
        using (this.bDirtyLock.EnterScope())
        {
            response = this.wrapper.MacroButtonIsDirty();
        }

        return this.HandleDirtyResponse(response, Utilities.BuildPath(executionPath));
    }

    /// <inheritdoc/>
    public Result<Response, bool> IsButtonsDirty()
    {
        using var scope = this.BeginCallScope();

        return this.ButtonsDirty_i(nameof(this.IsButtonsDirty));
    }

    #endregion
}
