// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

public partial class Remote
{
    private partial class Wrapper
    {
        /// <inheritdoc/>
        public LoginResponse Login()
            => (LoginResponse)this.remoteApiWrapper.Login();
        /// <inheritdoc/>
        public LoginResponse Logout()
            => (LoginResponse)this.remoteApiWrapper.Logout();
        /// <inheritdoc/>
        public RunResponse RunVoicemeeter(App app)
            => (RunResponse)this.remoteApiWrapper.RunVoicemeeter((int)app);

        /// <inheritdoc/>
        public (Response, Kind) GetVoicemeeterKind()
            => ((Response)this.remoteApiWrapper.GetVoicemeeterType(out var type), (Kind)type);
        /// <inheritdoc/>
        public (Response, VmVersion) GetVoicemeeterVersion()
            => ((Response)this.remoteApiWrapper.GetVoicemeeterVersion(out var version), (VmVersion)version);

        /// <inheritdoc/>
        public Response IsParametersDirty()
            => (Response)this.remoteApiWrapper.IsParametersDirty();
        /// <inheritdoc/>
        public (Response, float) GetParameter_Float(string param)
            => ((Response)this.remoteApiWrapper.GetParameter(param, out float value), value);
        /// <inheritdoc/>
        public (Response, string) GetParameter_String(string param)
            => ((Response)this.remoteApiWrapper.GetParameter(param, out string value), value);

        /// <inheritdoc/>
        public Response MacroButtonIsDirty()
            => (Response)this.remoteApiWrapper.MacroButtonIsDirty();
    }
}
