// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Diagnostics;
using System.Threading;
using VoicemeeterAPI.Types.Responses;
using VoicemeeterAPI.Messages;
using VoicemeeterAPI.Exceptions;
using VoicemeeterAPI.Exceptions.Remote;

namespace VoicemeeterAPI
{
    partial class Remote
    {
        #region Login

        /// <inheritdoc/>
        public void Login(int ms = 2000)
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(Remote));

            if (LoginStatus is LoginResponse.Ok or LoginResponse.VoicemeeterNotRunning)
                throw new RemoteAccessException(nameof(Login), LoginStatus);

            VmApiVmrInfo.Write("Logging in...");

            LoginStatus = (LoginResponse)_vmrApi.Login();

            switch (LoginStatus)
            {
                case LoginResponse.Ok:
                    if (!WaitForRunning(ms))
                        throw new LoginException(LoginResponse.Timeout);

                    VmApiVmrInfo.Write("Login successful.");
                    return;

                case LoginResponse.VoicemeeterNotRunning:
                    VmApiVmrWarning.Write("Login successful but Voicemeeter is not running.");
                    return;

                default:
                    throw new LoginException(LoginStatus);
            }
        }

        #endregion

        #region Logout

        /// <inheritdoc/>
        public void Logout(int ms = 1000)
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(Remote));

            if (LoginStatus is LoginResponse.LoggedOut)
            {
                VmApiVmrInfo.Write("Already logged out.");
                return;
            }

            var timeout = TimeSpan.FromMilliseconds(ms);
            var stopwatch = Stopwatch.StartNew();
            int result = -1;

            VmApiVmrInfo.Write("Logging out...");

            while (stopwatch.Elapsed < timeout)
            {
                Thread.Sleep(100);

                result = _vmrApi.Logout();

                if (result == (int)LoginResponse.Ok)
                {
                    VmApiVmrInfo.Write("Logout successful.");
                    LoginStatus = LoginResponse.LoggedOut;
                    return;
                }
            }

            VmApiVmrWarning.Write($"Logout timed out; last result: {result}.");
            LoginStatus = LoginResponse.Unknown;
        }

        #endregion

        #region Helpers

        private bool WaitForRunning(int ms)
        {
            var timeout = TimeSpan.FromMilliseconds(ms);
            var stopwatch = Stopwatch.StartNew();
            Exception? lastException = null;

            VmApiVmrInfo.Write("Checking for running Voicemeeter...");

            while (stopwatch.Elapsed < timeout)
            {
                Thread.Sleep(100);

                try
                {
                    VmApiVmrInfo.Write($"Voicemeeter {GetVoicemeeterKind()} is running.");
                    lastException = null;
                    break;
                }
                catch (RemoteException ex)
                {
                    lastException = ex;
                    VmApiVmrWarning.Write($"Caught exception: \"{lastException.Message}\" - retrying...");
                }
            }

            if (lastException != null)
                return false;

            return true;
        }

        #endregion
    }
}