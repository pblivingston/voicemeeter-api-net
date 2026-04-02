// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;

namespace PBLivingston.VoicemeeterAPI.Exceptions;

internal abstract class VmApiException(string m)
    : Exception($"[VoicemeeterAPI] {m}")
{
}