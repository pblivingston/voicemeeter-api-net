// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;

namespace VoicemeeterAPI.Messages
{
    /// <summary>
    ///   Static class for writing messages to the console with a consistent format.
    /// </summary>
    internal static class VmApiMessage
    {
        /// <summary>
        ///   Writes a message to the console with an optional domain and level prefix.
        /// </summary>
        /// <param name="domain">
        ///   The source of the message (e.g., "Remote", "Voicemeeter", "MacroButtons", etc.).
        ///   Can be null or empty for general messages.
        /// </param>
        /// <param name="level">
        ///   The level of the message (e.g., "Info", "Warning", etc.).
        ///   Can be null or empty for general messages.
        /// </param>
        /// <param name="message"></param>
        internal static void Write(string? domain, string? level, string message)
        {
            var prefix = $"[VoicemeeterAPI]";
            if (!string.IsNullOrWhiteSpace(domain)) prefix += $" {domain}";
            if (!string.IsNullOrWhiteSpace(level)) prefix += $" {level}";
            else prefix += $" Message";
            Console.WriteLine($"{prefix}: {message}");
        }

        /// <inheritdoc cref="Write(string?, string?, string)" path="/summary"/>
        /// <inheritdoc cref="Write(string?, string?, string)" path="param[@name='domain']"/>
        /// <inheritdoc cref="Write(string?, string?, string)" path="param[@name='level']"/>
        /// <param name="format"></param>
        /// <param name="args"></param>
        internal static void Write(string? domain, string? level, string format, params object[] args)
            => Write(domain, level, string.Format(format, args));

        /// <summary>
        ///   Writes a message to the console with default "[VoicemeeterAPI] Message" prefix.
        /// </summary>
        /// <param name="message"></param>
        internal static void Write(string message)
            => Write(null, null, message);

        /// <inheritdoc cref="Write(string)" path="/summary"/>
        /// <param name="format"></param>
        /// <param name="args"></param>
        internal static void Write(string format, params object[] args)
            => Write(null, null, string.Format(format, args));
    }
}