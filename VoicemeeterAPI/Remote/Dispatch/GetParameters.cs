// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using Microsoft.Extensions.Logging;
using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Logging;
using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI;

partial class Remote
{
    private void On_ParamsDirty(LogLevel level = LogLevel.Trace)
    {
        RemoteLog.Query_Success_Response(_logger, level, nameof(IsParamsDirty), Response.Dirty);

        ParamsDirty?.Invoke(this, new EventArgs());
    }

    private void On_GetParam_Start(string vmParam)
    {
        RemoteLog.GetParam_Start(_logger, vmParam);
    }

    private void On_GetParam_Success(string vmParam, float value)
    {
        RemoteLog.GetParam_Success_Float(_logger, vmParam, value);
    }

    private void On_GetParam_Success(string vmParam, int value)
    {
        RemoteLog.GetParam_Success_Int(_logger, vmParam, value);
    }

    private void On_GetParam_Success(string vmParam, bool value)
    {
        RemoteLog.GetParam_Success_Bool(_logger, vmParam, value);
    }

    private void On_GetParam_Success(string vmParam, string value)
    {
        RemoteLog.GetParam_Success_String(_logger, vmParam, value);
    }

    private GetParamException<float> On_GetParam_Error(Response response, string vmParam, float value, Type expectedType)
    {
        RemoteLog.GetParam_Error_Float(_logger, response, vmParam, value, expectedType.Name);

        return new GetParamException<float>(response, vmParam, value, expectedType);
    }

    private GetParamException<string> On_GetParam_Error(Response response, string vmParam, string value, Type expectedType)
    {
        RemoteLog.GetParam_Error_String(_logger, response, vmParam, value, expectedType.Name);

        return new GetParamException<string>(response, vmParam, value, expectedType);
    }
}