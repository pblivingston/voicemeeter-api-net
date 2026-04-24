// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System.Diagnostics;
using AtgDev.Voicemeeter;
using PBLivingston.VoicemeeterAPI.Types;
using PBLivingston.VoicemeeterAPI.Utilities;

namespace PBLivingston.VoicemeeterAPI;

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

    private readonly RemoteApiWrapper _remoteApiWrapper;
    private Process? _macroButtons = null;

    public bool Is64Bit { get; } = Environment.Is64BitProcess;
    public string InstallDir { get; } = PathHelperExt.GetInstallDirectory();

    private string DllName => VmrName + (Is64Bit ? "64.dll" : ".dll");

    public Wrapper(RemoteApiWrapper remoteApiWrapper)
    {
        _remoteApiWrapper = remoteApiWrapper ?? throw new ArgumentNullException(nameof(remoteApiWrapper));
    }

    public Wrapper(string installDir)
    {
        InstallDir = installDir;
        _remoteApiWrapper = new RemoteApiWrapper(Path.Combine(InstallDir, DllName));
    }

    public Wrapper()
    {
        _remoteApiWrapper = new RemoteApiWrapper(Path.Combine(InstallDir, DllName));
    }

    public void Dispose()
    {
        ReleaseMacroButtons();

        _remoteApiWrapper.Dispose();
    }

    /// <inheritdoc/>
    public LoginResponse Login() => (LoginResponse)_remoteApiWrapper.Login();
    /// <inheritdoc/>
    public LoginResponse Logout() => (LoginResponse)_remoteApiWrapper.Logout();
    /// <inheritdoc/>
    public RunResponse RunVoicemeeter(int app) => (RunResponse)_remoteApiWrapper.RunVoicemeeter(app);

    /// <inheritdoc/>
    public InfoResponse GetVoicemeeterType(out int type) => (InfoResponse)_remoteApiWrapper.GetVoicemeeterType(out type);
    /// <inheritdoc/>
    public InfoResponse GetVoicemeeterVersion(out int version) => (InfoResponse)_remoteApiWrapper.GetVoicemeeterVersion(out version);

    /// <inheritdoc/>
    public Response IsParametersDirty() => (Response)_remoteApiWrapper.IsParametersDirty();
    /// <inheritdoc/>
    public Response GetParameter(string param, out float value) => (Response)_remoteApiWrapper.GetParameter(param, out value);
    /// <inheritdoc/>
    public Response GetParameter(string param, out string value) => (Response)_remoteApiWrapper.GetParameter(param, out value);

    /// <inheritdoc/>
    public Response MacroButtonIsDirty() => (Response)_remoteApiWrapper.MacroButtonIsDirty();

    /// <inheritdoc/>
    public RunResponse MacroButtonIsRunning()
    {
        if (MacroButtonsHasExited())
        {
            ReleaseMacroButtons();

            if (!MacroButtonsExists())
                return RunResponse.NotInstalled;

            _macroButtons = GetMacroButtons();
        }

        return _macroButtons is null ? RunResponse.NotRunning : RunResponse.Ok;
    }

    private bool MacroButtonsExists() => File.Exists(Path.Combine(InstallDir, MbName + ".exe"));

    private bool MacroButtonsHasExited()
    {
        bool isClosed = true;
        try
        {
            _macroButtons?.Refresh();
            isClosed = _macroButtons?.HasExited ?? true;
        }
        catch { }
        return isClosed;
    }

    private Process? GetMacroButtons()
    {
        var processes = Process.GetProcessesByName(MbName);
        Process? target = null;

        foreach (Process p in processes)
        {
            var isValid = false;
            try
            {
                isValid = p.MainModule?.FileName.StartsWith(InstallDir) ?? false;
            }
            catch { }

            if (isValid && target is null) target = p;
            else p.Dispose();
        }

        return target;
    }

    private void ReleaseMacroButtons()
    {
        _macroButtons?.Dispose();
        _macroButtons = null;
    }
}