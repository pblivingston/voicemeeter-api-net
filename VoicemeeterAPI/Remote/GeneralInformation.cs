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
        public Kind GetKind()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(Remote));

            if (!LoggedIn) throw new RemoteAccessException(nameof(GetKind), LoginStatus);

            var result = (InfoResponse)_vmrApi.GetVoicemeeterType(out int k);
            var kind = (Kind)k;
            if (result != InfoResponse.Ok || kind < Kind.Standard || kind > Kind.Potato)
                throw new GetKindException(result, kind);

            LoginStatus = LoginResponse.Ok;
            return kind;
        }

        #endregion

        #region Get Voicemeeter Version

        /// <inheritdoc/>
        public VmVersion GetVoicemeeterVersion()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(Remote));

            if (!LoggedIn) throw new RemoteAccessException(nameof(GetVoicemeeterVersion), LoginStatus);

            var result = (InfoResponse)_vmrApi.GetVoicemeeterVersion(out int v);
            var version = (VmVersion)v;
            if (result != InfoResponse.Ok || !version.IsValid())
                throw new GetVersionException(result, version);

            LoginStatus = LoginResponse.Ok;
            return version;
        }

        #endregion
    }
}