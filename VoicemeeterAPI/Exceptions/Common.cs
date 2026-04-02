// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Runtime.CompilerServices;
using PBLivingston.VoicemeeterAPI.Utilities;

namespace PBLivingston.VoicemeeterAPI.Exceptions;

internal abstract class VmApiException(string m)
    : Exception($"[VoicemeeterAPI] {m}")
{
}

internal class TypeNotSupportedException<T>(string paramName, Type[] supportedTypes, [CallerMemberName] string methodName = "")
    : VmApiException($"Error: Type '{typeof(T).Name}' not supported for {methodName} param: {paramName}; supported types: {SupportedTypes.ListString(supportedTypes)}")
{
    public string Method { get; } = methodName;
    public string Param { get; } = paramName;
    public Type Type { get; } = typeof(T);
    public Type[] Supported = supportedTypes;
}