// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using Microsoft.Extensions.Logging;
using PBLivingston.VoicemeeterAPI.EventManagement.Logging;

namespace PBLivingston.VoicemeeterAPI.EventManagement;

internal static partial class RemoteDispatch
{
    public static void MbRunning_NotRunning(ILogger logger, LogLevel level)
    {
        RemoteLog.MbRunning_NotRunning(logger, level);
    }
}