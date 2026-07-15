// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

public abstract class PackedOutOfRangeException(string paramName, int actualValue, string message)
    : VmArgumentOutOfRangeException(
        message + Environment.NewLine + "Remaining three bytes should be > 0x00_0000.",
        paramName,
        actualValue
    )
{ }

public class VmPackedOutOfRangeException(string paramName, int actualValue)
    : PackedOutOfRangeException(paramName, actualValue, "First byte should be <= 0x03 and > 0x00.")
{ }

public class SemPackedOutOfRangeException(string paramName, int actualValue)
    : PackedOutOfRangeException(paramName, actualValue, "First byte should be 0x00.")
{ }
