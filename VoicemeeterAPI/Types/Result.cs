// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

public static class Result
{
    public static Result<TResponse> Success<TResponse>(TResponse response)
        where TResponse : struct, Enum
        => Result<TResponse>.Success(response);

    public static Result<TResponse> Failure<TResponse>(TResponse response)
        where TResponse : struct, Enum
        => Result<TResponse>.Failure(response);


    public static Result<TResponse, TValue> Success<TResponse, TValue>(TResponse response, TValue value)
        where TResponse : struct, Enum
        => Result<TResponse, TValue>.Success(response, value);

    public static Result<TResponse, TValue> Failure<TResponse, TValue>(TResponse response, TValue value)
        where TResponse : struct, Enum
        => Result<TResponse, TValue>.Failure(response, value);

    public static Result<TResponse, TValue> Failure<TResponse, TValue>(TResponse response)
        where TResponse : struct, Enum
        => Result<TResponse, TValue>.Failure(response);


    public static Result<TResponse, TParam, TValue> Success<TResponse, TParam, TValue>(TResponse response, TParam param, TValue value)
        where TResponse : struct, Enum
        => Result<TResponse, TParam, TValue>.Success(response, param, value);

    public static Result<TResponse, TParam, TValue> Failure<TResponse, TParam, TValue>(TResponse response, TParam param, TValue value)
        where TResponse : struct, Enum
        => Result<TResponse, TParam, TValue>.Failure(response, param, value);

    public static Result<TResponse, TParam, TValue> Failure<TResponse, TParam, TValue>(TResponse response, TParam param)
        where TResponse : struct, Enum
        => Result<TResponse, TParam, TValue>.Failure(response, param);
}

