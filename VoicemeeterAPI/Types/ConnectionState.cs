// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;

namespace PBLivingston.VoicemeeterAPI.Types;

/// <summary>
///   Snapshot of the state of a connection to VoicemeeterRemote.
/// </summary>
/// <param name="loginStatus"></param>
/// <param name="runningKind"></param>
/// <param name="runningVersion"></param>
public class ConnectionStateEventArgs(LoginResponse loginStatus, Kind runningKind, VmVersion runningVersion) : EventArgs
{
    public LoginResponse LoginStatus { get; } = loginStatus;
    public Kind RunningKind { get; } = runningKind;
    public VmVersion RunningVersion { get; } = runningVersion;

    public bool LoggedIn => LoginStatus < LoginResponse.LoggedOut;
    public bool Connected => LoginStatus == LoginResponse.Ok;

    public string MemberString => $"LoginStatus: {LoginStatus}; RunningKind: {RunningKind}; RunningVersion: {RunningVersion}";
}