// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

public class UnhandledResponseException(object response, ConnectionState lastConnectionState)
    : RemoteException("VoicemeeterRemote dll or wrapper returned an unexpected response.", response, lastConnectionState)
{ }
