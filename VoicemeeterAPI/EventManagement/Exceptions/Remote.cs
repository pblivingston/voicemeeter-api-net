// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI.EventManagement.Exceptions;

internal class AccessDeniedException(LoginResponse loginStatus)
    : VmApiException($"LoginStatus: {loginStatus}")
{
    public LoginResponse LoginStatus { get; } = loginStatus;
}

internal class RemoteException<T> : VmApiException
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

internal class RunException(RunResponse response, App app)
    : RemoteException<RunResponse>(response, $"Requested application: {app}")
{
    public App App { get; } = app;
}

internal class GetInfoException(InfoResponse response, int value)
    : RemoteException<InfoResponse>(response, $"Returned value: {value}")
{
    public int Value = value;
}

internal class GetParamException<T>(Response response, string vmParam, T value, Type expectedType)
    : RemoteException<Response>(response, $"Requested Voicemeeter parameter: {vmParam}\r\nReturned value: {value}\r\nExpected type: {expectedType}")
{
    public string VmParam { get; } = vmParam;
    public T Value { get; } = value;
    public Type ExpectedType { get; } = expectedType;
}