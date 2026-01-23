// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;
using VoicemeeterAPI.Utils;

namespace VoicemeeterAPI.Types
{
    public readonly struct VmVersion(int packed) : IEquatable<VmVersion>, IComparable<VmVersion>, IComparable
    {
        public int Packed { get; } = packed;

        // Parts
        public int V1 => (Packed >> 24) & 0xFF;
        public int V2 => (Packed >> 16) & 0xFF;
        public int V3 => (Packed >> 8) & 0xFF;
        public int V4 => Packed & 0xFF;

        // Aliases
        public int Major => V2;
        public int Minor => V3;
        public int Patch => V4;

        // Derived properties
        public Kind Kind => V1 < (int)Kind.None || V1 > (int)Kind.Potato ? Kind.Unknown : (Kind)V1;
        public SemVersion Semantic => new(Packed & 0x00FF_FFFF); // Extract the lower 24 bits for semantic version

        #region Constructors

        public VmVersion(int v1, int v2, int v3, int v4)
            : this(Pack(v1, v2, v3, v4))
        {
        }

        public VmVersion(Kind kind, int major, int minor, int patch)
            : this(Pack(kind, major, minor, patch))
        {
        }

        public VmVersion(int v1, SemVersion semVersion)
            : this(v1, semVersion.V1, semVersion.V2, semVersion.V3)
        {
        }

        public VmVersion(Kind kind, SemVersion semVersion)
            : this(kind, semVersion.Major, semVersion.Minor, semVersion.Patch)
        {
        }

        #endregion

        #region Deconstructors

        public void Deconstruct(out int v1, out int v2, out int v3, out int v4)
        {
            v1 = V1; v2 = V2; v3 = V3; v4 = V4;
        }

        public void Deconstruct(out Kind kind, out int major, out int minor, out int patch)
        {
            kind = Kind; major = Major; minor = Minor; patch = Patch;
        }

        public void Deconstruct(out int v1, out SemVersion semVersion)
        {
            v1 = V1; semVersion = Semantic;
        }

        public void Deconstruct(out Kind kind, out SemVersion semVersion)
        {
            kind = Kind; semVersion = Semantic;
        }

        #endregion

        #region Validation

        public bool IsValid() => Kind is not Kind.None and not Kind.Unknown;

        public static bool IsValid(VmVersion version) => version.IsValid();

        public static bool IsValid(int packed)
        {
            int kind = (packed >> 24) & 0xFF;
            return kind is >= 1 and <= 3;
        }

        public static bool IsValid(int kind, int major, int minor, int patch)
            => kind is >= 1 and <= 3
            && GeneralUtils.InByte(major)
            && GeneralUtils.InByte(minor)
            && GeneralUtils.InByte(patch);

        public static bool IsValid(Kind kind, int major, int minor, int patch)
            => IsValid((int)kind, major, minor, patch);

        public static bool IsValid(int kind, SemVersion? semVersion)
            => kind is >= 1 and <= 3 && semVersion is not null;

        public static bool IsValid(Kind kind, SemVersion? semVersion)
            => IsValid((int)kind, semVersion);

        #endregion

        #region Packing

        public static int Pack(int v1, int v2, int v3, int v4)
            => (v1 << 24) | (v2 << 16) | (v3 << 8) | v4;

        public static int Pack(Kind kind, int major, int minor, int patch)
            => Pack((int)kind, major, minor, patch);

        public static bool TryPack(int v1, int v2, int v3, int v4, out int packed)
        {
            packed = Pack(v1, v2, v3, v4);
            return IsValid(v1, v2, v3, v4);
        }

        public static bool TryPack(Kind kind, int major, int minor, int patch, out int packed)
            => TryPack((int)kind, major, minor, patch, out packed);

        #endregion

        #region String Representation

        public override string ToString() => $"{V1}.{V2}.{V3}.{V4}";

        public static bool TryParse(string s, out VmVersion version)
        {
            version = default;
            if (string.IsNullOrWhiteSpace(s)) return false;
            var parts = s.Split('.');
            if (parts.Length != 4) return false;
            if (!int.TryParse(parts[0], out var k)) return false;
            if (!int.TryParse(parts[1], out var maj)) return false;
            if (!int.TryParse(parts[2], out var min)) return false;
            if (!int.TryParse(parts[3], out var pat)) return false;
            version = new(k, maj, min, pat);
            return true;
        }

        #endregion

        #region Conversions

        public static explicit operator SemVersion(VmVersion v) => new(v); // VmVersion -> SemVersion

        public static explicit operator int(VmVersion v) => v.Packed;         // VmVersion -> int
        public static explicit operator VmVersion(int packed) => new(packed); // int -> VmVersion

        public static explicit operator (int v1, int v2, int v3, int v4)(VmVersion v) // VmVersion -> (int, int, int, int)
            => (v.V1, v.V2, v.V3, v.V4);
        public static explicit operator VmVersion((int v1, int v2, int v3, int v4) t) // (int, int, int, int) -> VmVersion
            => new(t.v1, t.v2, t.v3, t.v4);

        public static explicit operator (Kind kind, int major, int minor, int patch)(VmVersion v) // VmVersion -> (Kind, int, int, int)
            => (v.Kind, v.Major, v.Minor, v.Patch);
        public static explicit operator VmVersion((Kind kind, int major, int minor, int patch) t) // (Kind, int, int, int) -> VmVersion
            => new(t.kind, t.major, t.minor, t.patch);

        public static explicit operator (int kind, SemVersion semVersion)(VmVersion v) // VmVersion -> (int, SemVersion)
            => (v.V1, v.Semantic);
        public static explicit operator VmVersion((int kind, SemVersion semVersion) t) // (int, SemVersion) -> VmVersion
            => new(t.kind, t.semVersion.V1, t.semVersion.V2, t.semVersion.V3);

        public static explicit operator (Kind kind, SemVersion semVersion)(VmVersion v) // VmVersion -> (Kind, SemVersion)
            => (v.Kind, v.Semantic);
        public static explicit operator VmVersion((Kind kind, SemVersion semVersion) t) // (Kind, SemVersion) -> VmVersion
            => new(t.kind, t.semVersion);

        #endregion

        #region Equality and Ordering
        public bool Equals(VmVersion other) => Packed == other.Packed;
        public override bool Equals(object? obj) => obj is VmVersion v && Equals(v);
        public override int GetHashCode() => Packed;

        public int CompareTo(VmVersion other) => Packed.CompareTo(other.Packed);
        int IComparable.CompareTo(object? obj)
            => obj is VmVersion v
            ? CompareTo(v)
            : throw new ArgumentException("Object must be VmVersion", nameof(obj));

        public static bool operator ==(VmVersion a, VmVersion b) => a.Packed == b.Packed;
        public static bool operator !=(VmVersion a, VmVersion b) => a.Packed != b.Packed;
        public static bool operator <(VmVersion a, VmVersion b) => a.Packed < b.Packed;
        public static bool operator >(VmVersion a, VmVersion b) => a.Packed > b.Packed;
        public static bool operator <=(VmVersion a, VmVersion b) => a.Packed <= b.Packed;
        public static bool operator >=(VmVersion a, VmVersion b) => a.Packed >= b.Packed;

        #endregion
    }
}