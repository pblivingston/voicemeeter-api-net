// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;
using VoicemeeterAPI.Types;
using VoicemeeterAPI.Types.Responses;
using VoicemeeterAPI.Exceptions;
using VoicemeeterAPI.Exceptions.Remote;

namespace VoicemeeterAPI
{
    partial class Remote
    {
        #region Get Voicemeeter Kind

        /// <inheritdoc/>
        public Kind GetVoicemeeterKind()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(Remote));

            if (LoginStatus is LoginResponse.VoicemeeterNotRunning)
                return Kind.None;

            if (LoginStatus is not LoginResponse.Ok)
                throw new RemoteAccessException(nameof(GetVoicemeeterKind), LoginStatus);

            int result = _vmrApi.GetVoicemeeterType(out int kind);
            if (result != (int)GetVmKindResponse.Ok || kind < (int)Kind.None || kind > (int)Kind.Potato)
                throw new GetVmKindException((GetVmKindResponse)result, kind);

            RunningKind = (Kind)kind;
            return RunningKind;
        }

        #endregion
    }
}