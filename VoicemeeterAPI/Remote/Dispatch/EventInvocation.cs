// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

public partial class Remote
{
    private void OnConnectionStateChanged(ConnectionState previousState, ConnectionState currentState)
    {
        if (this.IsDisposed || previousState == currentState)
        {
            return;
        }

        this.ConnectionStateChanged?.Invoke(this, new(previousState, currentState));
    }

    private void OnParamsDirty(Result<Response, bool> dirty)
        => this.RaiseIfDirty(this.ParamsDirty, dirty);

    private void OnButtonsDirty(Result<Response, bool> dirty)
        => this.RaiseIfDirty(this.ButtonsDirty, dirty);

    private void RaiseIfDirty(EventHandler? handler, Result<Response, bool> dirty)
    {
        if (this.IsDisposed || dirty.IsFailure || !dirty)
        {
            return;
        }

        handler?.Invoke(this, EventArgs.Empty);
    }
}
