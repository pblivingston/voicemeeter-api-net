// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Logging;
using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI;

partial class Remote
{
    #region Login

    private void On_Login_Start()
    {
        RemoteLog.Login_Start(_logger);
    }

    private void On_Login_VmNotRunning()
    {
        RemoteLog.Login_VmNotRunning(_logger);
    }

    #endregion

    #region Logout

    private void On_Logout_Start()
    {
        RemoteLog.Logout_Start(_logger);
    }

    private void On_Logout_Timeout(LoginResponse lastResponse)
    {
        RemoteLog.Logout_Timeout(_logger, lastResponse);
    }

    #endregion

    #region Run

    private void On_Run_Start(App app)
    {
        RemoteLog.Run_Start(_logger, app);
    }

    private RunException On_Run_Error(RunResponse response, App app)
    {
        RemoteLog.Run_Error(_logger, response, app);

        return new RunException(response, app);
    }

    #endregion

    #region WaitForRunning

    private void On_WaitForRunning_Start()
    {
        RemoteLog.WaitForRunning_Start(_logger);
    }

    private void On_WaitForRunning_Detected(Kind kind, VmVersion version)
    {
        RemoteLog.WaitForRunning_Detected(_logger, kind, version);
    }

    private void On_WaitForRunning_Timeout()
    {
        RemoteLog.WaitForRunning_Timeout(_logger);
    }

    #endregion
}