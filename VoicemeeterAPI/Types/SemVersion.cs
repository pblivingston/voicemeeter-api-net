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
    private int V1 => (this.Packed >> 24) & 0xFF;
    private int V2 => (this.Packed >> 16) & 0xFF;
    private int V3 => (this.Packed >> 8) & 0xFF;
    private int V4 => this.Packed & 0xFF;

    /// <inheritdoc/>
    int IVersion.Kind => this.V1;
    /// <inheritdoc/>
    public int Major => this.V2;
    /// <inheritdoc/>
    public int Minor => this.V3;
    /// <inheritdoc/>
    public int Patch => this.V4;

    /// <inheritdoc/>
    Kind IVersion.K => (Kind)this.V1;
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
        maj = this.V2;
        min = this.V3;
        pat = this.V4;
    }

    /// <inheritdoc/>
    void IVersion.Deconstruct<T>(out T kind, out SemVersion sem)
    {
        kind = this.GetKind<T>();
        sem = this;
    }

    /// <inheritdoc/>
    void IVersion.Deconstruct<T>(out T kind, out int maj, out int min, out int pat)
    {
        kind = this.GetKind<T>();
        this.Deconstruct(out maj, out min, out pat);
    }

    private T GetKind<T>()
    {
        var t = typeof(T);

        return t switch
        {
            _ when t == typeof(int) => (T)(object)this.V1,
            _ when t == typeof(Kind) => (T)(object)((IVersion)this).K,
            _ => throw SupportedTypes.CreateArgumentException<T>(nameof(T), SupportedTypes.KindTypes)
        };
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
        => $"{this.V2}.{this.V3}.{this.V4}";

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
