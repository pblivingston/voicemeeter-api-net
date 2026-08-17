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
        => Result<TResponse, TValue>.Success(response, value);

    public static Result<TResponse, TValue> Failure<TResponse, TValue>(TResponse response)
        where TResponse : struct, Enum
        => Result<TResponse, TValue>.Failure(response);
}

/// <summary>
///   Represents the result of a VoicemeeterAPI operation that does not return data.
/// </summary>
public readonly struct Result<TResponse> where TResponse : struct, Enum
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
}

/// <summary>
///   Represents the result of a VoicemeeterAPI operation that sends or returns a specific value.
/// </summary>
public readonly struct Result<TResponse, TValue> where TResponse : struct, Enum
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
}
