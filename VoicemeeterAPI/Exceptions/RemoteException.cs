// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

public class RemoteException : VmApiException
{
    public ConnectionState LastConnectionState { get; }
    public object? Response { get; }

    public RemoteException(string message, object response, ConnectionState lastConnectionState)
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
