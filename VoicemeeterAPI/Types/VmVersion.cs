// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;

namespace VoicemeeterAPI.Types;

public readonly struct VmVersion(int packed) : IVersion<VmVersion>
{
    /// <inheritdoc/>
    public int Packed { get; } = packed;

    // Parts
    private int V1 => (Packed >> 24) & 0xFF;
    private int V2 => (Packed >> 16) & 0xFF;
    private int V3 => (Packed >> 8) & 0xFF;
    private int V4 => Packed & 0xFF;

    public int Kind => V1;
    /// <inheritdoc/>
    public int Major => V2;
    /// <inheritdoc/>
    public int Minor => V3;
    /// <inheritdoc/>
    public int Patch => V4;

    /// <inheritdoc/>
    public Kind K => V1 == 0 || KindUtils.IsValid(V1) ? (Kind)V1 : (Kind)(-1);
    /// <inheritdoc/>
    public SemVersion Semantic => new(Packed & 0x00FF_FFFF);

    #region Constructors

    public VmVersion(int kind, int maj, int min, int pat)
        : this(RawPack(kind, maj, min, pat))
    {
    }

    public VmVersion(Kind k, int maj, int min, int pat)
        : this((int)k, maj, min, pat)
    {
    }

    public VmVersion(int kind, SemVersion sem)
        : this(RawPack(kind, sem))
    {
    }

    public VmVersion(Kind k, SemVersion sem)
        : this((int)k, sem)
    {
    }

    #endregion

    #region Deconstructors

    /// <inheritdoc/>
    public void Deconstruct<T>(out T kind, out int maj, out int min, out int pat)
        where T : unmanaged
    {
        KindUtils.ValidateKindType<T>();

        kind = (T)(object)V1;
        maj = V2;
        min = V3;
        pat = V4;
    }

    /// <inheritdoc/>
    public void Deconstruct<T>(out T kind, out SemVersion sem)
        where T : unmanaged
    {
        KindUtils.ValidateKindType<T>();

        kind = (T)(object)V1;
        sem = Semantic;
    }

    /// <inheritdoc/>
    void IVersion.Deconstruct(out int maj, out int min, out int pat)
        => Deconstruct(out int _, out maj, out min, out pat);

    #endregion

    #region Validation

    public bool IsValid() => V1 is > 0 and <= 3;

    public static bool IsValid(VmVersion vm) => vm.IsValid();

    public static bool IsValid(int packed)
        => ((packed >> 24) & 0xFF) is >= 1 and <= 3;

    public static bool IsValid<T>(T kind, int maj, int min, int pat)
        where T : unmanaged
        => KindUtils.IsKindType<T>()
        && kind is > 0 and <= 3
        && VersionUtils.IsValid(maj, min, pat);

    public static bool IsValid<T>(T kind, SemVersion sem)
        where T : unmanaged
        => KindUtils.IsValid(kind)
        && sem.IsValid();

    #endregion

    #region Packing

    public static int RawPack(int kind, int maj, int min, int pat)
        => VersionUtils.RawPack(kind, maj, min, pat);

    public static bool TryPack<T>(T kind, int maj, int min, int pat, out int packed)
        where T : unmanaged
    {
        packed = 0;
        if (!IsValid(kind, maj, min, pat)) return false;
        packed = RawPack((int)(object)kind, maj, min, pat);
        return true;
    }

    public static int Pack<T>(T kind, int maj, int min, int pat)
        where T : unmanaged
        => TryPack(kind, maj, min, pat, out int packed) ? packed
        : throw new ArgumentException($"Invalid Voicemeeter version part(s): {nameof(kind)} = {kind}, {nameof(maj)} = {maj}, {nameof(min)} = {min}, {nameof(pat)} = {pat}.");

    public static int RawPack(int kind, SemVersion sem)
        => (kind << 24) | sem.Packed;

    public static bool TryPack<T>(T kind, SemVersion sem, out int packed)
        where T : unmanaged
    {
        packed = 0;
        if (!IsValid(kind, sem)) return false;
        packed = RawPack((int)(object)kind, sem);
        return true;
    }

    public static int Pack<T>(T kind, SemVersion sem) where T : unmanaged
        => TryPack(kind, sem, out int packed) ? packed
        : throw new ArgumentException($"Invalid Voicemeeter version part(s): {nameof(kind)} = {kind}, {nameof(sem)} = {sem}.");

    #endregion

    #region Unpacking

    public static void RawUnpack(int packed, out int kind, out int maj, out int min, out int pat)
        => VersionUtils.RawUnpack(packed, out kind, out maj, out min, out pat);

    public static bool TryUnpack<T>(int packed, out T kind, out int maj, out int min, out int pat)
        where T : unmanaged
    {
        kind = default; maj = 0; min = 0; pat = 0;
        if (!IsValid(packed)) return false;
        RawUnpack(packed, out int k, out maj, out min, out pat);
        kind = (T)(object)k;
        return true;
    }

    public static void Unpack<T>(int packed, out T kind, out int maj, out int min, out int pat)
        where T : unmanaged
    {
        if (!TryUnpack(packed, out kind, out maj, out min, out pat))
            throw new ArgumentOutOfRangeException(nameof(packed));
    }

    public static void RawUnpack(int packed, out int kind, out SemVersion sem)
    {
        kind = (packed >> 24) & 0xFF;
        sem = new(packed & 0x00FF_FFFF);
    }

    public static bool TryUnpack<T>(int packed, out T kind, out SemVersion sem)
        where T : unmanaged
    {
        kind = default; sem = default;
        if (!IsValid(packed)) return false;
        RawUnpack(packed, out int k, out sem);
        kind = (T)(object)k;
        return true;
    }

    public static void Unpack<T>(int packed, out T kind, out SemVersion sem)
        where T : unmanaged
    {
        if (!TryUnpack(packed, out kind, out sem))
            throw new ArgumentOutOfRangeException(nameof(packed));
    }

    #endregion

    #region String Representation

    public override string ToString() => $"{V1}.{V2}.{V3}.{V4}";

    public static bool TryParse<T>(string s, out T kind, out int maj, out int min, out int pat)
        where T : unmanaged
    {
        kind = default;
        if (!VersionUtils.TryParse(s, out int k, out maj, out min, out pat)) return false;
        kind = (T)(object)k;
        return true;
    }

    public static void Parse<T>(string s, out T kind, out int maj, out int min, out int pat)
        where T : unmanaged
    {
        if (!TryParse(s, out kind, out maj, out min, out pat))
            throw new ArgumentException(nameof(s));
    }

    public static bool TryParse<T>(string s, out T kind, out SemVersion sem)
        where T : unmanaged
    {
        kind = default; sem = default;
        if (!TryParse(s, out int k, out int maj, out int min, out int pat)) return false;
        kind = (T)(object)k;
        sem = new(maj, min, pat);
        return true;
    }

    public static void Parse<T>(string s, out T kind, out SemVersion sem)
        where T : unmanaged
    {
        if (!TryParse(s, out kind, out sem))
            throw new ArgumentException(nameof(s));
    }

    public static bool TryParse(string s, out VmVersion vm)
    {
        vm = default;
        if (!VersionUtils.TryParse(s, out int packed)) return false;
        if (!IsValid(packed)) return false;
        vm = new(packed);
        return true;
    }

    public static VmVersion Parse(string s)
        => TryParse(s, out VmVersion vm) ? vm
        : throw new ArgumentException(nameof(s));

    #endregion

    #region Conversions

    public static explicit operator int(VmVersion vm) => vm.Packed;       // VmVersion -> int
    public static explicit operator VmVersion(int packed) => new(packed); // int -> VmVersion

    public static explicit operator (int kind, int maj, int min, int pat)(VmVersion vm) // VmVersion -> (int, int, int, int)
        => (vm.Kind, vm.Major, vm.Minor, vm.Patch);
    public static explicit operator VmVersion((int kind, int maj, int min, int pat) t)  // (int, int, int, int) -> VmVersion
        => new(t.kind, t.maj, t.min, t.pat);

    public static explicit operator (Kind kind, int maj, int min, int pat)(VmVersion vm) // VmVersion -> (Kind, int, int, int)
        => (vm.K, vm.Major, vm.Minor, vm.Patch);
    public static explicit operator VmVersion((Kind kind, int maj, int min, int pat) t)  // (Kind, int, int, int) -> VmVersion
        => new(t.kind, t.maj, t.min, t.pat);

    public static explicit operator (int kind, SemVersion sem)(VmVersion vm) // VmVersion -> (int, SemVersion)
        => (vm.Kind, vm.Semantic);
    public static explicit operator VmVersion((int kind, SemVersion sem) t)  // (int, SemVersion) -> VmVersion
        => new(t.kind, t.sem.Major, t.sem.Minor, t.sem.Patch);

    public static explicit operator (Kind kind, SemVersion sem)(VmVersion vm) // VmVersion -> (Kind, SemVersion)
        => (vm.K, vm.Semantic);
    public static explicit operator VmVersion((Kind kind, SemVersion sem) t)  // (Kind, SemVersion) -> VmVersion
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
