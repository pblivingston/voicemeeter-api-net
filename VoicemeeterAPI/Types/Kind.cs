// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

/// <summary>
///   Voicemeeter kind
/// </summary>
public enum Kind
{
    Unknown = -1,
    None = 0,
    Standard = 1,
    Banana = 2,
    Potato = 3
}

public static class KindExt
{
    /// <summary>
    ///   Converts Voicemeeter kind to the corresponding Voicemeeter application.
    /// </summary>
    /// <param name="kind"></param>
    /// <param name="is64Bit">Defaults to OS bitness</param>
    /// <returns><see cref="App.Unknown"/> if not a valid Voicemeeter kind</returns>
    public static App ToApp(this Kind kind, bool? is64Bit = null)
    {
        if (kind is < Kind.None or > Kind.Potato)
        {
            return App.Unknown;
        }

        return is64Bit ?? Environment.Is64BitOperatingSystem
            ? (App)kind + 3
            : (App)kind;
    }

    /// <summary>
    ///   True if kind is a defined <see cref="Kind"/> greater than <see cref="Kind.None"/>.
    /// </summary>
    /// <param name="kind"></param>
    /// <returns></returns>
    public static bool IsValid(this Kind kind)
    {
        if (kind <= Kind.None)
        {
            return false;
        }

#if NET5_0_OR_GREATER
        return Enum.IsDefined(kind);
#else
        return Enum.IsDefined(typeof(Kind), kind);
#endif
    }
}
