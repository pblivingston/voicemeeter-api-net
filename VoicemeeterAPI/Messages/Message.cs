// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;

namespace VoicemeeterAPI.Messages
{
    internal static class VmApiMessage
    {
        public static void Write(string? domain, string? level, string message)
        {
            var prefix = $"[VoicemeeterAPI]";
            if (!string.IsNullOrWhiteSpace(domain)) prefix += $" {domain}";
            if (!string.IsNullOrWhiteSpace(level)) prefix += $" {level}";
            else prefix += $" Message";
            Console.WriteLine($"{prefix}: {message}");
        }

        public static void Write(string? domain, string? level, string format, params object[] args)
            => Write(domain, level, string.Format(format, args));

        public static void Write(string message)
            => Write(null, null, message);

        public static void Write(string format, params object[] args)
            => Write(null, null, string.Format(format, args));
    }

    internal static class RemoteMessage
    {
        public static void Write(string? level, string message)
            => VmApiMessage.Write("Remote", level, message);

        public static void Write(string? level, string format, params object[] args)
            => VmApiMessage.Write("Remote", level, format, args);

        public static void Write(string message)
            => Write(null, message);

        public static void Write(string format, params object[] args)
            => Write(null, format, args);
    }

    internal static class VoicemeeterMessage
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
}