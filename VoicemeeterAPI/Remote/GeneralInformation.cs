// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;
using VoicemeeterAPI.Types;
using VoicemeeterAPI.Messages;

namespace VoicemeeterAPI
{
    partial class Remote
    {
        #region Get Voicemeeter Kind

        /// <inheritdoc/>
        public Kind GetVoicemeeterKind()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(Remote));

            if (LoginStatus is not LoginResponse.Ok and not LoginResponse.VoicemeeterNotRunning)
                throw new RemoteAccessException(nameof(GetVoicemeeterKind), LoginStatus);

            var result = (InfoResponse)_vmrApi.GetVoicemeeterType(out int k);
            var kind = (Kind)k;
            if (result != InfoResponse.Ok || kind < Kind.Standard || kind > Kind.Potato)
                throw new GetVmKindException(result, kind);

            LoginStatus = LoginResponse.Ok;
            return kind;
        }

        #endregion
    }
}