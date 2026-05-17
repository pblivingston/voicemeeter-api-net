// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI.Logging;

using Microsoft.Extensions.Logging;

internal static partial class RemoteLog
{
    [LoggerMessage(
        EventId = (int)Event.MbRunning_NotRunning,
        EventName = nameof(Event.MbRunning_NotRunning),
        Message = "MacroButtons is not running."
    )]
    public static partial void MbRunning_NotRunning(ILogger logger, LogLevel level);
}
