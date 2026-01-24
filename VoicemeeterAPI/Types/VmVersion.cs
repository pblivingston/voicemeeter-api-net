// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;

namespace VoicemeeterAPI.Types
{
    public readonly struct VmVersion(int packed) : IEquatable<VmVersion>, IComparable<VmVersion>, IComparable
    {
        public int Packed { get; } = packed;

        // Parts
        private int V1 => (Packed >> 24) & 0xFF;
        private int V2 => (Packed >> 16) & 0xFF;
        private int V3 => (Packed >> 8) & 0xFF;
        private int V4 => Packed & 0xFF;

        // Derived properties
        public Kind Kind => V1 < (int)Kind.None || V1 > (int)Kind.Potato ? Kind.Unknown : (Kind)V1;
        public SemVersion Semantic => new(Packed & 0x00FF_FFFF); // Extract the lower 24 bits for semantic version

        // Aliases
        public int Major => V2;
        public int Minor => V3;
        public int Patch => V4;

        #region Constructors

        public VmVersion(int k, int maj, int min, int pat)
            : this(Pack(k, maj, min, pat))
        {
        }

        public VmVersion(Kind kind, int maj, int min, int pat)
            : this(Pack(kind, maj, min, pat))
        {
        }

        public VmVersion(int k, SemVersion sem)
            : this(k, sem.Major, sem.Minor, sem.Patch)
        {
        }

        public VmVersion(Kind kind, SemVersion sem)
            : this(kind, sem.Major, sem.Minor, sem.Patch)
        {
        }

        #endregion

        #region Deconstructors

        public void Deconstruct(out int k, out int maj, out int min, out int pat)
        {
            k = V1; maj = V2; min = V3; pat = V4;
        }

        public void Deconstruct(out Kind kind, out int maj, out int min, out int pat)
        {
            kind = Kind; maj = Major; min = Minor; pat = Patch;
        }

        public void Deconstruct(out int k, out SemVersion sem)
        {
            k = V1; sem = Semantic;
        }

        public void Deconstruct(out Kind kind, out SemVersion sem)
        {
            kind = Kind; sem = Semantic;
        }

        #endregion

        #region Validation

        public bool IsValid() => Kind is not Kind.None and not Kind.Unknown;

        public static bool IsValid(VmVersion vm) => vm.IsValid();

        public static bool IsValid(int packed)
        {
            int kind = (packed >> 24) & 0xFF;
            return kind is >= 1 and <= 3;
        }

        public static bool IsValid(int kind, int maj, int min, int pat)
            => kind is >= 1 and <= 3
            && maj.InByte()
            && min.InByte()
            && pat.InByte();

        public static bool IsValid(Kind kind, int maj, int min, int pat)
            => IsValid((int)kind, maj, min, pat);

        public static bool IsValid(int kind, SemVersion? sem)
            => kind is >= 1 and <= 3 && sem is not null;

        public static bool IsValid(Kind kind, SemVersion? sem)
            => IsValid((int)kind, sem);

        #endregion

        #region Packing

        public static int Pack(int k, int maj, int min, int pat)
            => (k << 24) | (maj << 16) | (min << 8) | pat;

        public static int Pack(Kind kind, int maj, int min, int pat)
            => Pack((int)kind, maj, min, pat);

        public static bool TryPack(int k, int maj, int min, int pat, out int packed)
        {
            packed = Pack(k, maj, min, pat);
            return IsValid(k, maj, min, pat);
        }

        public static bool TryPack(Kind kind, int maj, int min, int pat, out int packed)
            => TryPack((int)kind, maj, min, pat, out packed);

        #endregion

        #region String Representation

        public override string ToString() => $"{V1}.{V2}.{V3}.{V4}";

        public static bool TryParse(string s, out VmVersion vm)
        {
            vm = default;
            if (string.IsNullOrWhiteSpace(s)) return false;
            var parts = s.Split('.');
            if (parts.Length != 4) return false;
            if (!int.TryParse(parts[0], out var k)) return false;
            if (!int.TryParse(parts[1], out var maj)) return false;
            if (!int.TryParse(parts[2], out var min)) return false;
            if (!int.TryParse(parts[3], out var pat)) return false;
            vm = new(k, maj, min, pat);
            return true;
        }

        #endregion

        #region Conversions

        public static explicit operator SemVersion(VmVersion vm) => new(vm); // VmVersion -> SemVersion

        public static explicit operator int(VmVersion vm) => vm.Packed;         // VmVersion -> int
        public static explicit operator VmVersion(int packed) => new(packed); // int -> VmVersion

        public static explicit operator (int k, int maj, int min, int pat)(VmVersion vm) // VmVersion -> (int, int, int, int)
            => (vm.V1, vm.V2, vm.V3, vm.V4);
        public static explicit operator VmVersion((int k, int maj, int min, int pat) t) // (int, int, int, int) -> VmVersion
            => new(t.k, t.maj, t.min, t.pat);

        public static explicit operator (Kind kind, int maj, int min, int pat)(VmVersion vm) // VmVersion -> (Kind, int, int, int)
            => (vm.Kind, vm.Major, vm.Minor, vm.Patch);
        public static explicit operator VmVersion((Kind kind, int maj, int min, int pat) t) // (Kind, int, int, int) -> VmVersion
            => new(t.kind, t.maj, t.min, t.pat);

        public static explicit operator (int kind, SemVersion sem)(VmVersion vm) // VmVersion -> (int, SemVersion)
            => (vm.V1, vm.Semantic);
        public static explicit operator VmVersion((int kind, SemVersion sem) t) // (int, SemVersion) -> VmVersion
            => new(t.kind, t.sem.Major, t.sem.Minor, t.sem.Patch);

        public static explicit operator (Kind kind, SemVersion sem)(VmVersion vm) // VmVersion -> (Kind, SemVersion)
            => (vm.Kind, vm.Semantic);
        public static explicit operator VmVersion((Kind kind, SemVersion sem) t) // (Kind, SemVersion) -> VmVersion
            => new(t.kind, t.sem);

        #endregion

        #region Equality and Ordering
        public bool Equals(VmVersion other) => Packed == other.Packed;
        public override bool Equals(object? obj) => obj is VmVersion vm && Equals(vm);
        public override int GetHashCode() => Packed;

        public int CompareTo(VmVersion other) => Packed.CompareTo(other.Packed);
        int IComparable.CompareTo(object? obj)
            => obj is VmVersion vm
            ? CompareTo(vm)
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