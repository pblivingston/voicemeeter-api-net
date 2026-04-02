// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using PBLivingston.VoicemeeterAPI.Types;
using PBLivingston.VoicemeeterAPI.Exceptions;

namespace PBLivingston.VoicemeeterAPI;

partial class Remote
{
    #region MacroButton Is Dirty

    /// <inheritdoc/>
    public bool ButtonsDirty()
    {
        LoginGuard();

        var result = _vmrApi.MacroButtonIsDirty();

        return result switch
        {
            Response.Ok => false,
            Response.Dirty => true,
            _ => throw new RemoteException($"ButtonsDirty failed - {result}")
        };
    }

    #endregion
}