/// <summary>
///   Represents the result of a VoicemeeterAPI operation that does not send or return specific values.
/// </summary>
public readonly struct Result<TResponse> : IEquatable<Result<TResponse>>
    where TResponse : struct, Enum
{
    /// <summary>
    ///   Represents the response code returned by VoicemeeterRemote or internal wrapper.
    /// </summary>
    public TResponse Response { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => !this.IsSuccess;

    private Result(TResponse response, bool isSuccess)
    {
        this.Response = response;
        this.IsSuccess = isSuccess;
    }

    internal static Result<TResponse> Success(TResponse response)
        => new(response, true);
    internal static Result<TResponse> Failure(TResponse response)
        => new(response, false);

    public static implicit operator Result<TResponse>((TResponse response, bool isSuccess) t)
        => new(t.response, t.isSuccess);

    public bool Equals(Result<TResponse> other)
        => EqualityComparer<TResponse>.Default.Equals(this.Response, other.Response)
        && this.IsSuccess == other.IsSuccess;

    public override bool Equals(object? obj)
        => obj is Result<TResponse> r
        && this.Equals(r);

    public override int GetHashCode()
#if NET5_0_OR_GREATER
        => HashCode.Combine(this.Response, this.IsSuccess);
#else
    {
        unchecked
        {
            var hash = 17;
            hash = (hash * 23) + EqualityComparer<TResponse>.Default.GetHashCode(this.Response);
            hash = (hash * 23) + (this.IsSuccess ? 1 : 0);
            return hash;
        }
    }
#endif

    public static bool operator ==(Result<TResponse> a, Result<TResponse> b) => a.Equals(b);
    public static bool operator !=(Result<TResponse> a, Result<TResponse> b) => !a.Equals(b);
}

/// <summary>
///   Represents the result of a VoicemeeterAPI operation that sends or returns a specific value.
/// </summary>
public readonly struct Result<TResponse, TValue> : IEquatable<Result<TResponse, TValue>>
    where TResponse : struct, Enum
{
    private readonly Result<TResponse> inner;
    private readonly TValue? value;

    /// <inheritdoc cref="Result{TResponse}.Response"/>
    public TResponse Response => this.inner.Response;
    public bool IsSuccess => this.inner.IsSuccess;
    public bool IsFailure => this.inner.IsFailure;

    /// <summary>
    ///   The actual value sent or returned.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public TValue Value => this.value ??
        throw new InvalidOperationException($"Cannot access the value of a failed operation. Response code: {this.Response}.");

    private Result(TResponse response, TValue value, bool isSuccess)
    {
        this.inner = (response, isSuccess);
        this.value = value;
    }

    private Result(TResponse response)
        => this.inner = (response, false);

    internal static Result<TResponse, TValue> Success(TResponse response, TValue value)
        => new(response, value, true);
    internal static Result<TResponse, TValue> Failure(TResponse response, TValue value)
        => new(response, value, false);
    internal static Result<TResponse, TValue> Failure(TResponse response)
        => new(response);

    public static implicit operator Result<TResponse, TValue>((TResponse response, TValue value, bool isSuccess) t)
        => new(t.response, t.value, t.isSuccess);
    public static implicit operator Result<TResponse, TValue>(TResponse response)
        => new(response);

    public static implicit operator TValue(Result<TResponse, TValue> result)
        => result.Value;

    public bool Equals(Result<TResponse, TValue> other)
        => this.inner == other.inner
        && EqualityComparer<TValue?>.Default.Equals(this.value, other.value);

    public override bool Equals(object? obj)
        => obj is Result<TResponse, TValue> r
        && this.Equals(r);

    public override int GetHashCode()
#if NET5_0_OR_GREATER
        => HashCode.Combine(this.inner, this.value ?? default);
#else
    {
        unchecked
        {
            var hash = 17;
            hash = (hash * 23) + this.inner.GetHashCode();
            hash = (hash * 23) + (this.value is null ? 0 : EqualityComparer<TValue>.Default.GetHashCode(this.value));
            return hash;
        }
    }
#endif

    public static bool operator ==(Result<TResponse, TValue> a, Result<TResponse, TValue> b) => a.Equals(b);
    public static bool operator !=(Result<TResponse, TValue> a, Result<TResponse, TValue> b) => !a.Equals(b);
}

/// <summary>
///   Represents the result of a VoicemeeterAPI operation that sends or returns a specific value for a given parameter.
/// </summary>
public readonly struct Result<TResponse, TParam, TValue> : IEquatable<Result<TResponse, TParam, TValue>>
    where TResponse : struct, Enum
{
    private readonly Result<TResponse, TValue> inner;

    /// <inheritdoc cref="Result{TResponse, TValue}.Response"/>
    public TResponse Response => this.inner.Response;
    public bool IsSuccess => this.inner.IsSuccess;
    public bool IsFailure => this.inner.IsFailure;

    /// <inheritdoc cref="Result{TResponse, TValue}.Value"/>
    public TValue Value => this.inner.Value;

    /// <summary>
    ///   The actual param sent.
    /// </summary>
    public TParam Param { get; }

    private Result(TResponse response, TParam param, TValue value, bool isSuccess)
    {
        this.inner = (response, value, isSuccess);
        this.Param = param;
    }

    private Result(TResponse response, TParam param)
    {
        this.inner = response;
        this.Param = param;
    }

    internal static Result<TResponse, TParam, TValue> Success(TResponse response, TParam param, TValue value)
        => new(response, param, value, true);
    internal static Result<TResponse, TParam, TValue> Failure(TResponse response, TParam param, TValue value)
        => new(response, param, value, false);
    internal static Result<TResponse, TParam, TValue> Failure(TResponse response, TParam param)
        => new(response, param);

    public static implicit operator Result<TResponse, TParam, TValue>((TResponse response, TParam param, TValue value, bool isSuccess) t)
        => new(t.response, t.param, t.value, t.isSuccess);
    public static implicit operator Result<TResponse, TParam, TValue>((TResponse response, TParam param) t)
        => new(t.response, t.param);

    public static implicit operator TValue(Result<TResponse, TParam, TValue> result)
        => result.Value;

    public bool Equals(Result<TResponse, TParam, TValue> other)
        => this.inner == other.inner
        && EqualityComparer<TParam>.Default.Equals(this.Param, other.Param);

    public override bool Equals(object? obj)
        => obj is Result<TResponse, TParam, TValue> r
        && this.Equals(r);

    public override int GetHashCode()
#if NET5_0_OR_GREATER
        => HashCode.Combine(this.inner, this.Param);
#else
    {
        unchecked
        {
            var hash = 17;
            hash = (hash * 23) + this.inner.GetHashCode();
            hash = (hash * 23) + EqualityComparer<TParam>.Default.GetHashCode(this.Param);
            return hash;
        }
    }
#endif

    public static bool operator ==(Result<TResponse, TParam, TValue> a, Result<TResponse, TParam, TValue> b) => a.Equals(b);
    public static bool operator !=(Result<TResponse, TParam, TValue> a, Result<TResponse, TParam, TValue> b) => !a.Equals(b);
}
