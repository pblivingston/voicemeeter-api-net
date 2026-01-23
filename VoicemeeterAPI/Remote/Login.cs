// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Diagnostics;
using System.Threading;
using VoicemeeterAPI.Types;
using VoicemeeterAPI.Messages;
using VoicemeeterAPI.Utils;

namespace VoicemeeterAPI
{
    partial class Remote
    {
        #region Login

        /// <inheritdoc/>
        public void Login()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(Remote));

            if (LoggedIn) throw new RemoteAccessException(nameof(Login), LoginStatus);

            RemoteInfo.Write("Logging in...");

            LoginStatus = (LoginResponse)_vmrApi.Login();

            switch (LoginStatus)
            {
                case LoginResponse.Ok:
                    if (!WaitForRunning())
                        throw new LoginException(LoginResponse.Timeout);

                    RemoteInfo.Write("Login successful.");
                    return;

                case LoginResponse.VoicemeeterNotRunning:
                    RemoteWarning.Write("Login successful but Voicemeeter is not running.");
                    return;

                default:
                    throw new LoginException(LoginStatus);
            }
        }

        #endregion

        #region Logout

        /// <inheritdoc/>
        public void Logout()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(Remote));

            if (LoginStatus is LoginResponse.LoggedOut)
            {
                RemoteInfo.Write("Already logged out.");
                return;
            }

            var timeout = TimeSpan.FromMilliseconds(1000);
            var stopwatch = Stopwatch.StartNew();
            int result = -1;

            RemoteInfo.Write("Logging out...");

            while (stopwatch.Elapsed < timeout)
            {
                Thread.Sleep(100);

                result = _vmrApi.Logout();

                if (result == (int)LoginResponse.Ok)
                {
                    RemoteInfo.Write("Logout successful.");
                    LoginStatus = LoginResponse.LoggedOut;
                    return;
                }
            }

            RemoteWarning.Write($"Logout timed out; last result: {result}.");
            LoginStatus = LoginResponse.Unknown;
        }

        #endregion

        #region Run Voicemeeter

        /// <inheritdoc cref="IRemote.Run{T}(T)"/>
        public void Run(int app)
        {
            if (!LoggedIn) throw new RemoteAccessException(nameof(Run), LoginStatus);

            app = AppUtils.BitAdjust(app);

            RemoteInfo.Write($"Running app: {(App)app}...");

            var result = (RunResponse)_vmrApi.RunVoicemeeter(app);

            if (result != RunResponse.Ok) throw new RunException(result, (App)app);

            if (app <= (int)App.Potatox64 && !WaitForRunning())
                throw new RunException(RunResponse.Timeout, (App)app);
        }

        /// <inheritdoc cref="IRemote.Run{T}(T)"/>
        public void Run(App app) => Run((int)app);

        /// <inheritdoc cref="IRemote.Run{T}(T)"/>
        public void Run(Kind kind) => Run((int)kind);

        /// <inheritdoc cref="IRemote.Run{T}(T)"/>
        public void Run(string app)
        {
            if (!Enum.TryParse(app, true, out App a))
                throw new ArgumentException($"Invalid app: {app}", nameof(app));

            Run((int)a);
        }

        /// <inheritdoc/>
        void IRemote.Run<T>(T app)
        {
            switch (app)
            {
                case int i:
                    Run(i);
                    break;
                case App a:
                    Run(a);
                    break;
                case Kind k:
                    Run(k);
                    break;
                case string s:
                    Run(s);
                    break;
                default:
                    throw new ArgumentException("Object must be int, App, Kind, or string", nameof(app));
            }
        }

        #endregion

        #region Helpers

        private bool WaitForRunning(int ms = 2000)
        {
            var timeout = TimeSpan.FromMilliseconds(ms);
            var stopwatch = Stopwatch.StartNew();
            Exception? lastException = null;

            RemoteInfo.Write("Waiting for running Voicemeeter...");

            while (stopwatch.Elapsed < timeout)
            {
                Thread.Sleep(100);

                try
                {
                    RemoteInfo.Write($"Voicemeeter {GetKind()} is running.");
                    lastException = null;
                    break;
                }
                catch (RemoteException ex) { lastException = ex; }
            }

            if (lastException != null)
            {
                RemoteWarning.Write($"Timed out waiting for Voicemeeter. Last caught: \"{lastException.Message}\"");
                return false;
            }

            return true;
        }

        #endregion
    }
}