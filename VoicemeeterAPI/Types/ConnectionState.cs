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
public readonly struct ConnectionState(LoginResponse loginStatus, bool macroButtonsIsRunning, Kind runningKind, VmVersion runningVersion)
    : IEquatable<ConnectionState>
{
    /// <summary>
    ///   HashCode will be positive if logged in, negative if logged out.
    /// </summary>
    /// <remarks>
    ///   <code>(int)LoginStatus &lt;&lt; 30 | (MacroButtonsIsRunning ? 0 : 1) &lt;&lt; 29 | (int)RunningKind &lt;&lt; 26 | (int)RunningVersion</code>
    /// </remarks>
    public int HashCode { get; } = unchecked(
        ((int)loginStatus << 30) |
        ((macroButtonsIsRunning ? 0 : 1) << 29) |
        ((int)runningKind << 26) |
        ((int)runningVersion)
    );

    /// <summary>
    ///   The login status of the <see cref="IRemote"/> instance.
    /// </summary>
    /// <remarks>
    ///   Ok, VoicemeeterNotRunning, LoggedOut, Unknown
    /// </remarks>
    public LoginResponse LoginStatus => (LoginResponse)((this.HashCode >> 30) & 0x3);

    /// <summary>
    ///
    /// </summary>
    public bool MacroButtonsIsRunning => ((this.HashCode >> 29) & 0x1) == 0;

    /// <summary>
    ///   The running Voicemeeter Kind.
    /// </summary>
    /// <remarks>
    ///   None, Standard, Banana, Potato
    /// </remarks>
    public Kind RunningKind => (Kind)((this.HashCode >> 26) & 0x3);

    private int VmPacked => this.HashCode & 0x3FFFFFF;
    /// <summary>
    ///   The running Voicemeeter version.
    /// </summary>
    public VmVersion RunningVersion => this.VmPacked == 0 ? default : (VmVersion)this.VmPacked;

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

    public override string ToString() => $"""
        LoginStatus: {this.LoginStatus}
        MacroButtonsIsRunning: {this.MacroButtonsIsRunning}
        RunningKind: {this.RunningKind}
        RunningVersion: {this.RunningVersion}
        """;

    public void Deconstruct(out LoginResponse loginStatus, out bool macroButtonsIsRunning, out Kind runningKind, out VmVersion runningVersion)
    {
        loginStatus = this.LoginStatus;
        macroButtonsIsRunning = this.MacroButtonsIsRunning;
        runningKind = this.RunningKind;
        runningVersion = this.RunningVersion;
    }

    public static explicit operator ConnectionState((LoginResponse login, bool macro, Kind kind, VmVersion version) t)
        => new(t.login, t.macro, t.kind, t.version);
    public static explicit operator (LoginResponse login, bool macro, Kind kind, VmVersion version)(ConnectionState state)
        => (state.LoginStatus, state.MacroButtonsIsRunning, state.RunningKind, state.RunningVersion);

    public bool Equals(ConnectionState other)
        => this.HashCode == other.HashCode;
    public override bool Equals(object? obj)
        => obj is ConnectionState other
        && this.Equals(other);
    public override int GetHashCode()
        => this.HashCode;

    public static bool operator ==(ConnectionState a, ConnectionState b) => a.HashCode == b.HashCode;
    public static bool operator !=(ConnectionState a, ConnectionState b) => a.HashCode != b.HashCode;
}

public class ConnectionStateEventArgs(ConnectionState previousState, ConnectionState currentState)
    : EventArgs
{
    public ConnectionState PreviousState { get; } = previousState;
    public ConnectionState CurrentState { get; } = currentState;
    public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;
}