// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

public class VoicemeeterException : VmApiException
{
    public VoicemeeterException(string message)
        : base(message)
    { }

    public VoicemeeterException()
        : base()
    { }
}
