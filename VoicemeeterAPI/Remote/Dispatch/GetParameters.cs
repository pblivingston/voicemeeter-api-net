// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using Microsoft.Extensions.Logging;
using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Logging;
using PBLivingston.VoicemeeterAPI.Types;

public partial class Remote
{
    private void On_ParamsDirty(LogLevel level = LogLevel.Trace)
    {
        RemoteLog.Query_Success_Response(this.logger, level, nameof(IsParamsDirty), Response.Dirty);

        ParamsDirty?.Invoke(this, new EventArgs());
    }

    private void On_GetParam_Start(string vmParam)
        => RemoteLog.GetParam_Start(this.logger, vmParam);

    private void On_GetParam_Success(string vmParam, float value)
        => RemoteLog.GetParam_Success_Float(this.logger, vmParam, value);

    private void On_GetParam_Success(string vmParam, int value)
        => RemoteLog.GetParam_Success_Int(this.logger, vmParam, value);

    private void On_GetParam_Success(string vmParam, bool value)
        => RemoteLog.GetParam_Success_Bool(this.logger, vmParam, value);

    private void On_GetParam_Success(string vmParam, string value)
        => RemoteLog.GetParam_Success_String(this.logger, vmParam, value);

    private GetParamException<float> On_GetParam_Error(Response response, string vmParam, float value, Type expectedType)
    {
        RemoteLog.GetParam_Error_Float(this.logger, response, vmParam, value, expectedType.Name);

        return new GetParamException<float>(response, vmParam, value, expectedType);
    }

    private GetParamException<string> On_GetParam_Error(Response response, string vmParam, string value, Type expectedType)
    {
        RemoteLog.GetParam_Error_String(this.logger, response, vmParam, value, expectedType.Name);

        return new GetParamException<string>(response, vmParam, value, expectedType);
    }
}
