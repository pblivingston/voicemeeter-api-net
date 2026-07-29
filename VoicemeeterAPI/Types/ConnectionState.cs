// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

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
/// <param name="vmState"></param>
/// <param name="vmApp"></param>
/// <param name="vmVersion"></param>
/// <param name="buttonsState"></param>
public readonly struct ConnectionState(LoginResponse loginStatus, RunResponse vmState, App vmApp, VmVersion vmVersion, RunResponse buttonsState)
    : IEquatable<ConnectionState>
{
    /// <summary>
    ///   packed will be positive if logged in, negative if logged out.
    /// </summary>
    /// <remarks>
    ///   <code>(int)LoginStatus &lt;&lt; 30 | (int)MacroButtonsState &lt;&lt; 28 | (int)VoicemeeterKind &lt;&lt; 26 | (int)VoicemeeterVersion</code>
    /// </remarks>
    private readonly int packed = unchecked(
        ((int)loginStatus << 30) |
        (((int)vmState & 0x1) << 29) |
        ((vmApp < App.Standardx64 ? 0 : 1) << 28) |
        ((int)vmVersion << 2) |
        ((int)buttonsState)
    );

    /// <summary>
    ///   The login status of the <see cref="IRemote"/> instance.
    /// </summary>
    /// <remarks>
    ///   Ok, VoicemeeterNotRunning, LoggedOut, Unknown
    /// </remarks>
    public LoginResponse LoginStatus => (LoginResponse)((this.packed >> 30) & 0x3);

    /// <summary>
    ///   The state of the Voicemeeter application.
    /// </summary>
    public RunResponse VoicemeeterState => this.LoginStatus < LoginResponse.LoggedOut
        ? (RunResponse)((this.packed >> 29) & 0x3)
        : (RunResponse)(
            ((this.VoicemeeterKind is Kind.None ? 1 : 0) << 1) |
            ((this.packed >> 29) & 0x1)
        );

    /// <summary>
    ///   The running Voicemeeter Kind.
    /// </summary>
    /// <remarks>
    ///   None, Standard, Banana, Potato
    /// </remarks>
    public Kind VoicemeeterKind => (Kind)((this.packed >> 26) & 0x3);

    /// <summary>
    ///   The running Voicemeeter application.
    /// </summary>
    /// <remarks>
    ///   None, Standard, Banana, Potato, Standardx64, Bananax64, Potatox64
    /// </remarks>
    public App VoicemeeterApp => this.VoicemeeterKind.ToApp(((this.packed >> 28) & 0x1) == 1);

    /// <summary>
    ///   The running Voicemeeter version.
    /// </summary>
    public VmVersion VoicemeeterVersion => (VmVersion)((this.packed >> 2) & 0x3FFFFFF);

    /// <summary>
    ///   The state of the MacroButtons application.
    /// </summary>
    public RunResponse MacroButtonsState => (RunResponse)(this.packed & 0x3);

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
    public bool ConnectedToVoicemeeter => this.LoginStatus == LoginResponse.Ok;

    /// <summary>
    ///   Simplifies <see cref="MacroButtonsState"/> checks.
    /// </summary>
    /// <remarks>
    ///   `true` if MacroButtons is running and responding.
    /// </remarks>
    public bool MacroButtonsIsRunning => this.MacroButtonsState < RunResponse.NotRunning;

    /// <summary>
    ///   Simplifies MacroButtons checks.
    /// </summary>
    /// <remarks>
    ///   `true` if MacroButtons is responding and reachable via Voicemeeter.
    /// </remarks>
    public bool ConnectedToMacroButtons => this.ConnectedToVoicemeeter && this.MacroButtonsIsRunning;


    public ConnectionState()
        : this(LoginResponse.LoggedOut, RunResponse.NotRunning, App.None, default, RunResponse.NotRunning)
    { }

    public override string ToString()
    {
        var builder = new System.Text.StringBuilder();

        return builder
            .Append("{ ")
            .AddArg(nameof(this.LoginStatus), this.LoginStatus)
            .AddArg(nameof(this.VoicemeeterState), this.VoicemeeterState)
            .AddArg(nameof(this.VoicemeeterKind), this.VoicemeeterKind)
            .AddArg(nameof(this.VoicemeeterApp), this.VoicemeeterApp)
            .AddArg(nameof(this.VoicemeeterVersion), this.VoicemeeterVersion)
            .AddArg(nameof(this.MacroButtonsState), this.MacroButtonsState)
            .Append('}')
            .ToString();
    }

    public bool Equals(ConnectionState other)
        => this.packed == other.packed;
    public override bool Equals(object? obj)
        => obj is ConnectionState other
        && this.Equals(other);
    public override int GetHashCode()
        => this.packed;

    public static bool operator ==(ConnectionState a, ConnectionState b) => a.packed == b.packed;
    public static bool operator !=(ConnectionState a, ConnectionState b) => a.packed != b.packed;
}
