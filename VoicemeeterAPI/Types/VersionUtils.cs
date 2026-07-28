// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

public static class VersionUtils
{
    public static int Pack(int kind, int maj, int min, int pat)
    {
        ReadOnlySpan<(string, int)> parts = [
            (nameof(kind), kind),
            (nameof(maj), maj),
            (nameof(min), min),
            (nameof(pat), pat)
        ];
        foreach ((var paramName, var value) in parts)
        {
            if (!Utilities.InByte(value))
            {
                throw new ArgumentOutOfRangeException(paramName, value, "Part does not fit in a byte.");
            }
        }

        return (kind << 24) | (maj << 16) | (min << 8) | pat;
    }

    public static bool TryPack(int kind, int maj, int min, int pat, out int packed)
    {
        try
        {
            packed = Pack(kind, maj, min, pat);
            return true;
        }
        catch
        {
            packed = 0;
            return false;
        }
    }

    public static void Unpack(int packed, out int kind, out int maj, out int min, out int pat)
    {
        kind = (packed >> 24) & 0xFF;
        maj = (packed >> 16) & 0xFF;
        min = (packed >> 8) & 0xFF;
        pat = packed & 0xFF;
    }

    public static void Parse(string s, out int? kind, out int maj, out int min, out int pat)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            throw new ArgumentNullException(nameof(s));
        }

        var k = 0;
        var parts = s.Split('.');
        var l = parts.Length;
        if (
            l is not (3 or 4)
            || (l == 4 && !int.TryParse(parts[0], out k))
            || (!int.TryParse(parts[l - 3], out maj))
            || (!int.TryParse(parts[l - 2], out min))
            || (!int.TryParse(parts[l - 1], out pat))
        )
        {
            throw new ArgumentException($"Cannot parse '{s}' as requested version parts", nameof(s));
        }

        kind = l == 4 ? k : null;
    }

    public static bool TryParse(string s, out int? kind, out int maj, out int min, out int pat)
    {
        try
        {
            Parse(s, out kind, out maj, out min, out pat);
            return true;
        }
        catch
        {
            kind = 0;
            maj = 0;
            min = 0;
            pat = 0;
            return false;
        }
    }

    public static int Parse(string s)
    {
        Parse(s, out var kind, out var maj, out var min, out var pat);
        return Pack(kind ?? 0, maj, min, pat);
    }

    public static bool TryParse(string s, out int packed)
    {
        try
        {
            packed = Parse(s);
            return true;
        }
        catch
        {
            packed = 0;
            return false;
        }
    }
}
