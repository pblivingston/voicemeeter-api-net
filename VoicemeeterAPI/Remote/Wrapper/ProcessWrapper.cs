// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using System.Diagnostics;
using PBLivingston.VoicemeeterAPI.Types;

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
        ///   Checks if application has entered an idle state.
        /// </summary>
        /// <param name="app"></param>
        /// <returns>
        ///   Ok<br/>
        ///   Dirty<br/>
        ///   NoServer<br/>
        ///   Error<br/>
        ///   UnknownApp<br/>
        /// </returns>
        public Response IsApplicationInputIdle(App app)
            => app.IsValid()
                ? this.apps[app].IsInputIdle()
                : Response.UnknownApp;

        /// <summary>
        ///   Waits for the application to exit.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        ///   Error<br/>
        ///   UnknownApp<br/>
        ///   Last App State<br/>
        /// </returns>
        public async Task<RunResponse> WaitForApplicationExit(App app, CancellationToken cancellationToken = default)
            => app.IsValid()
                ? await this.apps[app].WaitForExit(cancellationToken)
                : RunResponse.UnknownApp;

        private class ProcessWrapper(App app, string installDir, string processName) : IDisposable
        {
            private readonly LockObject cacheLock = new();

            public App App { get; } = app;
            public string InstallDir { get; } = installDir;
            public string ProcessName { get; } = processName;
            private Process? process;

            public void Dispose()
            {
                using var lk = this.cacheLock.EnterScope();

                this.process?.Dispose();
                this.process = null;
            }

            public RunResponse GetState()
            {
                if (!this.ExecutableExists())
                {
                    return RunResponse.NotInstalled;
                }

                return GetState(this.GetProcess());
            }

            public Response IsInputIdle()
                => IsInputIdle(this.GetProcess());

            public RunResponse Close(bool force = false)
            {
                var process = this.GetProcess();
                var state = GetState(process);

                if (process is null || (state is not RunResponse.Ok && !force))
                {
                    return state;
                }

                try
                {
                    if (state is RunResponse.Ok && process.CloseMainWindow())
                    {
                        return state;
                    }

                    if (force)
                    {
                        process.Kill();
                        return state;
                    }

                    return RunResponse.Error;
                }
                catch
                {
                    return RunResponse.Error;
                }
            }

            public async Task<RunResponse> WaitForExit(CancellationToken cancellationToken = default)
            {
                var process = this.GetProcess();
                var state = GetState(process);

                if (process is null || state is RunResponse.NotResponding)
                {
                    return state;
                }

                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(TimeSpan.FromSeconds(15));

                var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

                void OnExited(object? sender, EventArgs e)
                    => tcs.TrySetResult(true);

                try
                {
                    process.EnableRaisingEvents = true;
                    process.Exited += OnExited;

                    if (process.HasExited)
                    {
                        return GetState(process);
                    }

                    using var registration = cts.Token.Register(() => tcs.TrySetCanceled());

                    return await tcs.Task
                        ? GetState(process)
                        : RunResponse.Error;
                }
                catch (OperationCanceledException)
                {
                    return GetState(process);
                }
                finally
                {
                    process.Exited -= OnExited;
                }
            }

            #region Helpers

            private bool ExecutableExists()
                => File.Exists(Path.Combine(this.InstallDir, this.ProcessName + ".exe"));

            private Process? GetProcess()
            {
                using var lk = this.cacheLock.EnterScope();

                try
                {
                    this.process?.Refresh();
                    if (this.process?.HasExited ?? true)
                    {
                        this.process?.Dispose();
                        this.process = null;
                    }
                }
                catch { }

                if (this.process is null && this.ExecutableExists())
                {
                    var processes = Process.GetProcessesByName(this.ProcessName);

                    foreach (var p in processes)
                    {
                        if (this.process is null)
                        {
                            try
                            {
                                var f = p.MainModule?.FileName;
                                if ((this.App is App.MacroButtons && p.MainModule is null)
                                    || (f is not null && f.StartsWith(this.InstallDir, StringComparison.OrdinalIgnoreCase)))
                                {
                                    this.process = p;
                                    continue;
                                }
                            }
                            catch { }
                        }

                        p.Dispose();
                    }
                }

                return this.process;
            }

            private static RunResponse GetState(Process? process)
            {
                if (process is null)
                {
                    return RunResponse.NotRunning;
                }

                process.Refresh();

                if (!process.Responding || process.MainModule is null)
                {
                    return RunResponse.NotResponding;
                }

                if (process.MainWindowHandle == IntPtr.Zero)
                {
                    return RunResponse.Hidden;
                }

                return RunResponse.Ok;
            }

            private static Response IsInputIdle(Process? process)
            {
                var state = GetState(process);

                if (process is null)
                {
                    return Response.NoServer;
                }

                if (state is RunResponse.NotResponding)
                {
                    return Response.Error;
                }

                try
                {
                    return process.WaitForInputIdle(0)
                        ? Response.Ok
                        : Response.Dirty;
                }
                catch
                {
                    return Response.Error;
                }
            }

            #endregion
        }
    }
}
