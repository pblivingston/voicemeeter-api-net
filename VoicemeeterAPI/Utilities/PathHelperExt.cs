// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;
using AtgDev.Voicemeeter.Utils;

namespace PBLivingston.VoicemeeterAPI.Utilities;

public static class PathHelperExt
{
    public static string GetInstallDirectory()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new PlatformNotSupportedException("Cannot get Voicemeeter installation path on current OS");

        return PathHelper.GetProgramFolder();
    }
}