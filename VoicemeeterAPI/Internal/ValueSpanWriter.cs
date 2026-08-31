// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI.Internal;

using System.Buffers;
using System.Runtime.CompilerServices;

internal ref struct ValueSpanWriter
{
    private Span<char> characters;
    private int position;
    private char[]? arrayToReturnToPool;

    private ValueSpanWriter(Span<char> initialBuffer)
    {
        this.characters = initialBuffer;
        this.position = 0;
        this.arrayToReturnToPool = null;
    }

    public static ValueSpanWriter StartArgs(Span<char> initialBuffer)
    {
        var writer = new ValueSpanWriter(initialBuffer);
        writer.Append("{ ");
        return writer;
    }

    public string FinalizeArgs()
    {
        this.Append("}");
        return this.ToString();
    }

    public void Dispose()
    {
        var toReturn = this.arrayToReturnToPool;
        if (toReturn != null)
        {
            this.arrayToReturnToPool = null;
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }

    public override readonly string ToString() => this.characters[..this.position].ToString();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(string? s)
    {
        if (s is null)
        {
            return;
        }

        var l = s.Length;
        if (this.position > this.characters.Length - l)
        {
            this.Grow(l);
        }

        s.AsSpan().CopyTo(this.characters[this.position..]);
        this.position += l;
    }

    #region Add Arg

    // all ConnectionState members: LoginResponse enum, RunResponse enum, App enum, VmVersion struct
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddArg<T>(T value, [CallerArgumentExpression(nameof(value))] string label = "") where T : struct
    {
        label = StripPrefix(label);
        this.Append(label);
        this.Append(": ");
        this.Append(value.ToString());
        this.Append("; ");
    }

    // LogArgs members: Kind enum, App enum, RunResponse enum, LoginResponse enum, VmVersion struct, ConnectionState struct
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddNullableArg<T>(T? value, [CallerArgumentExpression(nameof(value))] string label = "") where T : struct
    {
        if (!value.HasValue)
        {
            return;
        }

        label = StripPrefix(label);
        this.Append(label);
        this.Append(": ");
        this.Append(value.Value.ToString());
        this.Append("; ");
    }

    // LogArgs member "Param"
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddNullableArg(string? value, [CallerArgumentExpression(nameof(value))] string label = "")
    {
        if (value is null)
        {
            return;
        }

        label = StripPrefix(label);
        this.Append(label);
        this.Append(": ");
        this.Append(value);
        this.Append("; ");
    }

    // LogArgs member "Value"
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddNullableArg(object? value, [CallerArgumentExpression(nameof(value))] string label = "")
    {
        if (value is null)
        {
            return;
        }

        label = StripPrefix(label);
        this.Append(label);
        this.Append(": ");
        this.Append(value.ToString());
        this.Append("; ");
    }

    #endregion

    #region Helpers

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Grow(int requiredAdditionalCapacity)
    {
        var newCapacity = Math.Max(this.characters.Length + requiredAdditionalCapacity, this.characters.Length * 2);
        var poolArray = ArrayPool<char>.Shared.Rent(newCapacity);
        this.characters.CopyTo(poolArray);

        if (this.arrayToReturnToPool != null)
        {
            ArrayPool<char>.Shared.Return(this.arrayToReturnToPool);
        }

        this.arrayToReturnToPool = poolArray;
        this.characters = poolArray;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string StripPrefix(string s)
    {
        if (s.StartsWith("this.", StringComparison.InvariantCulture))
        {
            s = s[5..];
        }

        return s;
    }

    #endregion
}
