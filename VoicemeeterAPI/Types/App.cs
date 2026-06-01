// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI.Types;

using PBLivingston.VoicemeeterAPI.Exceptions;

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
    public static Kind ToKind(this App app)
        => AppUtils.ToKind(app);

    /// <inheritdoc cref="AppUtils.IsValid(App)"/>
    public static bool IsValid(this App app)
        => AppUtils.IsValid(app);

    /// <inheritdoc cref="AppUtils.IsVoicemeeter(App)"/>
    public static bool IsVoicemeeter(this App app)
        => AppUtils.IsVoicemeeter(app);

    /// <inheritdoc cref="AppUtils.BitAdjust(App, bool?)"/>
    public static App BitAdjust(this App app, bool? is64Bit = null)
        => AppUtils.BitAdjust(app, is64Bit);
}

public static class AppUtils
{
    /// <summary>
    ///   Converts given Voicemeeter <see cref="App"/> to a Voicemeeter <see cref="Kind"/>.
    /// </summary>
    /// <param name="app"></param>
    /// <returns><see cref="Kind.Unknown"/> if not a Voicemeeter <see cref="App"/></returns>
    public static Kind ToKind(App app)
        => app.IsVoicemeeter() || app is App.None
            ? app >= App.Standardx64
                ? (Kind)(app - 3) // 64-bit App -> 32-bit App
                : (Kind)app
            : Kind.Unknown;

    public static bool IsValid(App app)
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

    public static bool IsValid(int app)
        => IsValid((App)app);

    public static bool IsVoicemeeter(App app)
        => app is >= App.Standard and <= App.Potatox64;

    public static bool IsVoicemeeter(int app)
        => IsVoicemeeter((App)app);

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
    public static App BitAdjust(App app, bool? is64Bit = null)
        => (App)BitAdjust((int)app, is64Bit);

    /// <summary>
    ///   Attempts to parse the given string into an <see cref="App"/> and ensures a Voicemeeter app is the correct bit version based on the current operating system.
    /// </summary>
    /// <param name="appName"></param>
    /// <param name="app"></param>
    /// <returns></returns>
    public static bool TryParseBit(string appName, out App app, bool? is64Bit = null)
    {
        app = App.None;

        if (!Enum.TryParse(appName, true, out App result))
        {
            return false;
        }

        app = BitAdjust(result, is64Bit);
        return true;
    }

    /// <summary>
    ///   Parses the given string into an <see cref="App"/> and ensures a Voicemeeter app is the correct bit version based on the current operating system.
    /// </summary>
    /// <param name="appName"></param>
    /// <returns></returns>
    /// <exception cref="CannotParseAsTypeException"></exception>
    public static App ParseBit(string appName, bool? is64Bit = null)
        => TryParseBit(appName, out var app, is64Bit)
            ? app
            : throw new CannotParseAsTypeException(appName, typeof(App), nameof(appName));
}
