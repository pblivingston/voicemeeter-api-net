// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using System.Diagnostics;
using AtgDev.Voicemeeter;
using PBLivingston.VoicemeeterAPI.Types;
using PBLivingston.VoicemeeterAPI.Utilities;

/// <summary>
///   Implements the <see cref="IWrapper"/> interface to abstract underlying calls to the VoicemeeterRemote API.
/// </summary>
/// <param name="remoteApiWrapper"><see cref="RemoteApiWrapper"/></param>
/// <remarks>
///   The primary constructor initializes a new instance of the <see cref="Wrapper"/> class with a provided <see cref="RemoteApiWrapper"/>.
/// </remarks>
internal sealed partial class Wrapper : IWrapper
{
    private const string VmrName = "VoicemeeterRemote";
    private const string MbName = "VoicemeeterMacroButtons";

    private readonly RemoteApiWrapper remoteApiWrapper;
    private readonly Dictionary<App, ProcessWrapper> apps = [];
    private Process? macroButtons;

    public bool Is64Bit { get; } = Environment.Is64BitProcess;
    public string InstallDir { get; } = PathHelperExt.GetInstallDirectory();

    private string DllName => VmrName + (this.Is64Bit ? "64.dll" : ".dll");

    public Wrapper(RemoteApiWrapper remoteApiWrapper)
    {
        this.remoteApiWrapper = remoteApiWrapper
            ?? throw new ArgumentNullException(nameof(remoteApiWrapper));

        this.InitApps();
    }

    public Wrapper(string installDir)
    {
        this.InstallDir = installDir;
        this.remoteApiWrapper = new RemoteApiWrapper(Path.Combine(this.InstallDir, this.DllName));

        this.InitApps();
    }

    public Wrapper()
    {
        this.remoteApiWrapper = new RemoteApiWrapper(Path.Combine(this.InstallDir, this.DllName));

        this.InitApps();
    }

    public void Dispose()
    {
        this.ReleaseMacroButtons();
        this.ReleaseProcesses();
        this.remoteApiWrapper.Dispose();
    }

    private void InitApps()
    {
        this.apps.Add(App.Standard, new(App.Standard, this.InstallDir, "voicemeeter"));
        this.apps.Add(App.Banana, new(App.Banana, this.InstallDir, "voicemeeterpro"));
        this.apps.Add(App.Potato, new(App.Potato, this.InstallDir, "voicemeeter8"));
        this.apps.Add(App.Standardx64, new(App.Standardx64, this.InstallDir, "voicemeeter_x64"));
        this.apps.Add(App.Bananax64, new(App.Bananax64, this.InstallDir, "voicemeeterpro_x64"));
        this.apps.Add(App.Potatox64, new(App.Potatox64, this.InstallDir, "voicemeeter8x64"));
        this.apps.Add(App.DeviceCheck, new(App.DeviceCheck, this.InstallDir, "VBDeviceCheck"));
        this.apps.Add(App.MacroButtons, new(App.MacroButtons, this.InstallDir, "VoicemeeterMacroButtons"));
        this.apps.Add(App.StreamerView, new(App.StreamerView, this.InstallDir, "VMStreamerView"));
        this.apps.Add(App.BUSMatrix8, new(App.BUSMatrix8, this.InstallDir, "VoicemeeterBUSMatrix8"));
        this.apps.Add(App.BUSGEQ15, new(App.BUSGEQ15, this.InstallDir, "VoicemeeterBUSGEQ15"));
        this.apps.Add(App.VBAN2MIDI, new(App.VBAN2MIDI, this.InstallDir, "VBAN2MIDI"));
        this.apps.Add(App.CABLEControlPanel, new(App.CABLEControlPanel, this.InstallDir, "VBCABLE_ControlPanel"));
        this.apps.Add(App.AUXControlPanel, new(App.AUXControlPanel, this.InstallDir, "VBVMAUX_ControlPanel"));
        this.apps.Add(App.VAIO3ControlPanel, new(App.VAIO3ControlPanel, this.InstallDir, "VBVMVAIO3_ControlPanel"));
        this.apps.Add(App.VAIOControlPanel, new(App.VAIOControlPanel, this.InstallDir, "VBVoicemeeterVAIO_ControlPanel"));
    }

    private void ReleaseProcesses()
    {
        foreach ((_, var p) in this.apps)
        {
            p.Dispose();
        }
    }



    /// <inheritdoc/>
    public RunResponse MacroButtonIsRunning()
    {
        if (this.MacroButtonsHasExited())
        {
            this.ReleaseMacroButtons();

            if (!this.MacroButtonsExists())
            {
                return RunResponse.NotInstalled;
            }

            this.macroButtons = this.GetMacroButtons();
        }

        return this.macroButtons is null ? RunResponse.NotRunning : RunResponse.Ok;
    }

    private bool MacroButtonsExists()
        => File.Exists(Path.Combine(this.InstallDir, MbName + ".exe"));

    private bool MacroButtonsHasExited()
    {
        var isClosed = true;
        try
        {
            this.macroButtons?.Refresh();
            isClosed = this.macroButtons?.HasExited ?? true;
        }
        catch { }

        return isClosed;
    }

    private Process? GetMacroButtons()
    {
        var processes = Process.GetProcessesByName(MbName);
        Process? target = null;

        foreach (var p in processes)
        {
            var isValid = false;
            try
            {
                isValid = p.MainModule?.FileName.StartsWith(this.InstallDir, StringComparison.OrdinalIgnoreCase) ?? false;
            }
            catch { }

            if (isValid && target is null)
            {
                target = p;
            }
            else
            {
                p.Dispose();
            }
        }

        return target;
    }

    private void ReleaseMacroButtons()
    {
        this.macroButtons?.Dispose();
        this.macroButtons = null;
    }
}
