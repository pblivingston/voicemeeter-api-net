// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using Microsoft.Extensions.Logging;
using PBLivingston.VoicemeeterAPI.EventManagement.Exceptions;
using PBLivingston.VoicemeeterAPI.EventManagement.Logging;
using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI.EventManagement;

internal static partial class RemoteDispatch
{
    #region Login

    public static void Login_Start(ILogger logger)
    {
        RemoteLog.Login_Start(logger);
    }

    public static void Login_VmNotRunning(ILogger logger)
    {
        RemoteLog.Login_VmNotRunning(logger);
    }

    #endregion

    #region Logout

    public static void Logout_Start(ILogger logger)
    {
        RemoteLog.Logout_Start(logger);
    }

    public static void Logout_AlreadyLoggedOut(ILogger logger)
    {
        RemoteLog.Method_Error(logger, nameof(Remote.Logout), nameof(LoginResponse.AlreadyLoggedOut));
    }

    public static void Logout_Timeout(ILogger logger, LoginResponse lastResponse)
    {
        RemoteLog.Logout_Timeout(logger, lastResponse.ToString());
    }

    #endregion

    #region Run

    public static void Run_Start(ILogger logger, App app)
    {
        RemoteLog.Run_Start(logger, app.ToString());
    }

    public static void Run_Error(ILogger logger, RunResponse response, App app)
    {
        RemoteLog.Run_Error(logger, response.ToString(), app.ToString());

        throw new RunException(response, app);
    }

    #endregion

    #region WaitForRunning

    public static void WaitForRunning_Start(ILogger logger)
    {
        RemoteLog.WaitForRunning_Start(logger);
    }

    public static void WaitForRunning_Detected(ILogger logger, Kind kind, VmVersion version)
    {
        RemoteLog.WaitForRunning_Detected(logger, kind.ToString(), version.ToString());
    }

    public static void WaitForRunning_Timeout(ILogger logger)
    {
        RemoteLog.WaitForRunning_Timeout(logger);
    }

    #endregion
}