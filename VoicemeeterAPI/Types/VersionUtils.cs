// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;

namespace PBLivingston.VoicemeeterAPI.Types;

public static class VersionUtils
{
    private static bool InByte(int value) => (uint)value <= 0xFF;

    public static bool IsValid(int maj, int min, int pat)
        => InByte(maj)
        && InByte(min)
        && InByte(pat)
        && (maj | min | pat) > 0;

    public static int RawPack(int kind, int maj, int min, int pat)
        => (kind << 24) | (maj << 16) | (min << 8) | pat;

    public static void RawUnpack(int packed, out int kind, out int maj, out int min, out int pat)
    {
        kind = (packed >> 24) & 0xFF;
        maj = (packed >> 16) & 0xFF;
        min = (packed >> 8) & 0xFF;
        pat = packed & 0xFF;
    }

    public static string ToString(int packed)
    {
        RawUnpack(packed, out int kind, out int maj, out int min, out int pat);
        return $"{kind}.{maj}.{min}.{pat}";
    }

    public static bool TryParse(string s, out int kind, out int maj, out int min, out int pat)
    {
        kind = 0; maj = 0; min = 0; pat = 0;

        if (string.IsNullOrWhiteSpace(s)) return false;
        var parts = s.Split('.');
        var l = parts.Length;
        if (l is not (3 or 4)) return false;

        var k = 0;
        if (l == 4 && !(int.TryParse(parts[0], out k) && KindUtils.IsValid(k))) return false;
        if (!int.TryParse(parts[l - 3], out int m)) return false;
        if (!int.TryParse(parts[l - 2], out int n)) return false;
        if (!int.TryParse(parts[l - 1], out int p)) return false;
        if (!IsValid(m, n, p)) return false;

        kind = k; maj = m; min = n; pat = p;
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
}
