// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI.Types;

public class ConnectionStateEventArgs(ConnectionState previousState, ConnectionState currentState)
    : EventArgs
{
    public ConnectionState PreviousState { get; } = previousState;
    public ConnectionState CurrentState { get; } = currentState;
    public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;
}

/// <summary>
///   Snapshot of the state of a connection to VoicemeeterRemote.
/// </summary>
/// <param name="loginStatus"></param>
/// <param name="buttonsState"></param>
/// <param name="runningKind"></param>
/// <param name="runningVersion"></param>
public readonly struct ConnectionState(LoginResponse loginStatus, RunResponse buttonsState, Kind runningKind, VmVersion runningVersion)
    : IEquatable<ConnectionState>
{
    /// <summary>
    ///   HashCode will be positive if logged in, negative if logged out.
    /// </summary>
    /// <remarks>
    ///   <code>(int)LoginStatus &lt;&lt; 30 | (int)ButtonsState &lt;&lt; 28 | (int)RunningKind &lt;&lt; 26 | (int)RunningVersion</code>
    /// </remarks>
    public int HashCode { get; } = unchecked(
        ((int)loginStatus << 30) |
        ((int)buttonsState << 28) |
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
    public RunResponse ButtonsState => (RunResponse)((this.HashCode >> 28) & 0x3);

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
    ///   `true` if logged in to VoicemeeterRemote.
    /// </remarks>
    public bool LoggedIn => this.LoginStatus < LoginResponse.LoggedOut;

    /// <summary>
    ///   Simplifies <see cref="LoginStatus"/> checks.
    /// </summary>
    /// <remarks>
    ///   `true` if logged in to VoicemeeterRemote and Voicemeeter is running.
    /// </remarks>
    public bool Connected => this.LoginStatus == LoginResponse.Ok;

    /// <summary>
    ///   Simplifies <see cref="ButtonsState"/> checks.
    /// </summary>
    /// <remarks>
    ///   `true` if MacroButtons is running and responding.
    /// </remarks>
    public bool ButtonsRunning => this.ButtonsState < RunResponse.NotRunning;

    public ConnectionState()
        : this(LoginResponse.LoggedOut, RunResponse.NotRunning, Kind.None, default)
    { }

    public void Deconstruct(out LoginResponse login, out RunResponse buttons, out Kind kind, out VmVersion version)
    {
        login = this.LoginStatus;
        buttons = this.ButtonsState;
        kind = this.RunningKind;
        version = this.RunningVersion;
    }

    public override string ToString() => $"""
        LoginStatus: {this.LoginStatus}
        ButtonsState: {this.ButtonsState}
        RunningKind: {this.RunningKind}
        RunningVersion: {this.RunningVersion}
        """;

    public static explicit operator ConnectionState((LoginResponse login, RunResponse buttons, Kind kind, VmVersion version) t)
        => new(t.login, t.buttons, t.kind, t.version);
    public static explicit operator (LoginResponse login, RunResponse buttons, Kind kind, VmVersion version)(ConnectionState state)
        => (state.LoginStatus, state.ButtonsState, state.RunningKind, state.RunningVersion);

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
