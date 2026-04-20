// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

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

    public static TypeNotSupportedException TypeNotSupported(ILogger logger, Type type, string paramName, Type[] supportedTypes, [CallerMemberName] string methodName = "")
    {
        GeneralLog.TypeNotSupported(logger, type.Name, methodName, paramName, SupportedTypes.ListString(supportedTypes));

        return new TypeNotSupportedException(type, paramName, supportedTypes);
    }

    public static CannotParseAsTypeException CannotParseAsType(ILogger logger, string value, Type type, string paramName)
    {
        GeneralLog.CannotParseAsType(logger, value, type.Name);

        return new CannotParseAsTypeException(value, type, paramName);
    }
}