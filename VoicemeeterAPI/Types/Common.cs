// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace VoicemeeterAPI.Types
{
    public static class Utils
    {
        /// <summary>
        ///   Checks if a value is positive and fits in a byte.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool InByte(int value) => (uint)value <= 0xFF;
    }

    public static partial class IntExtensions
    {
        /// <inheritdoc cref="Utils.InByte(int)"/>
        public static bool InByte(this int value) => Utils.InByte(value);
    }
}