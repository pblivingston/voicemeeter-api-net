// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

public readonly struct SemVersion(int packed) : IVersion<SemVersion>
{
    public const int MaxValuePacked = -1;
    public const int MaxValidPacked = 0x00FF_FFFF;
    public const int MinValidPacked = 0x0000_0001;
    public static SemVersion MaxValue { get; } = new(MaxValuePacked);
    public static SemVersion MaxValid { get; } = new(MaxValidPacked);
    public static SemVersion MinValid { get; } = new(MinValidPacked);

    /// <inheritdoc/>
    public int Packed { get; } = packed;

    // Parts
    /// <inheritdoc/>
    int IVersion.Kind => this.Kind;
    /// <inheritdoc cref="IVersion.Kind"/>
    private int Kind => (this.Packed >> 24) & 0xFF;
    /// <inheritdoc/>
    public int Major => (this.Packed >> 16) & 0xFF;
    /// <inheritdoc/>
    public int Minor => (this.Packed >> 8) & 0xFF;
    /// <inheritdoc/>
    public int Patch => this.Packed & 0xFF;

    /// <inheritdoc/>
    Kind IVersion.K => (Kind)this.Kind;
    /// <inheritdoc/>
    SemVersion IVersion.Semantic => this;

    #region Constructors

    public SemVersion(int maj, int min, int pat)
        : this(Pack(maj, min, pat))
    { }

    public SemVersion(VmVersion vm)
        : this(vm.Semantic.Packed)
    { }

    #endregion

    #region Deconstructors

    /// <inheritdoc/>
    public void Deconstruct(out int maj, out int min, out int pat)
    {
        maj = this.Major;
        min = this.Minor;
        pat = this.Patch;
    }

    /// <inheritdoc/>
    void IVersion.Deconstruct(out int kind, out SemVersion sem)
    {
        kind = this.Kind;
        sem = this;
    }

    /// <inheritdoc/>
    void IVersion.Deconstruct(out Kind kind, out SemVersion sem)
    {
        kind = ((IVersion)this).K;
        sem = this;
    }

    /// <inheritdoc/>
    void IVersion.Deconstruct(out int kind, out int maj, out int min, out int pat)
    {
        kind = this.Kind;
        this.Deconstruct(out maj, out min, out pat);
    }

    /// <inheritdoc/>
    void IVersion.Deconstruct(out Kind kind, out int maj, out int min, out int pat)
    {
        kind = ((IVersion)this).K;
        this.Deconstruct(out maj, out min, out pat);
    }

    #endregion

    #region Validation

    public bool IsValid()
        => IsValid(this.Packed);

    public static bool IsValid(SemVersion sem)
        => sem.IsValid();

    public static bool IsValid(int packed)
        => packed is >= MinValidPacked and <= MaxValidPacked;

    #endregion

    #region Packing

    public static int Pack(int maj, int min, int pat)
        => VersionUtils.Pack(0, maj, min, pat);

    public static bool TryPack(int maj, int min, int pat, out int packed)
        => VersionUtils.TryPack(0, maj, min, pat, out packed);

    #endregion

    #region Unpacking

    public static void Unpack(int packed, out int maj, out int min, out int pat)
        => VersionUtils.Unpack(packed, out _, out maj, out min, out pat);

    #endregion

    #region String Representation

    public override string ToString()
        => $"{this.Major}.{this.Minor}.{this.Patch}";

    public static SemVersion Parse(string s)
    {
        VersionUtils.Parse(s, out var k, out var m, out var n, out var p);

        if (k is not null)
        {
            throw new ArgumentException("Version string had more than three parts.", nameof(s));
        }

        return new(m, n, p);
    }

    public static bool TryParse(string s, out SemVersion sem)
    {
        try
        {
            sem = Parse(s);
            return true;
        }
        catch
        {
            sem = default;
            return false;
        }
    }

    #endregion

    #region Conversions

    public static explicit operator SemVersion(VmVersion vm) // VmVersion -> SemVersion
        => new(vm);

    public static explicit operator int(SemVersion sem)    // SemVersion -> int
        => sem.Packed;
    public static explicit operator SemVersion(int packed) // int -> SemVersion
        => new(packed);

    public static explicit operator (int maj, int min, int pat)(SemVersion sem) // SemVersion -> (int, int, int)
        => (sem.Major, sem.Minor, sem.Patch);
    public static explicit operator SemVersion((int maj, int min, int pat) t)   // (int, int, int) -> SemVersion
        => new(t.maj, t.min, t.pat);

    #endregion

    #region Equality and Ordering

    public bool Equals(SemVersion other)
        => this.Packed == other.Packed;

    public override bool Equals(object? obj)
        => obj is SemVersion sem
        && this.Equals(sem);

    public override int GetHashCode()
        => this.Packed;

    public int CompareTo(SemVersion other)
        => this.Packed.CompareTo(other.Packed);

    int IComparable.CompareTo(object? obj)
        => obj is SemVersion sem
            ? this.CompareTo(sem)
            : throw new ArgumentException("Object must be SemVersion", nameof(obj));

    public static bool operator ==(SemVersion a, SemVersion b) => a.Packed == b.Packed;
    public static bool operator !=(SemVersion a, SemVersion b) => a.Packed != b.Packed;
    public static bool operator <(SemVersion a, SemVersion b) => a.Packed < b.Packed;
    public static bool operator >(SemVersion a, SemVersion b) => a.Packed > b.Packed;
    public static bool operator <=(SemVersion a, SemVersion b) => a.Packed <= b.Packed;
    public static bool operator >=(SemVersion a, SemVersion b) => a.Packed >= b.Packed;

    #endregion
}
