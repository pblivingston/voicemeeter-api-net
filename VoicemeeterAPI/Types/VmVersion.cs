// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

public readonly struct VmVersion(int packed) : IVersion<VmVersion>
{
    public const int MaxPacked = 0x03FF_FFFF;
    public static VmVersion MaxValue { get; } = new(MaxPacked);

    /// <inheritdoc/>
    public int Packed { get; } = packed;

    // Parts
    private int V1 => (this.Packed >> 24) & 0xFF;
    private int V2 => (this.Packed >> 16) & 0xFF;
    private int V3 => (this.Packed >> 8) & 0xFF;
    private int V4 => this.Packed & 0xFF;

    /// <inheritdoc/>
    public int Kind => this.V1;
    /// <inheritdoc/>
    public int Major => this.V2;
    /// <inheritdoc/>
    public int Minor => this.V3;
    /// <inheritdoc/>
    public int Patch => this.V4;

    /// <inheritdoc/>
    public Kind K => (Kind)this.V1;
    /// <inheritdoc/>
    public SemVersion Semantic => new(this.Packed & SemVersion.MaxPacked);

    #region Constructors

    public VmVersion(int kind, int maj, int min, int pat)
        : this(Pack(kind, maj, min, pat))
    { }

    public VmVersion(Kind k, int maj, int min, int pat)
        : this(Pack(k, maj, min, pat))
    { }

    public VmVersion(int kind, SemVersion sem)
        : this(Pack(kind, sem))
    { }

    public VmVersion(Kind k, SemVersion sem)
        : this(Pack(k, sem))
    { }

    #endregion

    #region Deconstructors

    /// <inheritdoc cref="IVersion.Deconstruct{T}(out T, out int, out int, out int)"/>
    public void Deconstruct(out int kind, out int maj, out int min, out int pat)
    {
        kind = this.V1;
        maj = this.V2;
        min = this.V3;
        pat = this.V4;
    }

    /// <inheritdoc cref="IVersion.Deconstruct{T}(out T, out int, out int, out int)"/>
    public void Deconstruct(out Kind k, out int maj, out int min, out int pat)
    {
        k = this.K;
        maj = this.V2;
        min = this.V3;
        pat = this.V4;
    }

    /// <inheritdoc cref="IVersion.Deconstruct{T}(out T, out SemVersion)"/>
    public void Deconstruct(out int kind, out SemVersion sem)
    {
        kind = this.V1;
        sem = this.Semantic;
    }

    /// <inheritdoc cref="IVersion.Deconstruct{T}(out T, out SemVersion)"/>
    public void Deconstruct(out Kind k, out SemVersion sem)
    {
        k = this.K;
        sem = this.Semantic;
    }

    /// <inheritdoc/>
    void IVersion.Deconstruct(out int maj, out int min, out int pat)
        => this.Deconstruct(out int _, out maj, out min, out pat);

    /// <inheritdoc/>
    void IVersion.Deconstruct<T>(out T kind, out int maj, out int min, out int pat)
    {
        kind = this.GetKind<T>();
        maj = this.V2;
        min = this.V3;
        pat = this.V4;
    }

    /// <inheritdoc/>
    void IVersion.Deconstruct<T>(out T kind, out SemVersion sem)
    {
        kind = this.GetKind<T>();
        sem = this.Semantic;
    }

    private T GetKind<T>()
    {
        var t = typeof(T);

        return t switch
        {
            _ when t == typeof(int) => (T)(object)this.V1,
            _ when t == typeof(Kind) => (T)(object)this.K,
            _ => throw SupportedTypes.CreateArgumentException<T>(nameof(T), SupportedTypes.KindTypes)
        };
    }

    #endregion

    #region Validation

    public bool IsValid()
        => IsValid(this.Packed);

    public static bool IsValid(VmVersion vm)
        => vm.IsValid();

    public static bool IsValid(int packed)
        => KindUtils.IsValid((packed >> 24) & 0xFF)
        && SemVersion.IsValid(packed & SemVersion.MaxPacked);

    #endregion

    #region Packing

    public static int Pack(int kind, int maj, int min, int pat)
        => VersionUtils.Pack(kind, maj, min, pat);

    public static bool TryPack(int kind, int maj, int min, int pat, out int packed)
        => VersionUtils.TryPack(kind, maj, min, pat, out packed);

    public static int Pack(Kind kind, int maj, int min, int pat)
        => Pack((int)kind, maj, min, pat);

    public static bool TryPack(Kind kind, int maj, int min, int pat, out int packed)
        => TryPack((int)kind, maj, min, pat, out packed);

    public static int Pack(int kind, SemVersion sem)
    {
        Utilities.ThrowIfNotInByte(kind);

        if (!(sem == default || sem.IsValid()))
        {
            throw new ArgumentOutOfRangeException(nameof(sem), sem, "Semantic version does not fit in three bytes.");
        }

        return (kind << 24) | sem.Packed;
    }

    public static bool TryPack(int kind, SemVersion sem, out int packed)
    {
        try
        {
            packed = Pack(kind, sem);
            return true;
        }
        catch
        {
            packed = 0;
            return false;
        }
    }

    public static int Pack(Kind kind, SemVersion sem)
        => Pack((int)kind, sem);

    public static bool TryPack(Kind kind, SemVersion sem, out int packed)
        => TryPack((int)kind, sem, out packed);

    #endregion

    #region Unpacking

    public static void Unpack(int packed, out int kind, out int maj, out int min, out int pat)
        => VersionUtils.Unpack(packed, out kind, out maj, out min, out pat);

    public static void Unpack(int packed, out Kind kind, out int maj, out int min, out int pat)
    {
        Unpack(packed, out int k, out maj, out min, out pat);
        kind = (Kind)k;
    }

    public static void Unpack(int packed, out int kind, out SemVersion sem)
    {
        kind = (packed >> 24) & 0xFF;
        sem = new(packed & SemVersion.MaxPacked);
    }

    public static void Unpack(int packed, out Kind kind, out SemVersion sem)
    {
        Unpack(packed, out int k, out sem);
        kind = (Kind)k;
    }

    #endregion

    #region String Representation

    public override string ToString()
        => $"{this.V1}.{this.V2}.{this.V3}.{this.V4}";

    public static VmVersion Parse(string s)
    {
        VersionUtils.Parse(s, out var k, out var m, out var n, out var p);

        if (k is null)
        {
            throw new ArgumentException("Version string had less than four parts.", nameof(s));
        }

        return new((int)k, m, n, p);
    }

    public static bool TryParse(string s, out VmVersion vm)
    {
        try
        {
            vm = Parse(s);
            return true;
        }
        catch
        {
            vm = default;
            return false;
        }
    }

    #endregion

    #region Conversions

    public static explicit operator int(VmVersion vm)     // VmVersion -> int
        => vm.Packed;
    public static explicit operator VmVersion(int packed) // int -> VmVersion
        => new(packed);

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
        => new(t.kind, t.sem);

    public static explicit operator (Kind kind, SemVersion sem)(VmVersion vm) // VmVersion -> (Kind, SemVersion)
        => (vm.K, vm.Semantic);
    public static explicit operator VmVersion((Kind kind, SemVersion sem) t)  // (Kind, SemVersion) -> VmVersion
        => new(t.kind, t.sem);

    #endregion

    #region Equality and Ordering
    public bool Equals(VmVersion other)
        => this.Packed == other.Packed;
    public override bool Equals(object? obj)
        => obj is VmVersion vm
        && this.Equals(vm);
    public override int GetHashCode()
        => this.Packed;

    public int CompareTo(VmVersion other)
        => this.Packed.CompareTo(other.Packed);
    int IComparable.CompareTo(object? obj)
        => obj is VmVersion vm
            ? this.CompareTo(vm)
            : throw new ArgumentException("Object must be VmVersion", nameof(obj));

    public static bool operator ==(VmVersion a, VmVersion b) => a.Packed == b.Packed;
    public static bool operator !=(VmVersion a, VmVersion b) => a.Packed != b.Packed;
    public static bool operator <(VmVersion a, VmVersion b) => a.Packed < b.Packed;
    public static bool operator >(VmVersion a, VmVersion b) => a.Packed > b.Packed;
    public static bool operator <=(VmVersion a, VmVersion b) => a.Packed <= b.Packed;
    public static bool operator >=(VmVersion a, VmVersion b) => a.Packed >= b.Packed;

    #endregion
}
