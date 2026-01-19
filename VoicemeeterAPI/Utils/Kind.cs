// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;
using VoicemeeterAPI.Types;

namespace VoicemeeterAPI.Utils
{
    internal static class KindUtils
    {
        #region Bitness

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

        #endregion

        #region Parsing

        /// <summary>
        ///   Attempts to parse the given string into a <see cref="Kind"/> value and confirms the result is a valid Voicemeeter kind.
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParseAsVm(string kind, out Kind result)
            => Enum.TryParse(kind, true, out result) && result >= Kind.Standard && result <= Kind.Potatox64;

        /// <summary>
        ///   Attempts to parse the given string into an OS-biased <see cref="Kind"/>.
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParseToBit(string kind, out Kind result)
        {
            if (!TryParseAsVm(kind, out Kind k))
            {
                result = k;
                return false;
            }

            result = ToBit(k);
            return true;
        }

        /// <summary>
        ///   Attempts to parse the given string into an OS-agnostic <see cref="Kind"/>.
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParseFromBit(string kind, out Kind result)
        {
            if (!TryParseAsVm(kind, out Kind k))
            {
                result = k;
                return false;
            }

            result = FromBit(k);
            return true;
        }

        #endregion
    }
}