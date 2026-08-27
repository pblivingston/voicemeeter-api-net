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
    private readonly int packed = Pack(loginStatus, vmState, vmApp, vmVersion, buttonsState);

    /// <summary>
    ///   The login status of the <see cref="IRemote"/> instance.
    /// </summary>
    /// <remarks>
    ///   Ok, VoicemeeterNotRunning, LoggedOut
    /// </remarks>
    public LoginResponse LoginStatus => ((this.packed >> 31) & 1) == 0
        ? (LoginResponse)((this.packed >> 30) & 0x3)
        : LoginResponse.LoggedOut;

    /// <summary>
    ///   The state of the Voicemeeter application.
    /// </summary>
    public RunResponse VoicemeeterState => (RunResponse)((this.packed >> 29) & 0x3);

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
    public VmVersion VoicemeeterVersion => vmState is RunResponse.NotResponding
        ? default
        : (VmVersion)((this.packed >> 2) & 0x3FFFFFF);

    /// <summary>
    ///   The state of the MacroButtons application.
    /// </summary>
    public RunResponse MacroButtonsState => (RunResponse)(this.packed & 0x3);

    /// <summary>
    ///   `true` if logged in to VoicemeeterRemote.
    /// </summary>
    public bool LoggedIn => this.LoginStatus.IsLoggedIn();

    /// <summary>
    ///   `true` if logged in to VoicemeeterRemote and Voicemeeter is running.
    /// </summary>
    public bool ConnectedToVoicemeeter => this.LoginStatus == LoginResponse.Ok;

    /// <summary>
    ///   `true` if MacroButtons is responding and reachable via Voicemeeter.
    /// </summary>
    public bool ConnectedToMacroButtons => this.ConnectedToVoicemeeter && this.MacroButtonsState.IsResponding();

    public ConnectionState()
        : this(LoginResponse.LoggedOut, RunResponse.NotRunning, App.None, default, RunResponse.NotRunning)
    { }

    public static int Pack(LoginResponse loginStatus, RunResponse vmState, App vmApp, VmVersion vmVersion, RunResponse buttonsState)
    {
        Utilities.ThrowIfNotInRange(loginStatus, LoginResponse.Ok, LoginResponse.LoggedOut);
        Utilities.ThrowIfNotInRange(vmState, RunResponse.Ok, RunResponse.NotResponding);
        Utilities.ThrowIfNotInRange(vmApp, App.None, App.Potatox64);
        Utilities.ThrowIfNotInRange(vmVersion, default, VmVersion.MaxValid);
        Utilities.ThrowIfNotInRange(buttonsState, RunResponse.Ok, RunResponse.NotResponding);

        if ((loginStatus is LoginResponse.Ok && vmState is not (RunResponse.Ok or RunResponse.Hidden)) ||
            (loginStatus is LoginResponse.VoicemeeterNotRunning && vmState is not (RunResponse.NotRunning or RunResponse.NotResponding)))
        {
            throw new ArgumentException($"LoginStatus '{loginStatus}' does not match VoicemeeterState '{vmState}'.");
        }

        if (!((vmState is RunResponse.NotResponding && vmVersion == default)
            || (vmApp.ToKind() == vmVersion.K)))
        {
            throw new ArgumentException($"Voicemeeter app '{vmApp}' and state '{vmState}' do not match Voicemeeter version '{vmVersion}'.");
        }

        return unchecked(
            // 000: Ok & Ok
            // 001: Ok & Hidden
            // 010: VoicemeeterNotRunning & NotRunning
            // 011: VoicemeeterNotRunning & NotResponding
            // 100: LoggedOut & previously Ok
            // 101: LoggedOut & previously Hidden
            // 110: LoggedOut & previously NotRunning
            // 111: LoggedOut & previously NotResponding
            (((int)loginStatus >> 1) << 31) | // only grab top bit "logged in/out"
            ((int)vmState << 29) |
            // 0: 32 bit app
            // 1: 64 bit app
            ((vmApp < App.Standardx64 ? 0 : 1) << 28) |
            (vmState is RunResponse.NotResponding
                ? ((int)vmApp.ToKind() << 26)
                : ((int)vmVersion << 2)) |
            ((int)buttonsState)
        );
    }

    public override string ToString()
    {
        var builder = new System.Text.StringBuilder();

        return builder
            .Append("{ ")
            .AddArg(nameof(this.LoginStatus), this.LoginStatus)
            .AddArg(nameof(this.VoicemeeterState), this.VoicemeeterState)
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
