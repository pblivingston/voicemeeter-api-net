// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using System.Diagnostics;

public partial class Remote
{
    private partial class Wrapper
    {
        private class ProcessWrapper(ProcessName processName, string installDir) : IDisposable
        {
            private readonly LockObject cacheLock = new();

            public ProcessName ProcessName { get; } = processName;
            public string InstallDir { get; } = installDir;
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

            public RunResponse Close(bool force = false)
            {
                var process = this.GetProcess();
                var state = GetState(process);

                if (state is RunResponse.NotRunning)
                {
                    return state;
                }

                try
                {
                    if (force)
                    {
                        process?.Kill();
                    }
                    else if (!process!.CloseMainWindow())
                    {
                        return RunResponse.Error;
                    }

                    return state;
                }
                catch
                {
                    return RunResponse.Error;
                }
            }

            public async Task WaitForInputIdle(CancellationToken cancellationToken)
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(TimeSpan.FromSeconds(15));

                Process? process;
                RunResponse state;
                var idle = false;
                do
                {
                    await Task.Delay(100, cts.Token);

                    process = this.GetProcess();
                    state = GetState(process);

                    if (state.IsResponding())
                    {
                        try
                        {
                            idle = process!.WaitForInputIdle(0);
                        }
                        catch { }
                    }
                }
                while (!idle);
            }

            public async Task<RunResponse> WaitForExit(CancellationToken cancellationToken)
            {
                var process = this.GetProcess();
                var state = GetState(process);

                if (!state.IsResponding())
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
                    process?.EnableRaisingEvents = true;
                    process?.Exited += OnExited;

                    if (process!.HasExited)
                    {
                        return this.GetState();
                    }

                    using var registration = cts.Token.Register(() => tcs.TrySetCanceled());

                    return await tcs.Task
                        ? this.GetState()
                        : RunResponse.Error;
                }
                finally
                {
                    process?.Exited -= OnExited;
                }
            }

            #region Helpers

            public bool TryAssignDiscovered(Process incomingProcess)
            {
                using var lk = this.cacheLock.EnterScope();

                if (!this.TryClearExited())
                {
                    return false; // already have an active process
                }

                return this.TryAssign(incomingProcess);
            }

            private Process? GetProcess()
            {
                using var lk = this.cacheLock.EnterScope();

                if (this.TryClearExited() && this.ExecutableExists())
                {
                    var processes = Process.GetProcessesByName(this.ProcessName.ToString());

                    foreach (var p in processes)
                    {
                        if (!(this.process is null && this.TryAssign(p)))
                        {
                            p.Dispose();
                        }
                    }
                }

                return this.process;
            }

            private bool ExecutableExists()
                => File.Exists(Path.Combine(this.InstallDir, this.ProcessName + ".exe"));

            /// <summary>
            ///   Must be within cacheLock
            /// </summary>
            /// <returns>
            ///   'true' if clear
            /// </returns>
            private bool TryClearExited()
            {
                if (this.process is not null)
                {
                    if (this.process.SafeHandle is { IsClosed: false, IsInvalid: false })
                    {
                        this.process.Refresh();
                        if (!this.process.HasExited)
                        {
                            return false;
                        }
                    }

                    this.process.Dispose();
                    this.process = null;
                }

                return true;
            }

            /// <summary>
            ///   Must be within cacheLock
            /// </summary>
            /// <param name="process"></param>
            /// <returns>
            ///   'true' if assigned successfully
            /// </returns>
            private bool TryAssign(Process process)
            {
                try
                {
                    var f = process.MainModule?.FileName;
                    if (((App)this.ProcessName is App.MacroButtons && process.MainModule is null)
                        || (f is not null && f.StartsWith(this.InstallDir, StringComparison.OrdinalIgnoreCase)))
                    {
                        this.process = process;
                        return true;
                    }
                }
                catch { }

                return false;
            }

            private static RunResponse GetState(Process? process)
            {
                if (process is null
                    || process.SafeHandle.IsClosed
                    || process.SafeHandle.IsInvalid)
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

            #endregion
        }
    }
}
