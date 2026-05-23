// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI.Utilities;

using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Logging;
using PBLivingston.VoicemeeterAPI.Types;

internal static class GeneralDispatch
{
    public static void On_BitAdjust(ILogger logger, App appBefore, App appAfter)
        => GeneralLog.BitAdjust(logger, appBefore, appAfter);

    public static TypeNotSupportedException On_TypeNotSupported(ILogger logger, Type type, string paramName, Type[] supportedTypes, [CallerMemberName] string methodName = "")
    {
        GeneralLog.TypeNotSupported(logger, type.Name, methodName, paramName, SupportedTypes.ListString(supportedTypes));

        return new TypeNotSupportedException(type, paramName, supportedTypes);
    }

    public static CannotParseAsTypeException On_CannotParseAsType(ILogger logger, string value, Type type, string paramName)
    {
        GeneralLog.CannotParseAsType(logger, value, type.Name);

        return new CannotParseAsTypeException(value, type, paramName);
    }

    public static VmApiArgumentOutOfRangeException On_ArgumentOutOfRange<T>(ILogger logger, T value, string paramName, string message, [CallerMemberName] string methodName = "")
    {
        GeneralLog.ArgumentOutOfRange(logger, value?.ToString() ?? "null", methodName, paramName);

        return new VmApiArgumentOutOfRangeException(paramName, value, message);
    }
}
