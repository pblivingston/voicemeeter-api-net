// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI.Types;

/// <summary>
///   Snapshot of the state of a connection to VoicemeeterRemote.
/// </summary>
/// <param name="loginStatus"></param>
/// <param name="runningKind"></param>
/// <param name="runningVersion"></param>
public class ConnectionStateEventArgs(LoginResponse loginStatus, Kind runningKind, VmVersion runningVersion) : EventArgs, IEquatable<ConnectionStateEventArgs>
{
    /// <summary>
    ///   Returns the current login status of the <see cref="IRemote"/> instance.
    /// </summary>
    /// <remarks>
    ///   Initially set to <see cref="LoginResponse.LoggedOut"/> until a successful login attempt is made.
    /// </remarks>
    public LoginResponse LoginStatus { get; } = loginStatus;

    /// <summary>
    ///   Returns the currently running Voicemeeter Kind.
    /// </summary>
    /// <remarks>
    ///   Standard, Banana, Potato
    /// </remarks>
    public Kind RunningKind { get; } = runningKind;

    /// <summary>
    ///   Returns the currently running Voicemeeter version.
    /// </summary>
    public VmVersion RunningVersion { get; } = runningVersion;

    /// <summary>
    ///   Simplifies <see cref="LoginStatus"/> checks.
    /// </summary>
    /// <remarks>
    ///   `true` if <see cref="LoginStatus"/> is <see cref="LoginResponse.Ok"/> or <see cref="LoginResponse.VoicemeeterNotRunning"/>, otherwise `false`.
    /// </remarks>
    public bool LoggedIn => LoginStatus < LoginResponse.LoggedOut;

    /// <summary>
    ///   Simplifies <see cref="LoginStatus"/> checks.
    /// </summary>
    /// <remarks>
    ///   `true` if <see cref="LoginStatus"/> is <see cref="LoginResponse.Ok"/>, otherwise `false`.
    /// </remarks>
    public bool Connected => LoginStatus == LoginResponse.Ok;

    public string MemberString => $"LoginStatus: {LoginStatus}; RunningKind: {RunningKind}; RunningVersion: {RunningVersion}";

    public bool Equals(ConnectionStateEventArgs other)
        => LoginStatus == other.LoginStatus
        && RunningKind == other.RunningKind
        && RunningVersion == other.RunningVersion;

    public override bool Equals(object? obj)
        => obj is ConnectionStateEventArgs other
        && Equals(other);

    public override int GetHashCode()
        => LoggedIn ? RunningVersion.GetHashCode() : -1;
}