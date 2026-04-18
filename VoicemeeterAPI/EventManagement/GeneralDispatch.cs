// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using PBLivingston.VoicemeeterAPI.EventManagement.Exceptions;
using PBLivingston.VoicemeeterAPI.EventManagement.Logging;
using PBLivingston.VoicemeeterAPI.Types;
using PBLivingston.VoicemeeterAPI.Utilities;

namespace PBLivingston.VoicemeeterAPI.EventManagement;

internal static class GeneralDispatch
{
    public static void BitAdjust(ILogger logger, App appBefore, App appAfter)
    {
        GeneralLog.BitAdjust(logger, appBefore.ToString(), appAfter.ToString());
    }

    public static void TypeNotSupported(ILogger logger, Type type, string paramName, Type[] supportedTypes, [CallerMemberName] string methodName = "")
    {
        GeneralLog.TypeNotSupported(logger, type.Name, methodName, paramName, SupportedTypes.ListString(supportedTypes));

        throw new TypeNotSupportedException(type, paramName);
    }

    public static void CannotParseString(ILogger logger, string value, Type enumType, string paramName)
    {
        GeneralLog.CannotParseString(logger, value, enumType.Name);

        throw new ArgumentException($"Invalid string: {value}", paramName);
    }
}