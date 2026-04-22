// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using PBLivingston.VoicemeeterAPI.Types;
using PBLivingston.VoicemeeterAPI.EventManagement;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace PBLivingston.VoicemeeterAPI;

partial class Remote
{
    #region MacroButtons Is Running

    internal bool IsMacroButtonsRunning(bool nested)
    {
        LoginGuard(requiredStatus: LoginResponse.VoicemeeterNotRunning);

        _macroButtons?.Refresh();

        if (_macroButtons?.HasExited ?? true)
        {
            ReleaseMacroButtonsHandle();
            _macroButtons = GetMacroButtonsProcess();
        }

        return _macroButtons != null;
    }

    public bool IsMacroButtonsRunning()
    {
        using var scope = BeginInstanceScope();

        return IsMacroButtonsRunning(false);
    }

    #endregion

    #region MacroButtons Is Dirty

    /// <inheritdoc cref="IRemote.IsButtonsDirty()"/>
    internal bool IsButtonsDirty(bool nested)
    {
        LoginGuard();

        var level = nested ? LogLevel.Trace : LogLevel.Information;

        RemoteDispatch.Dirty_Start(_logger, level);

        var result = _vmrApi.MacroButtonIsDirty();

        switch (result)
        {
            case Response.Ok:
                RemoteDispatch.Dirty_Clean(_logger, level);
                return false;

            case Response.Dirty:
                OnButtonsDirty(level);
                return true;

            default:
                throw RemoteDispatch.Method_Error(_logger, result);
        }
    }

    /// <inheritdoc/>
    public bool IsButtonsDirty()
    {
        using var scope = BeginInstanceScope();

        return IsButtonsDirty(false);
    }

    #endregion

    private Process? GetMacroButtonsProcess()
    {
        var processes = Process.GetProcessesByName(MbName);
        Process? target = null;

        foreach (Process p in processes)
        {
            var isValid = false;
            try
            {
                isValid = p.MainModule?.FileName.StartsWith(_installDir) ?? false;
            }
            catch { }

            if (isValid && target is null) target = p;
            else p.Dispose();
        }

        return target;
    }

    private void ReleaseMacroButtonsHandle()
    {
        _macroButtons?.Dispose();
        _macroButtons = null;
    }

    private void OnButtonsDirty(LogLevel level)
    {
        RemoteDispatch.Dirty_Dirty(_logger, level, this, ButtonsDirty, nameof(IsButtonsDirty));
    }
}
