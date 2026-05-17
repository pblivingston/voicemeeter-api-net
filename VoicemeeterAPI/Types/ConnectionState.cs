// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI.Types;

/// <summary>
///   Snapshot of the state of a connection to VoicemeeterRemote.
/// </summary>
/// <param name="loginStatus"></param>
/// <param name="macroButtonsIsRunning"></param>
/// <param name="runningKind"></param>
/// <param name="runningVersion"></param>
public class ConnectionStateEventArgs(LoginResponse loginStatus, bool macroButtonsIsRunning, Kind runningKind, VmVersion runningVersion) : EventArgs, IEquatable<ConnectionStateEventArgs>
{
    /// <summary>
    ///   The login status of the <see cref="IRemote"/> instance.
    /// </summary>
    /// <remarks>
    ///   Ok, VoicemeeterNotRunning, LoggedOut, Unknown
    /// </remarks>
    public LoginResponse LoginStatus { get; } = loginStatus;

    /// <summary>
    ///
    /// </summary>
    public bool MacroButtonsIsRunning { get; } = macroButtonsIsRunning;

    /// <summary>
    ///   The running Voicemeeter Kind.
    /// </summary>
    /// <remarks>
    ///   None, Standard, Banana, Potato
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
    public bool LoggedIn => this.LoginStatus < LoginResponse.LoggedOut;

    /// <summary>
    ///   Simplifies <see cref="LoginStatus"/> checks.
    /// </summary>
    /// <remarks>
    ///   `true` if <see cref="LoginStatus"/> is <see cref="LoginResponse.Ok"/>, otherwise `false`.
    /// </remarks>
    public bool Connected => this.LoginStatus == LoginResponse.Ok;

    public string MemberString => $"LoginStatus: {this.LoginStatus}; MacroButtonsIsRunning: {this.MacroButtonsIsRunning}; RunningKind: {this.RunningKind}; RunningVersion: {this.RunningVersion}";

    public bool Equals(ConnectionStateEventArgs? other)
        => other is not null
        && this.LoginStatus == other.LoginStatus
        && this.MacroButtonsIsRunning == other.MacroButtonsIsRunning
        && this.RunningKind == other.RunningKind
        && this.RunningVersion == other.RunningVersion;

    public override bool Equals(object? obj)
        => obj is ConnectionStateEventArgs other
        && this.Equals(other);

    /// <summary>
    ///   Hash will be positive if logged in, negative if logged out.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    ///   <code>(int)LoginStatus &lt;&lt; 30 | (MacroButtonsIsRunning ? 0 : 1) &lt;&lt; 29 | (int)RunningKind &lt;&lt; 26 | (int)RunningVersion</code>
    /// </remarks>
    public override int GetHashCode()
        => unchecked(
            ((int)this.LoginStatus << 30) |
            ((this.MacroButtonsIsRunning ? 0 : 1) << 29) |
            ((int)this.RunningKind << 26) |
            (int)this.RunningVersion
        );

    public static bool operator ==(ConnectionStateEventArgs? a, ConnectionStateEventArgs? b)
        => Equals(a, b);

    public static bool operator !=(ConnectionStateEventArgs? a, ConnectionStateEventArgs? b)
        => !Equals(a, b);
}
