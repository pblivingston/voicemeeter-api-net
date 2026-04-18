// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using AtgDev.Voicemeeter;
using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI;

/// <summary>
///   Interface to abstract underlying calls to the VoicemeeterRemote API.
/// </summary>
internal interface IWrapper : IDisposable
{
    /// <inheritdoc cref="RemoteApiWrapper.Login()" path="/summary"/>
    LoginResponse Login();
    /// <inheritdoc cref="RemoteApiWrapper.Logout()" path="/summary"/>
    LoginResponse Logout();
    /// <inheritdoc cref="RemoteApiWrapper.RunVoicemeeter(int)" path="/summary"/>
    RunResponse RunVoicemeeter(int app);

    /// <inheritdoc cref="RemoteApiWrapper.GetVoicemeeterType(out int)" path="/summary"/>
    InfoResponse GetVoicemeeterType(out int type);
    /// <inheritdoc cref="RemoteApiWrapper.GetVoicemeeterVersion(out int)" path="/summary"/>
    InfoResponse GetVoicemeeterVersion(out int version);

    /// <inheritdoc cref="RemoteApiWrapper.IsParametersDirty()" path="/summary"/>
    Response IsParametersDirty();
    /// <inheritdoc cref="RemoteApiWrapper.GetParameter(string, out float)" path="/summary"/>
    Response GetParameter(string param, out float value);
    /// <inheritdoc cref="RemoteApiWrapper.GetParameter(string, out string)" path="/summary"/>
    Response GetParameter(string param, out string value);

    /// <inheritdoc cref="RemoteApiWrapper.MacroButtonIsDirty()" path="/summary"/>
    Response MacroButtonIsDirty();
}