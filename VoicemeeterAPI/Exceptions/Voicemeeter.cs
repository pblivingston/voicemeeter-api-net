// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;

namespace VoicemeeterAPI.Exceptions
{
    internal class VoicemeeterException(string m)
        : Exception($"Voicemeeter Error: {m}")
    {
    }
}