// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;
using AtgDev.Voicemeeter;
using AtgDev.Voicemeeter.Utils;
using VoicemeeterAPI.Types.Responses;
using VoicemeeterAPI.Messages;
using VoicemeeterAPI.Types;

namespace VoicemeeterAPI
{
    /// <summary>
    ///   Implements the <see cref="IRemote"/> interface to provide methods for interacting with the VoicemeeterRemote API.
    /// </summary>
    /// <param name="wrapper"><see cref="RemoteApiWrapper"/> instance</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <inheritdoc/>
    public sealed partial class Remote(RemoteApiWrapper wrapper) : IRemote
    {
        private readonly RemoteApiWrapper _vmrApi = wrapper ?? throw new ArgumentNullException(nameof(wrapper));
        private bool _isDisposed = false;

        /// <inheritdoc/>
        public LoginResponse LoginStatus { get; private set; } = LoginResponse.Unknown;

        /// <inheritdoc/>
        public Kind RunningKind { get; private set; } = Kind.Unknown;

        /// <summary>
        ///   Initializes a new instance of the <see cref="Remote"/> class with a new
        ///   <see cref="RemoteApiWrapper"/> using the specified DLL path.
        /// </summary>
        /// <param name="dllPath">Path string to the VoicemeeterRemote DLL</param>
        public Remote(string dllPath) : this(new RemoteApiWrapper(dllPath))
        {
        }

        /// <summary>
        ///   Initializes a new instance of <see cref="Remote"/> class with a new
        ///   <see cref="RemoteApiWrapper"/> using the default DLL path.
        /// </summary>
        /// <remarks>
        ///   Uses <see cref="PathHelper.GetDllPath()"/> to determine the default path.
        /// </remarks>
        public Remote() : this(new RemoteApiWrapper(PathHelper.GetDllPath()))
        {
        }

        /// <summary>
        ///   Calls <see cref="AtgDev.Utils.Native.DllWrapperBase.Dispose()"/> when the <see cref="Remote"/> instance is disposed.
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed) return;

            VmApiVmrInfo.Write("Disposing Remote API wrapper...");

            _vmrApi.Dispose();

            VmApiVmrInfo.Write("Disposed");

            _isDisposed = true;
        }
    }
}