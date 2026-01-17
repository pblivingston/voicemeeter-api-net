// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace VoicemeeterAPI.Messages
{
    /// <summary>
    ///   Static class for writing <see cref="Remote"> messages to the console with a consistent format.
    /// </summary>
    /// <remarks><see cref="VmApiMessage"/></remarks>
    internal static class VmApiVmrMessage
    {
        /// <summary>
        ///   Writes a message to the console with an optional level prefix and a "Remote" domain.
        /// </summary>
        /// <inheritdoc cref="VmApiMessage.Write(string?, string?, string)" path="param[@name='level']"/>
        /// <param name="message"></param>
        public static void Write(string? level, string message)
            => VmApiMessage.Write("Remote", level, message);

        /// <inheritdoc cref="Write(string?, string)" path="/summary"/>
        /// <inheritdoc cref="VmApiMessage.Write(string?, string?, string)" path="param[@name='level']"/>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Write(string? level, string format, params object[] args)
            => VmApiMessage.Write("Remote", level, format, args);

        /// <summary>
        ///   Writes a message to the console with "[VoicemeeterAPI] Remote Message" prefix.
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

    /// <summary>
    ///   Static class for writing <see cref="Remote"> informational messages to the console with a consistent format.
    /// </summary>
    /// <remarks><see cref="VmApiInfo"/></remarks>
    internal static class VmApiVmrInfo
    {
        /// <summary>
        ///   Writes a message to the console with "[VoicemeeterAPI] Remote Info" prefix.
        /// </summary>
        /// <param name="message"></param>
        public static void Write(string message)
            => VmApiInfo.Write("Remote", message);

        /// <inheritdoc cref="Write(string)" path="/summary"/>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Write(string format, params object[] args)
            => VmApiInfo.Write("Remote", format, args);
    }

    /// <summary>
    ///   Static class for writing <see cref="Remote"> warning messages to the console with a consistent format.
    /// </summary>
    /// <remarks><see cref="VmApiWarning"/></remarks>
    internal static class VmApiVmrWarning
    {
        /// <summary>
        ///   Writes a message to the console with "[VoicemeeterAPI] Remote Warning" prefix.
        /// </summary>
        /// <param name="message"></param>
        public static void Write(string message)
            => VmApiWarning.Write("Remote", message);

        /// <inheritdoc cref="Write(string)" path="/summary"/>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Write(string format, params object[] args)
            => VmApiWarning.Write("Remote", format, args);
    }
}