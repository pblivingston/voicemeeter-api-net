// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;
using AtgDev.Voicemeeter;

namespace PBLivingston.VoicemeeterAPI;

/// <summary>
///   Interface to abstract underlying calls to the VoicemeeterRemote API.
/// </summary>
public interface IWrapper : IDisposable
{
    /// <inheritdoc cref="RemoteApiWrapper.Login()"/>
    int Login();
    /// <inheritdoc cref="RemoteApiWrapper.Logout()"/>
    int Logout();
    /// <inheritdoc cref="RemoteApiWrapper.RunVoicemeeter(int)"/>
    int RunVoicemeeter(int app);

    /// <inheritdoc cref="RemoteApiWrapper.GetVoicemeeterType(out int)"/>
    int GetVoicemeeterType(out int type);
    /// <inheritdoc cref="RemoteApiWrapper.GetVoicemeeterVersion(out int)"/>
    int GetVoicemeeterVersion(out int version);

    /// <inheritdoc cref="RemoteApiWrapper.IsParametersDirty()"/>
    int IsParametersDirty();
    /// <inheritdoc cref="RemoteApiWrapper.GetParameter(string, out float)"/>
    int GetParameter(string param, out float value);
    /// <inheritdoc cref="RemoteApiWrapper.GetParameter(string, out string)"/>
    int GetParameter(string param, out string value);

    /// <inheritdoc cref="RemoteApiWrapper.MacroButtonIsDirty()"/>
    int MacroButtonIsDirty();
}