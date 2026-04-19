// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using Microsoft.Extensions.Logging;

namespace PBLivingston.VoicemeeterAPI.EventManagement.Logging;

internal static partial class GeneralLog
{
    [LoggerMessage(
        EventId = (int)Event.BitAdjust,
        EventName = nameof(Event.BitAdjust),
        Level = LogLevel.Debug,
        Message = "App adjusted for OS bitness. Received: {AppNameBefore}; Returned: {AppNameAfter}"
    )]
    public static partial void BitAdjust(ILogger logger, string appNameBefore, string appNameAfter);

    [LoggerMessage(
        EventId = (int)Event.TypeNotSupported,
        EventName = nameof(Event.TypeNotSupported),
        Level = LogLevel.Error,
        Message = "Type '{TypeName}' not supported for {MethodName} param '{ParamName}'. Supported types: {SupportedTypes}"
    )]
    public static partial void TypeNotSupported(ILogger logger, string typeName, string methodName, string paramName, string supportedTypes);

    [LoggerMessage(
        EventId = (int)Event.CannotParseAsType,
        EventName = nameof(Event.CannotParseAsType),
        Level = LogLevel.Error,
        Message = "'{Value}' is not a valid {TypeName}."
    )]
    public static partial void CannotParseAsType(ILogger logger, string value, string typeName);
}