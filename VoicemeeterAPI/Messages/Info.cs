// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace VoicemeeterAPI.Messages
{
    internal static class VmApiInfo
    {
        public static void Write(string? domain, string message)
            => VmApiMessage.Write(domain, "Info", message);

        public static void Write(string? domain, string format, params object[] args)
            => VmApiMessage.Write(domain, "Info", format, args);

        public static void Write(string message)
            => Write(null, message);

        public static void Write(string format, params object[] args)
            => Write(null, format, args);
    }

    internal static class RemoteInfo
    {
        public static void Write(string message)
            => VmApiInfo.Write("Remote", message);

        public static void Write(string format, params object[] args)
            => VmApiInfo.Write("Remote", format, args);
    }

    internal static class VoicemeeterInfo
    {
        public static void Write(string message)
            => VmApiInfo.Write("Voicemeeter", message);

        public static void Write(string format, params object[] args)
            => VmApiInfo.Write("Voicemeeter", format, args);
    }
}