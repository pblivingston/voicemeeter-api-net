// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;
using VoicemeeterAPI.Types;
using VoicemeeterAPI.Messages;

namespace VoicemeeterAPI;

partial class Remote
{
    #region MacroButton Is Dirty

    /// <inheritdoc/>
    public bool ButtonsDirty()
    {
        if (_isDisposed) throw new ObjectDisposedException(nameof(Remote));

        if (LoginStatus is not LoginResponse.Ok)
            throw new RemoteAccessException(nameof(ButtonsDirty), LoginStatus);

        var result = (Response)_vmrApi.MacroButtonIsDirty();

        return result switch
        {
            Response.Ok => false,
            Response.Dirty => true,
            _ => throw new RemoteException($"ButtonDirty failed - {result}")
        };
    }

    #endregion
}
