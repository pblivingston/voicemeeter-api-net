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

    private void OnParamsDirty()
    {
        if (this.IsDisposed)
        {
            return;
        }

        this.ParamsDirty?.Invoke(this, EventArgs.Empty);
    }

    private void OnButtonsDirty()
    {
        if (this.IsDisposed)
        {
            return;
        }

        this.ButtonsDirty?.Invoke(this, EventArgs.Empty);
    }
}
