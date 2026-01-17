// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace VoicemeeterAPI.Messages
{
    /// <summary>
    ///   Static class for writing warning messages to the console with a consistent format.
    /// </summary>
    /// <remarks><see cref="VmApiMessage"/></remarks>
    internal static class VmApiWarning
    {
        /// <summary>
        ///   Writes a message to the console with an optional domain prefix and a "Warning" level.
        /// </summary>
        /// <inheritdoc cref="VmApiMessage.Write(string?, string?, string)" path="param[@name='domain']"/>
        /// <param name="message"></param>
        public static void Write(string? domain, string message)
            => VmApiMessage.Write(domain, "Warning", message);

        /// <inheritdoc cref="Write(string?, string)" path="/summary"/>
        /// <inheritdoc cref="VmApiMessage.Write(string?, string?, string)" path="param[@name='domain']"/>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Write(string? domain, string format, params object[] args)
            => VmApiMessage.Write(domain, "Warning", format, args);

        /// <summary>
        ///   Writes a message to the console with "[VoicemeeterAPI] Warning" prefix.
        /// </summary>
        /// <param name="message"></param>
        public static void Write(string message)
            => Write(null, message);

        /// <inheritdoc cref="Write(string)" path="/summary"/>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Write(string format, params object[] args)
            => Write(null, format, args);
    }
}