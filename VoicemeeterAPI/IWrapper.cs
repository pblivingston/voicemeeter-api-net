// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using AtgDev.Voicemeeter;
using PBLivingston.VoicemeeterAPI.Types;

/// <summary>
///   Interface to abstract underlying calls to the VoicemeeterRemote API.
/// </summary>
internal interface IWrapper : IDisposable
{
    public bool Is64Bit { get; }
    public string InstallDir { get; }

    /// <inheritdoc cref="RemoteApiWrapper.Login()" path="/summary"/>
    public LoginResponse Login();
    /// <inheritdoc cref="RemoteApiWrapper.Logout()" path="/summary"/>
    public LoginResponse Logout();
    /// <inheritdoc cref="RemoteApiWrapper.RunVoicemeeter(int)" path="/summary"/>
    public RunResponse RunVoicemeeter(int app);

    /// <inheritdoc cref="RemoteApiWrapper.GetVoicemeeterType(out int)" path="/summary"/>
    public InfoResponse GetVoicemeeterType(out int type);
    /// <inheritdoc cref="RemoteApiWrapper.GetVoicemeeterVersion(out int)" path="/summary"/>
    public InfoResponse GetVoicemeeterVersion(out int version);

    /// <inheritdoc cref="RemoteApiWrapper.IsParametersDirty()" path="/summary"/>
    public Response IsParametersDirty();
    /// <inheritdoc cref="RemoteApiWrapper.GetParameter(string, out float)" path="/summary"/>
    public Response GetParameter(string param, out float value);
    /// <inheritdoc cref="RemoteApiWrapper.GetParameter(string, out string)" path="/summary"/>
    public Response GetParameter(string param, out string value);

    /// <summary>
    ///   Check if MacroButtons is running.
    /// </summary>
    /// <returns>
    ///   Ok<br/>
    ///   NotRunning<br/>
    ///   NotInstalled<br/>
    /// </returns>
    public RunResponse MacroButtonIsRunning();
    /// <inheritdoc cref="RemoteApiWrapper.MacroButtonIsDirty()" path="/summary"/>
    public Response MacroButtonIsDirty();

    public RunResponse GetApplicationState(App app);
    public RunResponse ShutdownApplication(App app, bool force);
    public Response WaitForApplicationInputIdle(App app);
    public Response WaitForApplicationInputIdle(App app, int milliseconds);
    public Response WaitForApplicationExit(App app);
    public Response WaitForApplicationExit(App app, int milliseconds);
}
