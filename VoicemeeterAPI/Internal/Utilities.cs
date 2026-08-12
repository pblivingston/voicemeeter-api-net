// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI.Internal;

using System.Runtime.CompilerServices;

internal static class Utilities
{
    public static bool InByte<T>(T value) where T : struct
        => typeof(T) is var t && Unsafe.SizeOf<T>() switch
        {
            _ when t == typeof(float) => Unsafe.As<T, float>(ref value) is var f
                && f is >= 0f and <= 255f && f % 1 == 0,

            _ when t == typeof(double) => Unsafe.As<T, double>(ref value) is var d
                && d is >= 0d and <= 255d && d % 1 == 0,

            _ when t == typeof(decimal) => Unsafe.As<T, decimal>(ref value) is var m
                && m is >= 0m and <= 255m && m % 1 == 0,

            1 => Unsafe.As<T, sbyte>(ref value) >= 0,
            2 => Unsafe.As<T, ushort>(ref value) <= 0xFF,
            4 => Unsafe.As<T, uint>(ref value) <= 0xFF,
            8 => Unsafe.As<T, ulong>(ref value) <= 0xFF,
            16 => Unsafe.As<T, UInt128>(ref value) <= 0xFF,

            _ => false
        };

    public static void ThrowIfNotInByte(int value, [CallerArgumentExpression(nameof(value))] string paramName = "")
    {
        if (!InByte(value))
        {
            throw new ArgumentOutOfRangeException(paramName, value, "Value must fit in a byte.");
        }
    }

    public static void ThrowIfNotInRange<T>(T value, T min, T max, [CallerArgumentExpression(nameof(value))] string paramName = "")
        where T : struct
    {
        if (Comparer<T>.Default.Compare(value, min) < 0 || Comparer<T>.Default.Compare(value, max) > 0)
        {
            throw new ArgumentOutOfRangeException(paramName, value, $"Value must be at least '{min}' and at most '{max}'.");
        }
    }

    public static string BuildPath(string executionPath, [CallerMemberName] string methodName = "")
        => executionPath + "/" + methodName;
}
