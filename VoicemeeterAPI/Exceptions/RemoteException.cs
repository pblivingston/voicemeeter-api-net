// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

public class RemoteException : VmApiException
{
    public ConnectionState LastConnectionState { get; }
    public object? Response { get; }

    public RemoteException(string message, LoginResponse response, ConnectionState lastConnectionState)
        : base(message)
    {
        this.LastConnectionState = lastConnectionState;
        this.Response = response;
    }

    public RemoteException(string message, RunResponse response, ConnectionState lastConnectionState)
        : base(message)
    {
        this.LastConnectionState = lastConnectionState;
        this.Response = response;
    }

    public RemoteException(string message, Response response, ConnectionState lastConnectionState)
        : base(message)
    {
        this.LastConnectionState = lastConnectionState;
        this.Response = response;
    }

    public RemoteException(string message, ConnectionState lastConnectionState)
        : base(message)
        => this.LastConnectionState = lastConnectionState;

    public override string Message
    {
        get
        {
            var lines = new List<string>();

            if (!string.IsNullOrWhiteSpace(base.Message))
            {
                lines.Add(base.Message);
            }

            if (this.Response is not null)
            {
                lines.Add($"Response: {this.Response}");
            }

            lines.Add($"Last Connection State: {this.LastConnectionState}");

            return string.Join(Environment.NewLine, lines);
        }
    }
}

public class NoClientException(ConnectionState lastConnectionState)
    : RemoteException("Cannot get remote client.", lastConnectionState)
{ }

public class AlreadyLoggedInException(ConnectionState lastConnectionState)
    : RemoteException("Login should not be performed more than once in a single session.", lastConnectionState)
{ }

public class EngineNotRunningException(ConnectionState lastConnectionState)
    : RemoteException("Operation cannot be performed while Voicemeeter is not running.", lastConnectionState)
{ }
