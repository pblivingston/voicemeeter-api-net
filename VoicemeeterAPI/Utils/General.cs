// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;
using VoicemeeterAPI.Types;

namespace VoicemeeterAPI.Utils
{
    internal static class GeneralUtils
    {
        public static bool InByte(int value) => (uint)value <= 0xFF; // Check if value fits in a byte (0-255);

        public static Kind ParseKind(string strKind)
        {
            if (string.IsNullOrWhiteSpace(strKind))
                throw new ArgumentNullException(nameof(strKind));

            if (!Enum.TryParse(strKind, true, out Kind kind))
                throw new ArgumentException($"Invalid Voicemeeter kind: {strKind}", nameof(strKind));

            return kind;
        }

        /// <summary>
        ///   Adjusts the given Voicemeeter kind value to the correct bit version based on the current operating system.
        /// </summary>
        /// <param name="kind">OS-biased or OS-agnostic Voicemeeter kind.</param>
        /// <returns>Correct OS-biased Voicemeeter kind.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int ToBitKind(int kind)
        {
            if (kind < 1 || kind > 6)
                throw new ArgumentOutOfRangeException(nameof(kind), $"Invalid Voicemeeter kind: {kind}");

            return kind <= 3 && Environment.Is64BitOperatingSystem ? kind + 3 // Adjust for 64-bit versions
                : kind > 3 && !Environment.Is64BitOperatingSystem ? kind - 3 // Adjust for 32-bit versions
                : kind;
        }

        /// <inheritdoc cref="ToBitKind(int)"/>
        public static Kind ToBitKind(Kind kind) => (Kind)ToBitKind((int)kind);

        /// <summary>
        ///   Adjusts the given Voicemeeter kind value from the bit version to the normal version.
        /// </summary>
        /// <param name="kind">OS-biased Voicemeeter kind.</param>
        /// <returns>OS-agnostic Voicemeeter kind.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int FromBitKind(int kind)
        {
            if (kind < 1 || kind > 6)
                throw new ArgumentOutOfRangeException(nameof(kind), $"Invalid Voicemeeter kind: {kind}");

            return kind > 3 ? kind - 3 : kind;
        }

        /// <inheritdoc cref="FromBitKind(int)"/>
        public static Kind FromBitKind(Kind kind) => (Kind)FromBitKind((int)kind);
    }
}