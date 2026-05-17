// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI.Logging;

using Microsoft.Extensions.Logging;

internal static partial class LogScope
{
    public static readonly Func<ILogger, Guid, IDisposable?> Instance = LoggerMessage.DefineScope<Guid>("Instance: {GUID}");
    public static readonly Func<ILogger, string, IDisposable?> Method = LoggerMessage.DefineScope<string>("Method: {MethodName}");
}