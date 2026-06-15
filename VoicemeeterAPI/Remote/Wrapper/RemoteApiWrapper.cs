// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using PBLivingston.VoicemeeterAPI.Types;

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
        public RunResponse RunVoicemeeter(int app)
            => (RunResponse)this.remoteApiWrapper.RunVoicemeeter(app);

        /// <inheritdoc/>
        public (InfoResponse, int) GetVoicemeeterType()
        {
            var response = (InfoResponse)this.remoteApiWrapper.GetVoicemeeterType(out var type);
            return (response, type);
        }
        /// <inheritdoc/>
        public (InfoResponse, int) GetVoicemeeterVersion()
        {
            var response = (InfoResponse)this.remoteApiWrapper.GetVoicemeeterVersion(out var version);
            return (response, version);
        }

        /// <inheritdoc/>
        public Response IsParametersDirty()
            => (Response)this.remoteApiWrapper.IsParametersDirty();
        /// <inheritdoc/>
        public (Response, float) GetParameter_Float(string param)
        {
            var response = (Response)this.remoteApiWrapper.GetParameter(param, out float value);
            return (response, value);
        }
        /// <inheritdoc/>
        public (Response, string) GetParameter_String(string param)
        {
            var response = (Response)this.remoteApiWrapper.GetParameter(param, out string value);
            return (response, value);
        }

        /// <inheritdoc/>
        public Response MacroButtonIsDirty()
            => (Response)this.remoteApiWrapper.MacroButtonIsDirty();
    }
}
