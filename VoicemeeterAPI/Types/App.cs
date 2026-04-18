// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI.Types;

/// <summary>
///   Applications that can be run with <see cref="Remote.Run(App, int , int)"/>.
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
    /// <inheritdoc cref="AppUtils.ToKind(App)"/>
    public static Kind ToKind(this App app) => AppUtils.ToKind(app);

    /// <inheritdoc cref="AppUtils.BitAdjust(App)"/>
    public static App BitAdjust(this App app) => AppUtils.BitAdjust(app);
}

public static class AppUtils
{
    /// <summary>
    ///   Converts given Voicemeeter <see cref="App"/> to a Voicemeeter <see cref="Kind"/>.
    /// </summary>
    /// <param name="app"></param>
    /// <returns><see cref="Kind.Unknown"/> if not a Voicemeeter <see cref="App"/></returns>
    public static Kind ToKind(App app)
        => app is < App.None or > App.Potatox64 ? Kind.Unknown
        : app >= App.Standardx64 ? (Kind)(app - 3) // 64-bit App -> 32-bit App
        : (Kind)app;

    /// <summary>
    ///   If the given app is a Voicemeeter app, adjusts to the correct bit version based on the current operating system.
    /// </summary>
    /// <param name="app"></param>
    /// <param name="is64Bit">Defaults to OS bitness</param>
    /// <returns>The given app if not a Voicemeeter app or already correct</returns>
    public static int BitAdjust(int app, bool? is64Bit = null)
    {
        var is64 = is64Bit ?? Environment.Is64BitOperatingSystem;
        return app switch
        {
            > 0 and <= 3 when is64 => app + 3, // Adjust for 64-bit OS
            > 3 and <= 6 when !is64 => app - 3, // Adjust for 32-bit OS
            _ => app
        };
    }

    /// <inheritdoc cref="BitAdjust(int, bool?)"/>
    public static App BitAdjust(App app) => (App)BitAdjust((int)app);

    /// <summary>
    ///   Attempts to parse the given string into an <see cref="App"/> and ensures a Voicemeeter app is the correct bit version based on the current operating system.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="app"></param>
    /// <returns></returns>
    public static bool TryParseBit(string s, out App app)
    {
        app = App.None;
        if (!Enum.TryParse(s, true, out App result)) return false;
        app = BitAdjust(result);
        return true;
    }

    /// <summary>
    ///   Parses the given string into an <see cref="App"/> and ensures a Voicemeeter app is the correct bit version based on the current operating system.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static App ParseBit(string s) => TryParseBit(s, out App app) ? app
        : throw new ArgumentException($"Invalid app: {s}", nameof(s));
}
