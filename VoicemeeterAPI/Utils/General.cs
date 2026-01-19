// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;
using VoicemeeterAPI.Types;

namespace VoicemeeterAPI.Utils
{
    internal static class GeneralUtils
    {
        public static bool InByte(int value) => (uint)value <= 0xFF; // Check if value fits in a byte (0-255);
    }
}