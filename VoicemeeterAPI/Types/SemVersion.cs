// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI.Types;

using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Utilities;

public readonly struct SemVersion(int packed) : IVersion<SemVersion>
{
    /// <inheritdoc/>
    public int Packed { get; } = IsValid(packed)
        ? packed
        : throw new SemPackedOutOfRangeException(nameof(packed), packed);

    // Parts
    private int V0 => (this.Packed >> 24) & 0xFF;
    private int V1 => (this.Packed >> 16) & 0xFF;
    private int V2 => (this.Packed >> 8) & 0xFF;
    private int V3 => this.Packed & 0xFF;

    /// <inheritdoc/>
    int IVersion.Kind => this.V0;
    /// <inheritdoc/>
    public int Major => this.V1;
    /// <inheritdoc/>
    public int Minor => this.V2;
    /// <inheritdoc/>
    public int Patch => this.V3;

    /// <inheritdoc/>
    Kind IVersion.K => (Kind)this.V0;
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
        maj = this.V1;
        min = this.V2;
        pat = this.V3;
    }

    /// <inheritdoc/>
    void IVersion.Deconstruct<T>(out T kind, out SemVersion sem)
    {
        if (typeof(T) == typeof(int))
        {
            kind = (T)(object)this.V0;
        }
        else if (typeof(T) == typeof(Kind))
        {
            kind = (T)(object)((IVersion)this).K;
        }
        else
        {
            throw new TypeNotSupportedException(typeof(T), nameof(kind), SupportedTypes.KindTypes);
        }

        sem = this;
    }

    /// <inheritdoc/>
    void IVersion.Deconstruct<T>(out T kind, out int maj, out int min, out int pat)
    {
        if (typeof(T) == typeof(int))
        {
            kind = (T)(object)this.V0;
        }
        else if (typeof(T) == typeof(Kind))
        {
            kind = (T)(object)((IVersion)this).K;
        }
        else
        {
            throw new TypeNotSupportedException(typeof(T), nameof(kind), SupportedTypes.KindTypes);
        }

        this.Deconstruct(out maj, out min, out pat);
    }

    #endregion

    #region Validation

    public bool IsValid()
        => IsValid(this.Packed);

    public static bool IsValid(SemVersion sem)
        => sem.IsValid();

    public static bool IsValid(int packed)
        => ((packed >> 24) & 0xFF) == 0
        && packed > 0;

    public static bool IsValid(int maj, int min, int pat)
        => VersionUtils.IsValid(maj, min, pat);

    #endregion

    #region Packing

    public static int RawPack(int maj, int min, int pat)
        => VersionUtils.RawPack(0, maj, min, pat);

    public static bool TryPack(int maj, int min, int pat, out int packed)
    {
        packed = 0;

        if (!IsValid(maj, min, pat))
        {
            return false;
        }

        packed = RawPack(maj, min, pat);
        return true;
    }

    public static int Pack(int maj, int min, int pat)
        => TryPack(maj, min, pat, out var packed)
            ? packed
            : throw new PartsOutOfRangeException(maj, min, pat);

    #endregion

    #region Unpacking

    public static void RawUnpack(int packed, out int maj, out int min, out int pat)
        => VersionUtils.RawUnpack(packed, out _, out maj, out min, out pat);

    public static bool TryUnpack(int packed, out int maj, out int min, out int pat)
    {
        maj = 0;
        min = 0;
        pat = 0;

        if (!IsValid(packed))
        {
            return false;
        }

        RawUnpack(packed, out maj, out min, out pat);
        return true;
    }

    public static void Unpack(int packed, out int maj, out int min, out int pat)
    {
        if (!TryUnpack(packed, out maj, out min, out pat))
        {
            throw new SemPackedOutOfRangeException(nameof(packed), packed);
        }
    }

    #endregion

    #region String Representation

    public override string ToString()
        => $"{this.V1}.{this.V2}.{this.V3}";

    public static bool TryParse(string s, out int maj, out int min, out int pat)
    {
        maj = 0;
        min = 0;
        pat = 0;

        if (!VersionUtils.TryParse(s, out var k, out var m, out var n, out var p))
        {
            return false;
        }

        if (k is not 0)
        {
            return false;
        }

        maj = m;
        min = n;
        pat = p;
        return true;
    }

    public static void Parse(string s, out int maj, out int min, out int pat)
    {
        if (!TryParse(s, out maj, out min, out pat))
        {
            throw new CannotParseAsPartsException(s, nameof(s));
        }
    }

    public static bool TryParse(string s, out int packed)
    {
        packed = 0;

        if (!TryParse(s, out var m, out var n, out var p))
        {
            return false;
        }

        packed = RawPack(m, n, p);
        return true;
    }

    public static void Parse(string s, out int packed)
    {
        if (!TryParse(s, out packed))
        {
            throw new CannotParseAsPartsException(s, nameof(s));
        }
    }

    public static bool TryParse(string s, out SemVersion sem)
    {
        sem = default;

        if (!TryParse(s, out int packed))
        {
            return false;
        }

        sem = new(packed);
        return true;
    }

    public static SemVersion Parse(string s)
        => TryParse(s, out SemVersion sem)
            ? sem
            : throw new CannotParseAsTypeException(s, typeof(SemVersion), nameof(s));

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
