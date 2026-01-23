// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;
using VoicemeeterAPI.Types;

namespace VoicemeeterAPI.Utils
{
    public static class AppUtils
    {
        /// <summary>
        ///   If the given app is a Voicemeeter app, ensures it is the correct bit version based on the current operating system.
        /// </summary>
        /// <param name="app"></param>
        /// <returns>A correct OS-biased Voicemeeter app. Otherwise, the given app.</returns>
        public static int BitAdjust(int app)
            => app is > 0 and <= 3 && Environment.Is64BitOperatingSystem ? app + 3 // Adjust for 64-bit OS
            : app is > 3 and <= 6 && !Environment.Is64BitOperatingSystem ? app - 3 // Adjust for 32-bit OS
            : app;

        /// <inheritdoc cref="BitAdjust(int)"/>
        public static App BitAdjust(App app) => (App)BitAdjust((int)app);

        /// <summary>
        ///   Attempts to parse the given string into an <see cref="App"/> and ensures a Voicemeeter app is the correct bit version based on the current operating system.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="app"></param>
        /// <returns></returns>
        public static bool TryParseBit(string s, out App app)
        {
            app = App.None;
            if (!Enum.TryParse(s, true, out App result)) return false;
            app = BitAdjust(result);
            return true;
        }

        /// <summary>
        ///   Parses the given string into an <see cref="App"/> and ensures a Voicemeeter app is the correct bit version based on the current operating system.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static App ParseBit(string s) => TryParseBit(s, out App app) ? app
            : throw new ArgumentException($"Invalid app: {s}", nameof(s));
    }
}