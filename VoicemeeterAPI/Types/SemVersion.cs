// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;

namespace VoicemeeterAPI.Types
{
    public readonly struct SemVersion(int packed) : IEquatable<SemVersion>, IComparable<SemVersion>, IComparable
    {
        public int Packed { get; } = packed & 0x00FF_FFFF; // Mask out kind if provided (upper 8 bits)

        // Parts
        public int V1 => (Packed >> 16) & 0xFF;
        public int V2 => (Packed >> 8) & 0xFF;
        public int V3 => Packed & 0xFF;

        // Aliases
        public int Major => V1;
        public int Minor => V2;
        public int Patch => V3;

        #region Constructors

        public SemVersion(int major, int minor, int patch)
            : this(Pack(major, minor, patch))
        {
        }

        public SemVersion(VmVersion vmVersion)
            : this(vmVersion.Semantic.Packed)
        {
        }

        #endregion

        #region Deconstructors

        public void Deconstruct(out int major, out int minor, out int patch)
        {
            major = Major; minor = Minor; patch = Patch;
        }

        #endregion

        #region Validation

        public static bool IsValid(int major, int minor, int patch)
            => major.InByte()
            && minor.InByte()
            && patch.InByte();

        #endregion

        #region Packing

        public static int Pack(int major, int minor, int patch)
            => (major << 16) | (minor << 8) | patch;

        public static bool ValidateAndPack(int major, int minor, int patch, out int packed)
        {
            packed = Pack(major, minor, patch);
            return IsValid(major, minor, patch);
        }

        #endregion

        #region String Representation

        public override string ToString() => $"{V1}.{V2}.{V3}";

        public static bool TryParse(string s, out SemVersion version)
        {
            version = default;
            if (string.IsNullOrWhiteSpace(s)) return false;
            var parts = s.Split('.');
            if (parts.Length != 3) return false;
            if (!int.TryParse(parts[0], out var maj)) return false;
            if (!int.TryParse(parts[1], out var min)) return false;
            if (!int.TryParse(parts[2], out var pat)) return false;
            version = new(maj, min, pat);
            return true;
        }

        #endregion

        #region Conversions

        public static explicit operator SemVersion(VmVersion v) => new(v); // VmVersion -> SemVersion

        public static explicit operator int(SemVersion v) => v.Packed;         // SemVersion -> int
        public static explicit operator SemVersion(int packed) => new(packed); // int -> SemVersion

        public static explicit operator (int major, int minor, int patch)(SemVersion v) // SemVersion -> (int, int, int)
            => (v.Major, v.Minor, v.Patch);
        public static explicit operator SemVersion((int major, int minor, int patch) t) // (int, int, int) -> SemVersion
            => new(t.major, t.minor, t.patch);

        #endregion

        #region Equality and Ordering

        public bool Equals(SemVersion other) => Packed == other.Packed;
        public override bool Equals(object? obj) => obj is SemVersion v && Equals(v);
        public override int GetHashCode() => Packed;

        public int CompareTo(SemVersion other) => Packed.CompareTo(other.Packed);
        int IComparable.CompareTo(object? obj)
            => obj is SemVersion v
            ? CompareTo(v)
            : throw new ArgumentException("Object must be SemVersion", nameof(obj));

        public static bool operator ==(SemVersion a, SemVersion b) => a.Packed == b.Packed;
        public static bool operator !=(SemVersion a, SemVersion b) => a.Packed != b.Packed;
        public static bool operator <(SemVersion a, SemVersion b) => a.Packed < b.Packed;
        public static bool operator >(SemVersion a, SemVersion b) => a.Packed > b.Packed;
        public static bool operator <=(SemVersion a, SemVersion b) => a.Packed <= b.Packed;
        public static bool operator >=(SemVersion a, SemVersion b) => a.Packed >= b.Packed;

        #endregion
    }
}