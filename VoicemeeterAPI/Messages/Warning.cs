// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace VoicemeeterAPI.Messages
{
    internal static class VmApiWarning
    {
        public static void Write(string? domain, string message)
            => VmApiMessage.Write(domain, "Warning", message);

        public static void Write(string? domain, string format, params object[] args)
            => VmApiMessage.Write(domain, "Warning", format, args);

        public static void Write(string message)
            => Write(null, message);

        public static void Write(string format, params object[] args)
            => Write(null, format, args);
    }

    internal static class RemoteWarning
    {
        public static void Write(string message)
            => VmApiWarning.Write("Remote", message);

        public static void Write(string format, params object[] args)
            => VmApiWarning.Write("Remote", format, args);
    }

    internal static class VoicemeeterWarning
    {
        public static void Write(string message)
            => VmApiWarning.Write("Voicemeeter", message);

        public static void Write(string format, params object[] args)
            => VmApiWarning.Write("Voicemeeter", format, args);
    }
}