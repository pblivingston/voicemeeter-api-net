// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using PBLivingston.VoicemeeterAPI.Types;
using PBLivingston.VoicemeeterAPI.Exceptions;

namespace PBLivingston.VoicemeeterAPI;

partial class Remote
{
    #region Get Voicemeeter Kind

    /// <inheritdoc cref="IRemote.GetKind()"/>
    internal Kind GetKind(bool nested = false)
    {
        LoginGuard(requiredStatus: LoginResponse.VoicemeeterNotRunning);

        var result = _vmrApi.GetVoicemeeterType(out int k);
        var kind = (Kind)k;

        if (result == InfoResponse.Ok && kind.IsValid())
        {
            if (LoginStatus != LoginResponse.Ok)
                LoginStatus = LoginResponse.Ok;

            if (RunningKind != kind)
                RunningKind = kind;
        }
        else if (result == InfoResponse.NoServer)
        {
            if (LoginStatus != LoginResponse.VoicemeeterNotRunning)
                LoginStatus = LoginResponse.VoicemeeterNotRunning;

            if (RunningKind != Kind.None)
                RunningKind = Kind.None;
        }
        else throw new RemoteException($"GetKind failed - {result}; returned kind: {kind}");

        if (RunningKind != RunningVersion.K)
        {
            if (nested) throw new RemoteException($"RunningKind '{RunningKind}' and RunningVersion '{RunningVersion}' do not match");

            GetVersion(true);
        }

        return RunningKind;
    }

    /// <inheritdoc/>
    public Kind GetKind() => GetKind(false);

    #endregion

    #region Get Voicemeeter Version

    /// <inheritdoc cref="IRemote.GetVersion()"/>
    internal VmVersion GetVersion(bool nested)
    {
        LoginGuard(requiredStatus: LoginResponse.VoicemeeterNotRunning);

        var result = _vmrApi.GetVoicemeeterVersion(out int v);

        if (result == InfoResponse.Ok && VmVersion.IsValid(v))
        {
            if (LoginStatus != LoginResponse.Ok)
                LoginStatus = LoginResponse.Ok;

            if (RunningVersion.Packed != v)
                RunningVersion = (VmVersion)v;
        }
        else if (result == InfoResponse.NoServer)
        {
            if (LoginStatus != LoginResponse.VoicemeeterNotRunning)
                LoginStatus = LoginResponse.VoicemeeterNotRunning;

            if (RunningVersion != default)
                RunningVersion = default;
        }
        else throw new RemoteException($"GetVersion failed - {result}; returned version: {VersionUtils.ToString(v)}");

        if (RunningVersion.K != RunningKind)
        {
            if (nested) throw new RemoteException($"RunningVersion '{RunningVersion}' and RunningKind '{RunningKind}' do not match");

            GetKind(true);
        }

        return RunningVersion;
    }

    /// <inheritdoc/>
    public VmVersion GetVersion() => GetVersion(false);

    #endregion
}
