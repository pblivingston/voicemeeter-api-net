// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI.Internal;

using System.Runtime.CompilerServices;

internal static class Utilities
{
    public static bool InByte(int value)
        => (uint)value <= 0xFF;

    public static string BuildPath(string executionPath, [CallerMemberName] string methodName = "")
        => executionPath + "/" + methodName;
}
