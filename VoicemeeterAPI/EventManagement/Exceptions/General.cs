// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;

namespace PBLivingston.VoicemeeterAPI.EventManagement.Exceptions;

internal class VmApiException : Exception
{
    public VmApiException(string message, Exception innerException)
        : base(message, innerException)
    { }

    public VmApiException(string message)
        : base(message)
    { }

    public VmApiException()
        : base()
    { }
}

internal class TypeNotSupportedException(Type type, string paramName)
    : VmApiException($"Type: {type.Name}\r\nParameter name: {paramName}")
{
    public string ParamName { get; } = paramName;
    public Type Type { get; } = type;
}