// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;

namespace VoicemeeterAPI.Types
{
    public readonly struct SemVersion(int packed) : IEquatable<SemVersion>, IComparable<SemVersion>, IComparable
    {
        public int Packed { get; } = packed & 0x00FF_FFFF; // Mask out kind if provided (upper 8 bits)

        // Parts
        private int V1 => (Packed >> 16) & 0xFF;
        private int V2 => (Packed >> 8) & 0xFF;
        private int V3 => Packed & 0xFF;

        // Aliases
        public int Major => V1;
        public int Minor => V2;
        public int Patch => V3;

        #region Constructors

        public SemVersion(int maj, int min, int pat)
            : this(Pack(maj, min, pat))
        {
        }

        public SemVersion(VmVersion vm)
            : this(vm.Semantic.Packed)
        {
        }

        #endregion

        #region Deconstructors

        public void Deconstruct(out int maj, out int min, out int pat)
        {
            maj = Major; min = Minor; pat = Patch;
        }

        #endregion

        #region Validation

        public static bool IsValid(int maj, int min, int pat)
            => maj.InByte()
            && min.InByte()
            && pat.InByte();

        #endregion

        #region Packing

        public static int Pack(int maj, int min, int pat)
            => (maj << 16) | (min << 8) | pat;

        public static bool ValidateAndPack(int maj, int min, int pat, out int packed)
        {
            packed = Pack(maj, min, pat);
            return IsValid(maj, min, pat);
        }

        #endregion

        #region String Representation

        public override string ToString() => $"{V1}.{V2}.{V3}";

        public static bool TryParse(string s, out SemVersion sem)
        {
            sem = default;
            if (string.IsNullOrWhiteSpace(s)) return false;
            var parts = s.Split('.');
            if (parts.Length != 3) return false;
            if (!int.TryParse(parts[0], out var maj)) return false;
            if (!int.TryParse(parts[1], out var min)) return false;
            if (!int.TryParse(parts[2], out var pat)) return false;
            sem = new(maj, min, pat);
            return true;
        }

        #endregion

        #region Conversions

        public static explicit operator SemVersion(VmVersion vm) => new(vm); // VmVersion -> SemVersion

        public static explicit operator int(SemVersion sem) => sem.Packed;         // SemVersion -> int
        public static explicit operator SemVersion(int packed) => new(packed); // int -> SemVersion

        public static explicit operator (int maj, int min, int pat)(SemVersion sem) // SemVersion -> (int, int, int)
            => (sem.Major, sem.Minor, sem.Patch);
        public static explicit operator SemVersion((int maj, int min, int pat) t) // (int, int, int) -> SemVersion
            => new(t.maj, t.min, t.pat);

        #endregion

        #region Equality and Ordering

        public bool Equals(SemVersion other) => Packed == other.Packed;
        public override bool Equals(object? obj) => obj is SemVersion sem && Equals(sem);
        public override int GetHashCode() => Packed;

        public int CompareTo(SemVersion other) => Packed.CompareTo(other.Packed);
        int IComparable.CompareTo(object? obj)
            => obj is SemVersion sem
            ? CompareTo(sem)
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