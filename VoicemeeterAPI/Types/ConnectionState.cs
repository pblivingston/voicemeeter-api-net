// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI.Types;

/// <summary>
///   Snapshot of the state of a connection to VoicemeeterRemote.
/// </summary>
/// <param name="loginStatus"></param>
/// <param name="runningKind"></param>
/// <param name="runningVersion"></param>
public class ConnectionStateEventArgs(LoginResponse loginStatus, bool macroButtonsIsRunning, Kind runningKind, VmVersion runningVersion) : EventArgs, IEquatable<ConnectionStateEventArgs>
{
    /// <summary>
    ///   The login status of the <see cref="IRemote"/> instance.
    /// </summary>
    public LoginResponse LoginStatus { get; } = loginStatus;

    /// <summary>
    /// 
    /// </summary>
    public bool MacroButtonsIsRunning { get; } = macroButtonsIsRunning;

    /// <summary>
    ///   The running Voicemeeter Kind.
    /// </summary>
    /// <remarks>
    ///   Standard, Banana, Potato
    /// </remarks>
    public Kind RunningKind { get; } = runningKind;

    /// <summary>
    ///   The running Voicemeeter version.
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

    public string MemberString => $"LoginStatus: {LoginStatus}; MacroButtonsIsRunning: {MacroButtonsIsRunning}; RunningKind: {RunningKind}; RunningVersion: {RunningVersion}";

    public bool Equals(ConnectionStateEventArgs other)
        => other is not null
        && LoginStatus == other.LoginStatus
        && MacroButtonsIsRunning == other.MacroButtonsIsRunning
        && RunningKind == other.RunningKind
        && RunningVersion == other.RunningVersion;

    public override bool Equals(object? obj)
        => obj is ConnectionStateEventArgs other
        && Equals(other);

    /// <summary>
    ///   Hash will be positive if logged in, negative if logged out.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    ///   <code>(int)LoginStatus &lt;&lt; 30 | (MacroButtonsIsRunning ? 0 : 1) &lt;&lt; 29 | (int)RunningKind &lt;&lt; 26 | (int)RunningVersion</code>
    /// </remarks>
    public override int GetHashCode()
        => unchecked((int)LoginStatus << 30 | (MacroButtonsIsRunning ? 0 : 1) << 29 | (int)RunningKind << 26 | (int)RunningVersion);

    public static bool operator ==(ConnectionStateEventArgs? a, ConnectionStateEventArgs? b)
        => Equals(a, b);

    public static bool operator !=(ConnectionStateEventArgs? a, ConnectionStateEventArgs? b)
        => !Equals(a, b);
}