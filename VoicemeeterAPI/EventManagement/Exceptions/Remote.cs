// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI.EventManagement.Exceptions;

public class RemoteException(string message)
    : VmApiException(message)
{ }

public class AccessDeniedException(LoginResponse loginStatus)
    : RemoteException($"LoginStatus: {loginStatus}")
{
    public LoginResponse LoginStatus { get; } = loginStatus;
}

public class KindMismatchException(Kind returnedKind, VmVersion returnedVersion)
    : RemoteException($"""
    Kind and version do not match.
    GetKind returned: {returnedKind} ({(int)returnedKind})
    GetVersion returned: {returnedVersion} ({returnedVersion.Kind})
    """)
{
    public Kind ReturnedKind { get; } = returnedKind;
    public VmVersion ReturnedVersion { get; } = returnedVersion;
}

public class RemoteException<T> : RemoteException
    where T : unmanaged
{
    public T Response { get; }

    public RemoteException(T response)
        : base($"Response: {response}")
    {
        Response = response;
    }

    public RemoteException(T response, string message)
        : base($"Response: {response}\r\n{message}")
    {
        Response = response;
    }
}

public class RunException(RunResponse response, App app)
    : RemoteException<RunResponse>(response, $"Requested application: {app}")
{
    public App App { get; } = app;
}

public class GetInfoException(InfoResponse response, int returnedValue)
    : RemoteException<InfoResponse>(response, $"Returned value: {returnedValue}")
{
    public int ReturnedValue = returnedValue;
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