// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using System.Diagnostics;

public partial class Remote
{
    private partial class Wrapper
    {
        /// <summary>
        ///   Gets the current state of the application.
        /// </summary>
        /// <param name="app"></param>
        /// <returns>
        ///   Ok<br/>
        ///   Hidden<br/>
        ///   NotRunning<br/>
        ///   NotResponding<br/>
        ///   NotInstalled<br/>
        ///   UnknownApp<br/>
        /// </returns>
        public RunResponse GetApplicationState(App app)
            => app.IsValid()
                ? this.apps[app].GetState()
                : RunResponse.UnknownApp;

        /// <summary>
        ///   Gets the current Voicemeeter application and its state.
        /// </summary>
        /// <returns></returns>
        public (App, RunResponse) GetVoicemeeterState()
        {
            var app = this.RefreshApps();
            return (
                app, app is App.None
                    ? RunResponse.NotRunning
                    : this.apps[app].GetState()
            );
        }

        /// <summary>
        ///   Attempts to close the process.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="force"></param>
        /// <returns>
        ///   Error<br/>
        ///   UnknownApp<br/>
        ///   Last App State<br/>
        /// </returns>
        /// <remarks>
        ///   If app has tray mode enabled, force will be required to shut it down.
        /// </remarks>
        public RunResponse CloseApplication(App app, bool force = false)
            => app.IsValid()
                ? this.apps[app].Close(force)
                : RunResponse.UnknownApp;

        /// <summary>
        ///   Waits for the application to enter an idle state.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        ///   Error<br/>
        ///   UnknownApp<br/>
        ///   App State<br/>
        /// </returns>
        public async Task<RunResponse> WaitForApplicationInputIdle(App app, CancellationToken cancellationToken)
            => app.IsValid()
                ? await this.apps[app].WaitForInputIdle(cancellationToken)
                : RunResponse.UnknownApp;

        /// <summary>
        ///   Waits for the application to exit.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        ///   Error<br/>
        ///   UnknownApp<br/>
        ///   App State<br/>
        /// </returns>
        public async Task<RunResponse> WaitForApplicationExit(App app, CancellationToken cancellationToken)
            => app.IsValid()
                ? await this.apps[app].WaitForExit(cancellationToken)
                : RunResponse.UnknownApp;

        private enum ProcessName
        {
            voicemeeter = App.Standard,
            voicemeeterpro = App.Banana,
            voicemeeter8 = App.Potato,
            voicemeeter_x64 = App.Standardx64,
            voicemeeterpro_x64 = App.Bananax64,
            voicemeeter8x64 = App.Potatox64,
            VBDeviceCheck = App.DeviceCheck,
            VoicemeeterMacroButtons = App.MacroButtons,
            VMStreamerView = App.StreamerView,
            VoicemeeterBUSMatrix8 = App.BUSMatrix8,
            VoicemeeterBUSGEQ15 = App.BUSGEQ15,
            VBAN2MIDI = App.VBAN2MIDI,
            VBCABLE_ControlPanel = App.CABLEControlPanel,
            VBVMAUX_ControlPanel = App.AUXControlPanel,
            VBVMVAIO3_ControlPanel = App.VAIO3ControlPanel,
            VBVoicemeeterVAIO_ControlPanel = App.VAIOControlPanel
        }

        private void InitApps()
        {
#if NET5_0_OR_GREATER
            foreach (var name in Enum.GetValues<ProcessName>())
#else
            foreach (ProcessName name in Enum.GetValues(typeof(ProcessName)))
#endif
            {
                this.apps.Add((App)name, new(name, this.InstallDir));
            }
        }

        private void ReleaseApps()
        {
            foreach (var a in this.apps)
            {
                a.Value.Dispose();
            }
        }

        /// <summary>
        ///   Assigns running VB-Audio apps to their appropriate cache entries.
        /// </summary>
        /// <returns>
        ///   The active Voicemeeter application, otherwise 'None'.
        /// </returns>
        private App RefreshApps()
        {
            var processes = Process.GetProcesses();

            var vm = App.None;
            foreach (var p in processes)
            {
                if (!(
                    Enum.TryParse(p.ProcessName, out ProcessName name)
                    && (App)name is var app
                    && app.IsValid()
                    && this.apps[app].TryAssignDiscovered(p)
                ))
                {
                    p.Dispose();
                    continue;
                }

                if (app.IsVoicemeeter())
                {
                    vm = app;
                }
            }

            return vm;
        }
    }
}
