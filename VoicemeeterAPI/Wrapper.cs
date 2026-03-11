// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using AtgDev.Voicemeeter;

namespace VoicemeeterAPI;

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
    public int Login() => _remoteApiWrapper.Login();
    /// <inheritdoc/>
    public int Logout() => _remoteApiWrapper.Logout();
    /// <inheritdoc/>
    public int RunVoicemeeter(int app) => _remoteApiWrapper.RunVoicemeeter(app);

    /// <inheritdoc/>
    public int GetVoicemeeterType(out int type) => _remoteApiWrapper.GetVoicemeeterType(out type);
    /// <inheritdoc/>
    public int GetVoicemeeterVersion(out int version) => _remoteApiWrapper.GetVoicemeeterVersion(out version);

    /// <inheritdoc/>
    public int IsParametersDirty() => _remoteApiWrapper.IsParametersDirty();
    /// <inheritdoc/>
    public int GetParameter(string param, out float value) => _remoteApiWrapper.GetParameter(param, out value);
    /// <inheritdoc/>
    public int GetParameter(string param, out string value) => _remoteApiWrapper.GetParameter(param, out value);

    /// <inheritdoc/>
    public int MacroButtonIsDirty() => _remoteApiWrapper.MacroButtonIsDirty();
}