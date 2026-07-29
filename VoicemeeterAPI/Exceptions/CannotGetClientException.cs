// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

public class CannotGetClientException(ConnectionState lastConnectionState)
    : RemoteException("Unable to log in to VoicemeeterRemote. Client application limit has likely been reached.", lastConnectionState)
{ }
