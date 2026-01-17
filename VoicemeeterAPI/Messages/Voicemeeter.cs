// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace VoicemeeterAPI.Messages
{
    internal static class VmApiVmMessage
    {
        public static void Write(string? level, string message)
            => VmApiMessage.Write("Voicemeeter", level, message);
        public static void Write(string? level, string format, params object[] args)
            => VmApiMessage.Write("Voicemeeter", level, format, args);

        public static void Write(string message)
            => Write(null, message);
        public static void Write(string format, params object[] args)
            => Write(null, format, args);
    }

    internal static class VmApiVmInfo
    {
        public static void Write(string message)
            => VmApiInfo.Write("Voicemeeter", message);
        public static void Write(string format, params object[] args)
            => VmApiInfo.Write("Voicemeeter", format, args);
    }

    internal static class VmApiVmWarning
    {
        public static void Write(string message)
            => VmApiWarning.Write("Voicemeeter", message);
        public static void Write(string format, params object[] args)
            => VmApiWarning.Write("Voicemeeter", format, args);
    }
}