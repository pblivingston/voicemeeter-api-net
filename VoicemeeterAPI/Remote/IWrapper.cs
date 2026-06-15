// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using AtgDev.Voicemeeter;
using PBLivingston.VoicemeeterAPI.Types;

public partial class Remote
{
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
        public (InfoResponse, int) GetVoicemeeterType();
        /// <inheritdoc cref="RemoteApiWrapper.GetVoicemeeterVersion(out int)" path="/summary"/>
        public (InfoResponse, int) GetVoicemeeterVersion();

        /// <inheritdoc cref="RemoteApiWrapper.IsParametersDirty()" path="/summary"/>
        public Response IsParametersDirty();
        /// <inheritdoc cref="RemoteApiWrapper.GetParameter(string, out float)" path="/summary"/>
        public (Response, float) GetParameter_Float(string param);
        /// <inheritdoc cref="RemoteApiWrapper.GetParameter(string, out string)" path="/summary"/>
        public (Response, string) GetParameter_String(string param);

        /// <inheritdoc cref="RemoteApiWrapper.MacroButtonIsDirty()" path="/summary"/>
        public Response MacroButtonIsDirty();

        /// <inheritdoc cref="Wrapper.GetApplicationState(App)"/>
        public RunResponse GetApplicationState(App app);
        /// <inheritdoc cref="Wrapper.CloseApplication(App, bool)"/>
        public RunResponse CloseApplication(App app, bool force);
        /// <inheritdoc cref="Wrapper.IsApplicationInputIdle(App)"/>
        public Response IsApplicationInputIdle(App app);
        /// <inheritdoc cref="Wrapper.WaitForApplicationExit(App, CancellationToken)"/>
        public Task<RunResponse> WaitForApplicationExit(App app, CancellationToken cancellationToken = default);
    }
}
