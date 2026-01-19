// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace VoicemeeterAPI.Types
{
    /// <summary>
    ///   Voicemeeter applications that can be run with <see cref="Remote.RunVoicemeeter(int)"/> and kinds returned by <see cref="Remote.GetVoicemeeterKind()"/>.
    /// </summary>
    /// <remarks>
    ///   Standard = 1, Banana = 2, and Potato = 3 represent both the 32-bit executables and the OS-agnostic abstractions returned by <see cref="Remote.GetVoicemeeterKind()"/>.
    ///   If running Standard, Banana, or Potato, <see cref="Remote.RunVoicemeeter(int)"/> will automatically adjust for OS bitness where necessary.
    /// </remarks>
    public enum Kind
    {
        None = 0,
        Standard = 1,
        Banana = 2,
        Potato = 3,
        Standardx64 = 4,
        Bananax64 = 5,
        Potatox64 = 6,
        DeviceCheck = 10,
        MacroButtons = 11,
        StreamerView = 12,
        BUSMatrix8 = 13,
        BUSGEQ15 = 14,
        VBAN2MIDI = 15,
        CABLEControlPanel = 20,
        AUXControlPanel = 21,
        VAIO3ControlPanel = 22,
        VAIOControlPanel = 23,
        Unknown
    }
}