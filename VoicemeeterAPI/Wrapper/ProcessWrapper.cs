// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using System.Diagnostics;
using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;

internal partial class Wrapper
{
    public RunResponse GetApplicationState(App app)
        => app.IsValid()
            ? this.apps[app].GetState()
            : RunResponse.UnknownApp;

    public RunResponse ShutdownApplication(App app, bool force = false)
        => app.IsValid()
            ? this.apps[app].Shutdown(force)
            : RunResponse.UnknownApp;

    public Response WaitForApplicationInputIdle(App app)
        => app.IsValid()
            ? this.apps[app].WaitForInputIdle()
            : Response.UnknownApp;

    public Response WaitForApplicationInputIdle(App app, int milliseconds)
        => app.IsValid()
            ? this.apps[app].WaitForInputIdle(milliseconds)
            : Response.UnknownApp;

    public Response WaitForApplicationExit(App app)
        => app.IsValid()
            ? this.apps[app].WaitForExit()
            : Response.UnknownApp;

    public Response WaitForApplicationExit(App app, int milliseconds)
        => app.IsValid()
            ? this.apps[app].WaitForExit(milliseconds)
            : Response.UnknownApp;

    private class ProcessWrapper(App app, string installDir, string processName) : IDisposable
    {
        public App App { get; } = app;
        public string InstallDir { get; } = installDir;
        public string ProcessName { get; } = processName;
        public Process? Process { get; set; }

        public void Dispose()
        {
            this.Process?.Dispose();
            this.Process = null;
        }

        public RunResponse GetState()
        {
            if (!this.ExecutableExists())
            {
                return RunResponse.NotInstalled;
            }

            this.GetProcess();

            if (this.Process is null)
            {
                return RunResponse.NotRunning;
            }

            if (!this.IsResponding() || this.IsMainModuleNull())
            {
                return RunResponse.NotResponding;
            }

            if (this.IsHidden())
            {
                return RunResponse.Hidden;
            }

            return RunResponse.Ok;
        }

        /// <summary>
        ///   Attempts to close the process and releases the process.
        /// </summary>
        /// <param name="force"></param>
        /// <returns>
        ///   Error<br/>
        ///   Last App State<br/>
        /// </returns>
        /// <remarks>
        ///   If app has tray mode enabled, force will be required to shut it down.
        /// </remarks>
        public RunResponse Shutdown(bool force = false)
        {
            var state = this.GetState();

            if (this.Process is not null)
            {
                try
                {
                    if (force)
                    {
                        this.Process.Kill();
                    }
                    else if (state is RunResponse.Ok && !this.Process.CloseMainWindow())
                    {
                        return RunResponse.Error;
                    }
                }
                catch
                {
                    return RunResponse.Error;
                }
                finally
                {
                    this.Dispose();
                }
            }

            return state;
        }

        public Response WaitForInputIdle()
            => this.BoolToResponse(this.WaitForInputIdle_i);

        public Response WaitForInputIdle(int milliseconds)
            => this.BoolToResponse(() => this.WaitForInputIdle_i(milliseconds));

        public Response WaitForExit()
            => this.BoolToResponse(this.WaitForExit_i);

        public Response WaitForExit(int milliseconds)
            => this.BoolToResponse(() => this.WaitForExit_i(milliseconds));

        #region Helpers

        private Process? GetProcess()
        {
            if (this.IsClosed())
            {
                this.Dispose();
            }

            if (this.Process is null && this.ExecutableExists())
            {
                var processes = Process.GetProcessesByName(this.ProcessName);

                foreach (var p in processes)
                {
                    if (this.Process is null)
                    {
                        try
                        {
                            var f = p.MainModule?.FileName;
                            if ((this.App is App.MacroButtons && p.MainModule is null)
                                || (f is not null && f.StartsWith(this.InstallDir, StringComparison.OrdinalIgnoreCase)))
                            {
                                this.Process = p;
                                continue;
                            }
                        }
                        catch { }
                    }

                    p.Dispose();
                }
            }

            return this.Process;
        }

        private bool ExecutableExists()
            => File.Exists(Path.Combine(this.InstallDir, this.ProcessName + ".exe"));

        private bool IsClosed()
        {
            var isClosed = true;
            try
            {
                this.Process?.Refresh();
                isClosed = this.Process?.HasExited ?? true;
            }
            catch { }

            return isClosed;
        }

        private bool IsHidden()
        {
            var isHidden = false;
            try
            {
                this.Process?.Refresh();
                isHidden = (this.Process?.MainWindowHandle ?? -1) == 0;
            }
            catch { }

            return isHidden;
        }

        private bool IsResponding()
        {
            var isResponding = false;
            try
            {
                this.Process?.Refresh();
                isResponding = this.Process?.Responding ?? false;
            }
            catch { }

            return isResponding;
        }

        private bool IsMainModuleNull()
        {
            var isNull = true;
            try
            {
                this.Process?.Refresh();
                isNull = this.Process?.MainModule is null;
            }
            catch { }

            return isNull;
        }

        private Response BoolToResponse(Func<bool> action)
        {
            var state = this.GetState();

            if (this.Process is null)
            {
                return Response.NoServer;
            }

            if (state is RunResponse.NotResponding)
            {
                return Response.Error;
            }

            try
            {
                return action()
                    ? Response.Ok
                    : Response.Dirty;
            }
            catch
            {
                return Response.Error;
            }
        }

        private bool WaitForInputIdle_i()
        {
            if (this.Process is null)
            {
                throw new VmApiException($"{nameof(this.Process)} is null. This exception should be caught.");
            }

            return this.Process.WaitForInputIdle();
        }

        private bool WaitForInputIdle_i(int milliseconds)
        {
            if (this.Process is null)
            {
                throw new VmApiException($"{nameof(this.Process)} is null. This exception should be caught.");
            }

            return this.Process.WaitForInputIdle(milliseconds);
        }

        private bool WaitForExit_i()
        {
            if (this.Process is null)
            {
                throw new VmApiException($"{nameof(this.Process)} is null. This exception should be caught.");
            }

            this.Process.WaitForExit();
            return true;
        }

        private bool WaitForExit_i(int milliseconds)
        {
            if (this.Process is null)
            {
                throw new VmApiException($"{nameof(this.Process)} is null. This exception should be caught.");
            }

            return this.Process.WaitForExit(milliseconds);
        }

        #endregion
    }
}
