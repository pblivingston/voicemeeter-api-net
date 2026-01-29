// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;

namespace VoicemeeterAPI.Messages
{
    internal static class VmApiMessage
    {
        public static void Write(string message, string? domain = null, string? level = null)
        {
            var d = string.IsNullOrWhiteSpace(domain) ? "" : $"{domain} ";
            var l = string.IsNullOrWhiteSpace(domain) ? "Message" : level;

            Console.WriteLine($"[VoicemeeterAPI] {d}{l}: {message}");
        }
    }

    internal static class VmApiInfo
    {
        public static void Write(string message, string? domain = null)
            => VmApiMessage.Write(message, domain, "Info");
    }

    internal static class VmApiWarning
    {
        public static void Write(string message, string? domain = null)
            => VmApiMessage.Write(message, domain, "Warning");
    }

    internal static class RemoteMessage
    {
        public static void Write(string message, string? level = null)
            => VmApiMessage.Write(message, "Remote", level);
    }

    internal static class RemoteInfo
    {
        public static void Write(string message)
            => VmApiInfo.Write(message, "Remote");
    }

    internal static class RemoteWarning
    {
        public static void Write(string message)
            => VmApiWarning.Write(message, "Remote");
    }

    internal static class VoicemeeterMessage
    {
        public static void Write(string message, string? level = null)
            => VmApiMessage.Write(message, "Voicemeeter", level);
    }

    internal static class VoicemeeterInfo
    {
        public static void Write(string message)
            => VmApiInfo.Write(message, "Voicemeeter");
    }

    internal static class VoicemeeterWarning
    {
        public static void Write(string message)
            => VmApiWarning.Write(message, "Voicemeeter");
    }
}