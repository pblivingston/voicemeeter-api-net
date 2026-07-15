// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI.Exceptions;

public class VmApiException : Exception
{
    public VmApiException(string? message, Exception innerException)
        : base(message, innerException)
    { }

    public VmApiException(string? message)
        : base(message)
    { }

    public VmApiException()
        : base()
    { }
}
