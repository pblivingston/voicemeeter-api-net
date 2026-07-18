// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

public class UnhandledResponseException : RemoteException
{
    private const string UnhandledResponseMessage = "API or wrapper returned an unexpected response.";

    public UnhandledResponseException(LoginResponse response, ConnectionState lastConnectionState)
        : base(UnhandledResponseMessage, response, lastConnectionState)
    { }

    public UnhandledResponseException(RunResponse response, ConnectionState lastConnectionState)
        : base(UnhandledResponseMessage, response, lastConnectionState)
    { }

    public UnhandledResponseException(Response response, ConnectionState lastConnectionState)
        : base(UnhandledResponseMessage, response, lastConnectionState)
    { }
}
