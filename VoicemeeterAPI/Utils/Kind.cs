// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;
using VoicemeeterAPI.Types;

namespace VoicemeeterAPI.Utils
{
    internal static class KindUtils
    {
        /// <summary>
        ///   Attempts to parse the given string into a <see cref="Kind"/>.
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static Kind Parse(string kind)
        {
            if (string.IsNullOrWhiteSpace(kind))
                throw new ArgumentNullException(nameof(kind));

            if (!Enum.TryParse(kind, true, out Kind k))
                throw new ArgumentException($"Invalid Voicemeeter kind: {kind}", nameof(kind));

            return k;
        }

        /// <summary>
        ///   Adjusts the given Voicemeeter kind value to the correct bit version based on the current operating system.
        /// </summary>
        /// <param name="kind">OS-biased or OS-agnostic Voicemeeter kind.</param>
        /// <returns>Correct OS-biased Voicemeeter kind.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int ToBit(int kind)
        {
            if (kind < 1 || kind > 6)
                throw new ArgumentOutOfRangeException(nameof(kind), $"Invalid Voicemeeter kind: {kind}");

            return kind <= 3 && Environment.Is64BitOperatingSystem ? kind + 3 // Adjust for 64-bit versions
                : kind > 3 && !Environment.Is64BitOperatingSystem ? kind - 3 // Adjust for 32-bit versions
                : kind;
        }

        /// <inheritdoc cref="ToBit(int)"/>
        public static Kind ToBit(Kind kind) => (Kind)ToBit((int)kind);

        /// <summary>
        ///   Adjusts the given Voicemeeter kind value from the bit version to the normal version.
        /// </summary>
        /// <param name="kind">OS-biased Voicemeeter kind.</param>
        /// <returns>OS-agnostic Voicemeeter kind.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int FromBit(int kind)
        {
            if (kind < 1 || kind > 6)
                throw new ArgumentOutOfRangeException(nameof(kind), $"Invalid Voicemeeter kind: {kind}");

            return kind > 3 ? kind - 3 : kind;
        }

        /// <inheritdoc cref="FromBit(int)"/>
        public static Kind FromBit(Kind kind) => (Kind)FromBit((int)kind);
    }
}