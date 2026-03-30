// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using PBLivingston.VoicemeeterAPI.Types;
using PBLivingston.VoicemeeterAPI.Messages;

namespace PBLivingston.VoicemeeterAPI;

partial class Remote
{
    #region Get Voicemeeter Kind

    /// <inheritdoc/>
    public Kind GetKind()
    {
        LoginGuard(requiredStatus: LoginResponse.VoicemeeterNotRunning);

        var result = _vmrApi.GetVoicemeeterType(out int k);
        var kind = (Kind)k;

        if (result == InfoResponse.Ok && kind.IsValid())
        {
            LoginStatus = LoginResponse.Ok;
            return kind;
        }

        if (result == InfoResponse.NoServer)
        {
            LoginStatus = LoginResponse.VoicemeeterNotRunning;
            return Kind.None;
        }

        throw new RemoteException($"GetKind failed - {result}; returned kind: {kind}");
    }

    #endregion

    #region Get Voicemeeter Version

    /// <inheritdoc/>
    public VmVersion GetVersion()
    {
        LoginGuard(requiredStatus: LoginResponse.VoicemeeterNotRunning);

        var result = _vmrApi.GetVoicemeeterVersion(out int v);

        if (result == InfoResponse.Ok && VmVersion.IsValid(v))
        {
            LoginStatus = LoginResponse.Ok;
            return (VmVersion)v;
        }

        if (result == InfoResponse.NoServer)
        {
            LoginStatus = LoginResponse.VoicemeeterNotRunning;
            return default;
        }

        throw new RemoteException($"GetVersion failed - {result}; returned version: {VersionUtils.ToString(v)}");
    }

    #endregion
}
