// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using PBLivingston.VoicemeeterAPI.Types;

internal partial class Wrapper
{
    /// <inheritdoc/>
    public LoginResponse Login()
        => (LoginResponse)this.remoteApiWrapper.Login();
    /// <inheritdoc/>
    public LoginResponse Logout()
        => (LoginResponse)this.remoteApiWrapper.Logout();
    /// <inheritdoc/>
    public RunResponse RunVoicemeeter(int app)
        => (RunResponse)this.remoteApiWrapper.RunVoicemeeter(app);

    /// <inheritdoc/>
    public InfoResponse GetVoicemeeterType(out int type)
        => (InfoResponse)this.remoteApiWrapper.GetVoicemeeterType(out type);
    /// <inheritdoc/>
    public InfoResponse GetVoicemeeterVersion(out int version)
        => (InfoResponse)this.remoteApiWrapper.GetVoicemeeterVersion(out version);

    /// <inheritdoc/>
    public Response IsParametersDirty()
        => (Response)this.remoteApiWrapper.IsParametersDirty();
    /// <inheritdoc/>
    public Response GetParameter(string param, out float value)
        => (Response)this.remoteApiWrapper.GetParameter(param, out value);
    /// <inheritdoc/>
    public Response GetParameter(string param, out string value)
        => (Response)this.remoteApiWrapper.GetParameter(param, out value);

    /// <inheritdoc/>
    public Response MacroButtonIsDirty()
        => (Response)this.remoteApiWrapper.MacroButtonIsDirty();
}
