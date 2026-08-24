// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

public readonly struct VmVersion(int packed) : IVersion<VmVersion>
{
    public const int MaxValuePacked = -1;
    public const int MaxValidPacked = 0x03FF_FFFF;
    public const int MinValidPacked = 0x0100_0001;
    public static VmVersion MaxValue { get; } = new(MaxValuePacked);
    public static VmVersion MaxValid { get; } = new(MaxValidPacked);
    public static VmVersion MinValid { get; } = new(MinValidPacked);

    /// <inheritdoc/>
    public int Packed { get; } = packed;

    // Parts
    /// <inheritdoc/>
    public int Kind => (this.Packed >> 24) & 0xFF;
    /// <inheritdoc/>
    public int Major => (this.Packed >> 16) & 0xFF;
    /// <inheritdoc/>
    public int Minor => (this.Packed >> 8) & 0xFF;
    /// <inheritdoc/>
    public int Patch => this.Packed & 0xFF;

    /// <inheritdoc/>
    public Kind K => (Kind)this.Kind;
    /// <inheritdoc/>
    public SemVersion Semantic => new(this.Packed & SemVersion.MaxValidPacked);

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
        kind = this.Kind;
        maj = this.Major;
        min = this.Minor;
        pat = this.Patch;
    }

    /// <inheritdoc cref="IVersion.Deconstruct{T}(out T, out int, out int, out int)"/>
    public void Deconstruct(out Kind k, out int maj, out int min, out int pat)
    {
        k = this.K;
        maj = this.Major;
        min = this.Minor;
        pat = this.Patch;
    }

    /// <inheritdoc cref="IVersion.Deconstruct{T}(out T, out SemVersion)"/>
    public void Deconstruct(out int kind, out SemVersion sem)
    {
        kind = this.Kind;
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
        maj = this.Major;
        min = this.Minor;
        pat = this.Patch;
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
            _ when t == typeof(int) => (T)(object)this.Kind,
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
        => packed is >= MinValidPacked and <= MaxValidPacked;

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
        sem = new(packed & SemVersion.MaxValidPacked);
    }

    public static void Unpack(int packed, out Kind kind, out SemVersion sem)
    {
        Unpack(packed, out int k, out sem);
        kind = (Kind)k;
    }

    #endregion

    #region String Representation

    public override string ToString()
        => $"{this.Kind}.{this.Major}.{this.Minor}.{this.Patch}";

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
