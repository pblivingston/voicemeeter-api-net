// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

public class VersionNotSupportedException(VmVersion runningVersion, ConnectionState lastConnectionState)
    : RemoteException("Running Voicemeeter was not a supported version.", lastConnectionState)
{
    public VmVersion RunningVersion { get; } = runningVersion;

    public override string Message
        => base.Message + Environment.NewLine +
            $"Running Version: {this.RunningVersion}";
}
