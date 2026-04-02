// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI.Exceptions;

internal class VoicemeeterException(string m)
    : VmApiException($"Voicemeeter Error: {m}")
{
}