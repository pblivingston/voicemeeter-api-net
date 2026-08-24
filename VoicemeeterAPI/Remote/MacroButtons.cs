// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

public partial class Remote
{
    #region MacroButtons Is Dirty

    /// <inheritdoc cref="IRemote.IsButtonsDirty()"/>
    internal Result<Response, bool> ButtonsDirty_i(string executionPath)
    {
        using var lk = this.bDirtyLock.EnterScope();

        var response = this.wrapper.MacroButtonIsDirty();

        return this.HandleDirtyResponse(response, Utilities.BuildPath(executionPath));
    }

    /// <inheritdoc/>
    public Result<Response, bool> IsButtonsDirty()
    {
        using var scope = this.BeginCallScope();

        var result = this.ButtonsDirty_i(nameof(this.IsButtonsDirty));

        this.OnButtonsDirty(result);

        return result;
    }

    #endregion
}
