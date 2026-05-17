// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using Microsoft.Extensions.Logging;
using PBLivingston.VoicemeeterAPI.Logging;
using PBLivingston.VoicemeeterAPI.Types;

public partial class Remote
{
    private void On_MbRunning_NotRunning(LogLevel level = LogLevel.Trace)
        => RemoteLog.MbRunning_NotRunning(this.logger, level);

    private void On_ButtonsDirty(LogLevel level = LogLevel.Trace)
    {
        RemoteLog.Query_Success_Response(this.logger, level, nameof(IsButtonsDirty), Response.Dirty);

        ButtonsDirty?.Invoke(this, new EventArgs());
    }
}
