// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using System.Diagnostics;
using PBLivingston.VoicemeeterAPI.Types;

internal partial class Wrapper
{
    /// <inheritdoc cref="ProcessWrapper.GetState()"/>
    /// <param name="app"></param>
    public RunResponse GetApplicationState(App app)
        => app.IsValid()
            ? this.apps[app].GetState()
            : RunResponse.UnknownApp;

    /// <inheritdoc cref="ProcessWrapper.Shutdown(bool)"/>
    /// <param name="app"></param>
    public RunResponse ShutdownApplication(App app, bool force = false)
        => app.IsValid()
            ? this.apps[app].Shutdown(force)
            : RunResponse.UnknownApp;

    /// <inheritdoc cref="ProcessWrapper.IsInputIdle()"/>
    /// <param name="app"></param>
    public Response IsApplicationInputIdle(App app)
        => app.IsValid()
            ? this.apps[app].IsInputIdle()
            : Response.UnknownApp;

    /// <inheritdoc cref="ProcessWrapper.WaitForExit(CancellationToken)"/>
    /// <param name="app"></param>
    public async Task<RunResponse> WaitForApplicationExit(App app, CancellationToken cancellationToken = default)
        => app.IsValid()
            ? await this.apps[app].WaitForExit(cancellationToken)
            : RunResponse.UnknownApp;

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

        /// <summary>
        ///   Gets the current state of the application.
        /// </summary>
        /// <returns>
        ///   Ok<br/>
        ///   Hidden<br/>
        ///   NotRunning<br/>
        ///   NotResponding<br/>
        ///   NotInstalled<br/>
        /// </returns>
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
            }

            return this.GetState();
        }

        /// <summary>
        ///   Checks if application has entered an idle state.
        /// </summary>
        /// <returns>
        ///   Ok<br/>
        ///   Dirty<br/>
        ///   NoServer<br/>
        ///   Error<br/>
        /// </returns>
        public Response IsInputIdle()
        {
            var state = this.GetState();

            if (state is RunResponse.NotInstalled or RunResponse.NotResponding)
            {
                return Response.Error;
            }

            if (this.Process is null)
            {
                return Response.NoServer;
            }

            try
            {
                return this.Process.WaitForInputIdle(0)
                    ? Response.Ok
                    : Response.Dirty;
            }
            catch
            {
                return Response.Error;
            }
        }

        /// <summary>
        ///   Waits for the application to exit.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>
        ///   Error<br/>
        ///   Last App State<br/>
        /// </returns>
        public async Task<RunResponse> WaitForExit(CancellationToken cancellationToken = default)
        {
            var state = this.GetState();

            if (this.Process is null || state is RunResponse.NotResponding)
            {
                return state;
            }

            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            void OnExited(object? sender, EventArgs e)
                => tcs.TrySetResult(true);

            try
            {
                this.Process.EnableRaisingEvents = true;
                this.Process.Exited += OnExited;

                state = this.GetState();

                if (state is not (RunResponse.Ok or RunResponse.Hidden))
                {
                    return state;
                }

                using (cancellationToken.Register(() => tcs.TrySetCanceled()))
                {
                    return await tcs.Task
                        ? this.GetState()
                        : RunResponse.Error;
                }
            }
            catch (OperationCanceledException)
            {
                return this.GetState();
            }
            finally
            {
                this.Process?.Exited -= OnExited;
            }
        }

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

        #endregion
    }
}
