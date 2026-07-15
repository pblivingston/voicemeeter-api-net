// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

public class RemoteException : VmApiException
{
    public ConnectionState? LastConnectionState { get; }
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

    public RemoteException(string message, object response)
        : base(message)
        => this.Response = response;

    public RemoteException(string message)
        : base(message)
    { }

    public RemoteException()
        : base()
    { }

    public override string Message
    {
        get
        {
            var lines = new List<string>();

            if (!string.IsNullOrWhiteSpace(base.Message))
            {
                lines.Add(base.Message);
            }

            if (this.LastConnectionState is not null)
            {
                lines.Add($"Last Connection State: {this.LastConnectionState}");
            }

            if (this.Response is not null)
            {
                lines.Add($"Response: {this.Response}");
            }

            return string.Join(Environment.NewLine, lines);
        }
    }
}



#region to be deprecated

public class RemoteException<T> : RemoteException
    where T : unmanaged
{
    public RemoteException(T response, string message)
        : base(message, response)
    { }

    public RemoteException(T response)
        : base("", response)
    { }
}

public class AccessDeniedException(LoginResponse loginStatus)
    : RemoteException($"LoginStatus: {loginStatus}")
{
    public LoginResponse LoginStatus { get; } = loginStatus;
}

public class RunException(RunResponse response, App app)
    : RemoteException<RunResponse>(response, $"Requested application: {app}")
{
    public App App { get; } = app;
}

public class GetInfoException(InfoResponse response, int returnedValue)
    : RemoteException<InfoResponse>(response, $"Returned value: {returnedValue}")
{
    public int ReturnedValue { get; } = returnedValue;
}

public class GetParamException<T>(Response response, string vmParam, T returnedValue, Type expectedType)
    : RemoteException<Response>(response, $"""
    Requested Voicemeeter parameter: {vmParam}
    Returned value: {returnedValue}
    Expected type: {expectedType}
    """)
    where T : notnull
{
    public string VmParam { get; } = vmParam;
    public T ReturnedValue { get; } = returnedValue;
    public Type ExpectedType { get; } = expectedType;
}

#endregion
