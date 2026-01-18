// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Diagnostics;
using System.Threading;
using VoicemeeterAPI.Types;
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
        public void Login()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(Remote));

            if (LoginStatus is LoginResponse.Ok or LoginResponse.VoicemeeterNotRunning)
                throw new RemoteAccessException(nameof(Login), LoginStatus);

            VmApiVmrInfo.Write("Logging in...");

            var result = (LoginResponse)_vmrApi.Login();

            switch (result)
            {
                case LoginResponse.Ok:
                    VmApiVmrInfo.Write("Login successful");
                    LoginStatus = result;
                    RunningKind = GetVoicemeeterKind();
                    break;

                case LoginResponse.VoicemeeterNotRunning:
                    VmApiVmrWarning.Write("Login successful but Voicemeeter is not running");
                    LoginStatus = result;
                    RunningKind = Kind.None;
                    break;

                default:
                    throw new LoginException(result);
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
                VmApiVmrInfo.Write("Already logged out");
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
                    VmApiVmrInfo.Write("Logout successful");
                    LoginStatus = LoginResponse.LoggedOut;
                    RunningKind = Kind.Unknown;
                    return;
                }
            }

            VmApiVmrWarning.Write($"Logout timed out; last result: {result}");
            LoginStatus = LoginResponse.Unknown;
            RunningKind = Kind.Unknown;
        }

        #endregion

        #region Helpers

        #endregion
    }
}