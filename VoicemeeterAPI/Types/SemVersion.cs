// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;

namespace VoicemeeterAPI.Types;

public readonly struct SemVersion(int packed) : IVersion<SemVersion>
{
    private const int dV0 = 0;

    /// <inheritdoc/>
    public int Packed { get; } = packed;

    // Parts
    private int V0 => (Packed >> 24) & 0xFF;
    private int V1 => (Packed >> 16) & 0xFF;
    private int V2 => (Packed >> 8) & 0xFF;
    private int V3 => Packed & 0xFF;

    /// <inheritdoc/>
    int IVersion.Kind => V0;
    /// <inheritdoc/>
    public int Major => V1;
    /// <inheritdoc/>
    public int Minor => V2;
    /// <inheritdoc/>
    public int Patch => V3;

    /// <inheritdoc/>
    Kind IVersion.K => V0 == dV0 ? Kind.None : Kind.Unknown;
    /// <inheritdoc/>
    SemVersion IVersion.Semantic => this;

    #region Constructors

    public SemVersion(int maj, int min, int pat)
        : this(RawPack(maj, min, pat))
    {
    }

    public SemVersion(VmVersion vm)
        : this(vm.Semantic.Packed)
    {
    }

    #endregion

    #region Deconstructors

    /// <inheritdoc/>
    public void Deconstruct(out int maj, out int min, out int pat)
    {
        maj = V1;
        min = V2;
        pat = V3;
    }

    /// <inheritdoc/>
    void IVersion.Deconstruct<T>(out T kind, out SemVersion sem)
    {
        KindUtils.ValidateKindType<T>();

        kind = (T)(object)V0;
        sem = this;
    }

    /// <inheritdoc/>
    void IVersion.Deconstruct<T>(out T kind, out int maj, out int min, out int pat)
    {
        KindUtils.ValidateKindType<T>();

        kind = (T)(object)V0;
        Deconstruct(out maj, out min, out pat);
    }

    #endregion

    #region Validation

    public bool IsValid() => V0 == dV0;

    public static bool IsValid(SemVersion sem) => sem.IsValid();

    public static bool IsValid(int packed)
        => ((packed >> 24) & 0xFF) == dV0;

    public static bool IsValid(int maj, int min, int pat)
        => VersionUtils.IsValid(maj, min, pat);

    #endregion

    #region Packing

    public static int RawPack(int maj, int min, int pat)
        => VersionUtils.RawPack(dV0, maj, min, pat);

    public static bool TryPack(int maj, int min, int pat, out int packed)
    {
        packed = 0;
        if (!IsValid(maj, min, pat)) return false;
        packed = RawPack(maj, min, pat);
        return true;
    }

    public static int Pack(int maj, int min, int pat, out int packed)
        => TryPack(maj, min, pat, out packed) ? packed
        : throw new ArgumentException($"Invalid Semantic version part(s): {nameof(maj)} = {maj}, {nameof(min)} = {min}, {nameof(pat)} = {pat}.");

    #endregion

    #region Unpacking

    public static void RawUnpack(int packed, out int maj, out int min, out int pat)
        => VersionUtils.RawUnpack(packed, out _, out maj, out min, out pat);

    public static bool TryUnpack(int packed, out int maj, out int min, out int pat)
    {
        maj = 0; min = 0; pat = 0;
        if (!IsValid(packed)) return false;
        RawUnpack(packed, out maj, out min, out pat);
        return true;
    }

    public static void Unpack(int packed, out int maj, out int min, out int pat)
    {
        if (!TryUnpack(packed, out maj, out min, out pat))
            throw new ArgumentOutOfRangeException(nameof(packed));
    }

    #endregion

    #region String Representation

    public override string ToString() => $"{V1}.{V2}.{V3}";

    public static bool TryParse(string s, out int maj, out int min, out int pat)
    {
        if (!VersionUtils.TryParse(s, out int k, out maj, out min, out pat)) return false;
        if (k is not dV0) return false;
        return true;
    }

    public static void Parse(string s, out int maj, out int min, out int pat)
    {
        if (!TryParse(s, out maj, out min, out pat))
            throw new ArgumentException(nameof(s));
    }

    public static bool TryParse(string s, out SemVersion sem)
    {
        sem = default;
        if (!VersionUtils.TryParse(s, out int packed)) return false;
        if (!IsValid(packed)) return false;
        sem = new(packed);
        return true;
    }

    public static SemVersion Parse(string s)
        => TryParse(s, out SemVersion sem) ? sem
        : throw new ArgumentException(nameof(s));

    #endregion

    #region Conversions

    public static explicit operator SemVersion(VmVersion vm) => new(vm); // VmVersion -> SemVersion

    public static explicit operator int(SemVersion sem) => sem.Packed;     // SemVersion -> int
    public static explicit operator SemVersion(int packed) => new(packed); // int -> SemVersion

    public static explicit operator (int maj, int min, int pat)(SemVersion sem) // SemVersion -> (int, int, int)
        => (sem.Major, sem.Minor, sem.Patch);
    public static explicit operator SemVersion((int maj, int min, int pat) t)   // (int, int, int) -> SemVersion
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
