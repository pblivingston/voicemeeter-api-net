// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;
using VoicemeeterAPI.Types;
using VoicemeeterAPI.Messages;

namespace VoicemeeterAPI
{
    partial class Remote
    {
        #region Is Parameters Dirty

        /// <inheritdoc/>
        public bool ParamsDirty()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(Remote));

            if (LoginStatus is not LoginResponse.Ok)
                throw new RemoteAccessException(nameof(ParamsDirty), LoginStatus);

            var result = (Response)_vmrApi.IsParametersDirty();

            return result switch
            {
                Response.Ok => false,
                Response.Dirty => true,
                _ => throw new RemoteException($"ParamsDirty failed - {result}"),
            };
        }

        #endregion
    }
}