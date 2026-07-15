// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI.Exceptions;

public class CannotParseAsPartsException(string actualValue, string paramName)
    : VmArgumentException("Cannot parse string as requested version parts", paramName)
{
    public string ActualValue { get; } = actualValue;

    public override string Message
        => base.Message + Environment.NewLine +
            $"Actual Value: '{this.ActualValue}'";
}
