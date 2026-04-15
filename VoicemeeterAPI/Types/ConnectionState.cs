// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI.Types;

/// <summary>
///   Snapshot of the state of a connection to VoicemeeterRemote.
/// </summary>
/// <param name="LoginStatus"></param>
/// <param name="RunningKind"></param>
/// <param name="RunningVersion"></param>
public record ConnectionStateRecord(LoginResponse LoginStatus, Kind RunningKind, VmVersion RunningVersion)
{
    public bool LoggedIn => LoginStatus < LoginResponse.LoggedOut;
    public bool Connected => LoginStatus == LoginResponse.Ok;
}