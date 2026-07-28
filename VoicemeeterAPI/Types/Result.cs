// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using System.Runtime.CompilerServices;

public static class Result
{
    /// <summary>
    ///   Creates a <see cref="Result{TResponse}"/> where 'Response' is 'Ok' (0).
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    public static Result<TResponse> Success<TResponse>()
        where TResponse : struct, Enum
        => Result<TResponse>.Success();

    /// <summary>
    ///   Creates a <see cref="Result{TResponse}"/> using the given response.
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="response"></param>
    /// <returns></returns>
    /// <remarks>
    ///   'IsFailure' if 'Response' is not 'Ok' (0).
    /// </remarks>
    public static Result<TResponse> Failure<TResponse>(TResponse response)
        where TResponse : struct, Enum
        => Result<TResponse>.Failure(response);


    /// <summary>
    ///   Creates a <see cref="Result{TResponse, TValue}"/> using the given response and value.
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="response"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <remarks>
    ///   'IsSuccess' if 'Response' is 'Ok' (0) or greater.
    /// </remarks>
    public static Result<TResponse, TValue> Success<TResponse, TValue>(TResponse response, TValue value)
        where TResponse : struct, Enum
        => Result<TResponse, TValue>.Success(response, value);

    /// <summary>
    ///   Creates a <see cref="Result{TResponse, TValue}"/> using the given response.
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="response"></param>
    /// <returns></returns>
    /// <remarks>
    ///   'IsFailure' if 'Response' is less than 'Ok' (0).
    /// </remarks>
    public static Result<TResponse, TValue> Failure<TResponse, TValue>(TResponse response)
        where TResponse : struct, Enum
        => Result<TResponse, TValue>.Failure(response);
}

/// <summary>
///   Represents the result of a VoicemeeterRemote operation that does not return data.
/// </summary>
public readonly struct Result<TResponse> where TResponse : struct, Enum
{
    /// <summary>
    ///   Represents the actual response code returned by VoicemeeterRemote.
    /// </summary>
    public TResponse Response { get; }
    /// <summary>
    ///   'true' if Response is 'Ok' (0).
    /// </summary>
    public bool IsSuccess => EqualityComparer<TResponse>.Default.Equals(this.Response, default);
    public bool IsFailure => !this.IsSuccess;

    private Result(TResponse response)
        => this.Response = response;

    internal static Result<TResponse> Success()
        => new(default);
    internal static Result<TResponse> Failure(TResponse response)
        => new(response);

    public static implicit operator Result<TResponse>(TResponse response)
        => new(response);
}

/// <summary>
///   Represents the result of a VoicemeeterRemote operation that returns a specific value.
/// </summary>
public readonly struct Result<TResponse, TValue> where TResponse : struct, Enum
{
    private readonly TValue? value;

    /// <summary>
    ///   Represents the actual response code returned by VoicemeeterRemote.<br/>
    ///   'Ok' where the response code reflects the value or additional information.
    /// </summary>
    public TResponse Response { get; }
    /// <summary>
    ///   'true' if Response is 'Ok' (0) or greater.
    /// </summary>
    public bool IsSuccess => AsInt(this.Response) >= 0;
    /// <summary>
    ///   'true' if Response is less than 'Ok' (0).
    /// </summary>
    public bool IsFailure => !this.IsSuccess;

    /// <summary>
    ///   The actual value returned or value reflected by the actual response code.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public TValue Value => this.IsSuccess
        ? this.value!
        : throw new InvalidOperationException($"Cannot access the value of a failed operation. Response code: {this.Response}.");

    private Result(TResponse response, TValue value)
    {
        this.value = value;
        this.Response = response;
    }

    private Result(TResponse response)
        => this.Response = response;

    internal static Result<TResponse, TValue> Success(TResponse response, TValue value)
        => new(response, value);
    internal static Result<TResponse, TValue> Failure(TResponse response)
        => new(response);

    public static implicit operator Result<TResponse, TValue>(TResponse response)
        => new(response);
    public static implicit operator Result<TResponse, TValue>((TResponse response, TValue value) t)
        => new(t.response, t.value);

    private static int AsInt(TResponse response)
        => Unsafe.As<TResponse, int>(ref response);
}
