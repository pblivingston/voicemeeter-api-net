// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;

namespace VoicemeeterAPI.Types;

public static class VersionUtils
{
    #region Type Validation

    public static bool IsVersionType(Type t)
        => t == typeof(SemVersion) || t == typeof(VmVersion);

    public static bool IsVersionType<T>() where T : struct, IVersion<T>
        => IsVersionType(typeof(T));

    public static bool TryGetVersionType<T>(out Type t) where T : struct, IVersion<T>
    {
        t = typeof(Type);
        if (!IsVersionType<T>()) return false;
        t = typeof(T);
        return true;
    }

    public static Type GetVersionType<T>() where T : struct, IVersion<T>
        => TryGetVersionType<T>(out Type t) ? t
        : throw new NotSupportedException($"Type '{t.Name}' is not supported. Use SemVersion or VmVersion.");

    /// <summary>
    ///   Simply throws if the given type is not supported.
    /// </summary>
    /// <param name="t"></param>
    /// <exception cref="NotSupportedException"></exception>
    public static void ValidateVersionType(Type t)
    { if (!IsVersionType(t)) throw new NotSupportedException($"Type '{t.Name}' is not supported. Use SemVersion or VmVersion."); }

    /// <inheritdoc cref="ValidateVersionType(Type)"/>
    public static void ValidateVersionType<T>() where T : struct, IVersion<T>
        => ValidateVersionType(typeof(T));

    #endregion

    #region Helpers

    public static bool IsValid(int maj, int min, int pat)
        => maj.InByte()
        && min.InByte()
        && pat.InByte();

    public static int RawPack(int kind, int maj, int min, int pat)
        => (kind << 24) | (maj << 16) | (min << 8) | pat;

    public static void RawUnpack(int packed, out int kind, out int maj, out int min, out int pat)
    {
        kind = (packed >> 24) & 0xFF;
        maj = (packed >> 16) & 0xFF;
        min = (packed >> 8) & 0xFF;
        pat = packed & 0xFF;
    }

    public static bool TryParse(string s, out int kind, out int maj, out int min, out int pat)
    {
        kind = 0; maj = 0; min = 0; pat = 0;
        if (string.IsNullOrWhiteSpace(s)) return false;
        var parts = s.Split('.');
        var l = parts.Length;
        if (l is not (3 or 4)) return false;
        if (l == 4 && !(int.TryParse(parts[0], out kind) && KindUtils.IsValid(kind))) return false;
        if (!int.TryParse(parts[l - 3], out maj)) return false;
        if (!int.TryParse(parts[l - 2], out min)) return false;
        if (!int.TryParse(parts[l - 1], out pat)) return false;
        if (!IsValid(maj, min, pat)) return false;
        return true;
    }

    public static void Parse(string s, out int kind, out int maj, out int min, out int pat)
    {
        if (!TryParse(s, out kind, out maj, out min, out pat))
            throw new ArgumentException(nameof(s));
    }

    public static bool TryParse(string s, out int packed)
    {
        packed = 0;
        if (!TryParse(s, out int kind, out int maj, out int min, out int pat)) return false;
        packed = RawPack(kind, maj, min, pat);
        return true;
    }

    public static int Parse(string s)
        => TryParse(s, out int packed) ? packed
        : throw new ArgumentException(nameof(s));

    #endregion
}
