// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using VoicemeeterAPI.Utils;

namespace VoicemeeterAPI.Types
{
    /// <summary>
    ///   Applications that can be run with <see cref="Remote.Run(App)"/>.
    /// </summary>
    public enum App
    {
        Unknown = -1,
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
        VAIOControlPanel = 23
    }

    public static class AppExt
    {
        public static App BitAdjust(this App app) => AppUtils.BitAdjust(app);
    }
}