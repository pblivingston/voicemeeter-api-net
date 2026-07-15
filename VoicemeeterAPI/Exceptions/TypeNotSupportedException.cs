// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

public class TypeNotSupportedException(Type type, string paramName, Type[] supportedTypes)
    : TypeException(type, $"'{paramName}' was not a supported type.", paramName)
{
    public Type[] SupportedTypes { get; } = supportedTypes;

    public override string Message
        => base.Message + Environment.NewLine +
            $"Supported types: {Utilities.SupportedTypes.ListString(this.SupportedTypes)}";
}
