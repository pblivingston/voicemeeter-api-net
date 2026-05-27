// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Logging;
using PBLivingston.VoicemeeterAPI.Types;

public partial class Remote
{
    #region Login

    private void On_Login_Start()
        => RemoteLog.Login_Start(this.logger);

    private void On_Login_VmNotRunning()
        => RemoteLog.Login_VmNotRunning(this.logger);

    private void On_Login_MbNotRunning()
        => RemoteLog.Login_MbNotRunning(this.logger);

    #endregion

    #region Logout

    private void On_Logout_Start()
        => RemoteLog.Logout_Start(this.logger);

    #endregion

    #region Run

    private void On_Run_Start(App app)
        => RemoteLog.Run_Start(this.logger, app);

    private RunException On_Run_Error(RunResponse response, App app)
    {
        RemoteLog.Run_Error(this.logger, response, app);

        return new RunException(response, app);
    }

    #endregion

    #region WaitForRunning

    private void On_WaitForVoicemeeter_Start()
        => RemoteLog.WaitForVoicemeeter_Start(this.logger);

    private void On_WaitForVoicemeeter_Detected(Kind kind, VmVersion version)
        => RemoteLog.WaitForVoicemeeter_Detected(this.logger, kind, version);

    private void On_WaitForVoicemeeter_Timeout()
        => RemoteLog.WaitForVoicemeeter_Timeout(this.logger);

    private void On_WaitForRunning_Start(App app)
        => RemoteLog.WaitForRunning_Start(this.logger, app);

    private void On_WaitForRunning_Detected(App app, RunResponse appState)
        => RemoteLog.WaitForRunning_Detected(this.logger, app, appState);

    private void On_WaitForRunning_Timeout(App app)
        => RemoteLog.WaitForRunning_Timeout(this.logger, app);

    #endregion
}
