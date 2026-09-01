// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

/// <summary>
///   VB-Audio application
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
    /// <summary>
    ///   Converts Voicemeeter application to the corresponding Voicemeeter kind.
    /// </summary>
    /// <param name="app"></param>
    /// <returns><see cref="Kind.Unknown"/> if not a Voicemeeter application or <see cref="App.None"/></returns>
    public static Kind ToKind(this App app)
    {
        if (!(app.IsVoicemeeter() || app is App.None))
        {
            return Kind.Unknown;
        }

        return app >= App.Standardx64
            ? (Kind)(app - 3) // 64-bit App -> 32-bit App
            : (Kind)app;
    }

    /// <summary>
    ///   True if application is a defined <see cref="App"/> greater than <see cref="App.None"/>.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static bool IsValid(this App app)
    {
        if (app <= App.None)
        {
            return false;
        }

#if NET5_0_OR_GREATER
        return Enum.IsDefined(app);
#else
        return Enum.IsDefined(typeof(App), app);
#endif
    }

    /// <summary>
    ///   True if application is a Voicemeeter application (<see cref="App.Standard"/>, <see cref="App.Potatox64"/>, etc.).
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static bool IsVoicemeeter(this App app)
        => app is >= App.Standard and <= App.Potatox64;
}
