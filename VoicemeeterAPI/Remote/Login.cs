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

        /// <inheritdoc/>
        public void RunVoicemeeter(int kind)
        {
            // Standard -> Standardx64, etc. for 64-bit versions
            kind = kind is <= (int)Kind.Potatox64 ? GeneralUtils.ToBitKind(kind) : kind;

            if (!LoggedIn) throw new RemoteAccessException(nameof(RunVoicemeeter), LoginStatus);

            var result = (RunResponse)_vmrApi.RunVoicemeeter(kind);

            if (result != RunResponse.Ok) throw new RunException(result, (Kind)kind);

            if (kind <= (int)Kind.Potatox64 && !WaitForRunning())
                throw new RunException(RunResponse.Timeout, (Kind)kind);
        }

        /// <inheritdoc/>
        public void RunVoicemeeter(Kind kind) => RunVoicemeeter((int)kind);

        /// <inheritdoc/>
        public void RunVoicemeeter(string kind)
        {
            var k = GeneralUtils.ParseKind(kind);
            RunVoicemeeter(k);
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
                    RemoteInfo.Write($"Voicemeeter {GetVoicemeeterKind()} is running.");
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