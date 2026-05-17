// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Logging;
using PBLivingston.VoicemeeterAPI.Types;

public partial class Remote
{
    private void On_GetInfo_Start(Type infoType, LogLevel level = LogLevel.Trace)
        => RemoteLog.GetInfo_Start(this.logger, level, infoType.Name);

    private void On_GetInfo_Success(Kind value, LogLevel level = LogLevel.Trace)
        => RemoteLog.GetInfo_Success_Kind(this.logger, level, value);

    private void On_GetInfo_Success(VmVersion value, LogLevel level = LogLevel.Trace)
        => RemoteLog.GetInfo_Success_Version(this.logger, level, value);

    private GetInfoException On_GetInfo_Error(InfoResponse response, int value, [CallerMemberName] string methodName = "")
    {
        RemoteLog.GetInfo_Error(this.logger, methodName, response, value);

        return new GetInfoException(response, value);
    }
}
