// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI.Exceptions;

public class CannotParseAsTypeException(string actualValue, Type type, string paramName)
    : TypeException(type, "Cannot parse string as requested type.", paramName)
{
    public string ActualValue { get; } = actualValue;

    public override string Message
        => base.Message + Environment.NewLine +
            $"Actual Value: '{this.ActualValue}'";
}
