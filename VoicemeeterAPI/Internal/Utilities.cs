// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI.Internal;

using System.Runtime.CompilerServices;

internal static class Utilities
{
    public static bool InByte(int value)
        => (uint)value <= 0xFF;

    public static void ThrowIfNotInByte(int value, [CallerArgumentExpression(nameof(value))] string paramName = "")
    {
        if (!InByte(value))
        {
            throw new ArgumentOutOfRangeException(paramName, value, "Value does not fit in a byte.");
        }
    }

    public static string BuildPath(string executionPath, [CallerMemberName] string methodName = "")
        => executionPath + "/" + methodName;
}
