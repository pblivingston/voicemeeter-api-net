// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

public class CannotConvertToTypeException(Type type, string voicemeeterParam, float returnedValue, string paramName)
    : TypeException(type, "Cannot convert Voicemeeter parameter to requested type.", paramName)
{
    public string VoicemeeterParam { get; } = voicemeeterParam;
    public float ReturnedValue { get; } = returnedValue;

    public override string Message
        => $"""
        {base.Message}
        Voicemeeter Param: '{this.VoicemeeterParam}'
        Returned Value: {this.ReturnedValue}
        """;
}
