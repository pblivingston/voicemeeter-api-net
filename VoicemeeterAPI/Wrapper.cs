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
internal sealed class Wrapper : IWrapper
{
    private const string VmrName = "VoicemeeterRemote";
    private const string MbName = "VoicemeeterMacroButtons";

    private readonly RemoteApiWrapper remoteApiWrapper;
    private Process? macroButtons;

    public bool Is64Bit { get; } = Environment.Is64BitProcess;
    public string InstallDir { get; } = PathHelperExt.GetInstallDirectory();

    private string DllName => VmrName + (this.Is64Bit ? "64.dll" : ".dll");

    public Wrapper(RemoteApiWrapper remoteApiWrapper)
        => this.remoteApiWrapper = remoteApiWrapper
            ?? throw new ArgumentNullException(nameof(remoteApiWrapper));

    public Wrapper(string installDir)
    {
        this.InstallDir = installDir;
        this.remoteApiWrapper = new RemoteApiWrapper(Path.Combine(this.InstallDir, this.DllName));
    }

    public Wrapper()
        => this.remoteApiWrapper = new RemoteApiWrapper(Path.Combine(this.InstallDir, this.DllName));

    public void Dispose()
    {
        this.ReleaseMacroButtons();

        this.remoteApiWrapper.Dispose();
    }

    /// <inheritdoc/>
    public LoginResponse Login()
        => (LoginResponse)this.remoteApiWrapper.Login();
    /// <inheritdoc/>
    public LoginResponse Logout()
        => (LoginResponse)this.remoteApiWrapper.Logout();
    /// <inheritdoc/>
    public RunResponse RunVoicemeeter(int app)
        => (RunResponse)this.remoteApiWrapper.RunVoicemeeter(app);

    /// <inheritdoc/>
    public InfoResponse GetVoicemeeterType(out int type)
        => (InfoResponse)this.remoteApiWrapper.GetVoicemeeterType(out type);
    /// <inheritdoc/>
    public InfoResponse GetVoicemeeterVersion(out int version)
        => (InfoResponse)this.remoteApiWrapper.GetVoicemeeterVersion(out version);

    /// <inheritdoc/>
    public Response IsParametersDirty()
        => (Response)this.remoteApiWrapper.IsParametersDirty();
    /// <inheritdoc/>
    public Response GetParameter(string param, out float value)
        => (Response)this.remoteApiWrapper.GetParameter(param, out value);
    /// <inheritdoc/>
    public Response GetParameter(string param, out string value)
        => (Response)this.remoteApiWrapper.GetParameter(param, out value);

    /// <inheritdoc/>
    public Response MacroButtonIsDirty()
        => (Response)this.remoteApiWrapper.MacroButtonIsDirty();

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
