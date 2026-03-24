// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;

namespace PBLivingston.VoicemeeterAPI.Types;

/// <summary>
///   Voicemeeter kinds returned by <see cref="Remote.GetKind()"/>.
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
    /// <inheritdoc cref="KindUtils.ToApp(Kind)"/>
    public static App ToApp(this Kind kind) => KindUtils.ToApp(kind);

    /// <inheritdoc cref="KindUtils.IsValid(Kind)"/>
    public static bool IsValid(this Kind kind) => KindUtils.IsValid(kind);
}

public static class KindUtils
{
    public static bool IsKindType(Type t)
        => t == typeof(int) || t == typeof(Kind);

    /// <summary>
    ///   Converts given Voicemeeter <see cref="Kind"/> to an OS-biased Voicemeeter <see cref="App"/>.
    /// </summary>
    /// <param name="kind"></param>
    /// <returns><see cref="App.Unknown"/> if not a valid Voicemeeter <see cref="Kind"/></returns>
    public static App ToApp(Kind kind)
        => kind is < Kind.None or > Kind.Potato ? App.Unknown
        : ((App)kind).BitAdjust();

    public static bool IsValid(int kind)
        => kind is >= 1 and <= 3;

    public static bool IsValid(Kind kind)
        => IsValid((int)kind);
}