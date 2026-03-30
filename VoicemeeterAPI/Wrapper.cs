// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using AtgDev.Voicemeeter;
using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI;

/// <summary>
///   Implements the <see cref="IWrapper"/> interface to abstract underlying calls to the VoicemeeterRemote API.
/// </summary>
/// <param name="remoteApiWrapper"><see cref="RemoteApiWrapper"/></param>
/// <remarks>
///   The primary constructor initializes a new instance of the <see cref="Wrapper"/> class with a provided <see cref="RemoteApiWrapper"/>.
/// </remarks>
internal sealed class Wrapper(RemoteApiWrapper remoteApiWrapper) : IWrapper
{
    private readonly RemoteApiWrapper _remoteApiWrapper = remoteApiWrapper;

    public void Dispose() => _remoteApiWrapper.Dispose();

    /// <inheritdoc/>
    public LoginResponse Login() => (LoginResponse)_remoteApiWrapper.Login();
    /// <inheritdoc/>
    public LoginResponse Logout() => (LoginResponse)_remoteApiWrapper.Logout();
    /// <inheritdoc/>
    public RunResponse RunVoicemeeter(int app) => (RunResponse)_remoteApiWrapper.RunVoicemeeter(app);

    /// <inheritdoc/>
    public InfoResponse GetVoicemeeterType(out int type) => (InfoResponse)_remoteApiWrapper.GetVoicemeeterType(out type);
    /// <inheritdoc/>
    public InfoResponse GetVoicemeeterVersion(out int version) => (InfoResponse)_remoteApiWrapper.GetVoicemeeterVersion(out version);

    /// <inheritdoc/>
    public Response IsParametersDirty() => (Response)_remoteApiWrapper.IsParametersDirty();
    /// <inheritdoc/>
    public Response GetParameter(string param, out float value) => (Response)_remoteApiWrapper.GetParameter(param, out value);
    /// <inheritdoc/>
    public Response GetParameter(string param, out string value) => (Response)_remoteApiWrapper.GetParameter(param, out value);

    /// <inheritdoc/>
    public Response MacroButtonIsDirty() => (Response)_remoteApiWrapper.MacroButtonIsDirty();
}