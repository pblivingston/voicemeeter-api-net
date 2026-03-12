// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;
using AtgDev.Utils.Native;
using AtgDev.Voicemeeter;
using AtgDev.Voicemeeter.Utils;
using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI;

/// <summary>
///   Implements the <see cref="IRemote"/> interface to provide methods for interacting with the VoicemeeterRemote API.
/// </summary>
/// <param name="wrapper"><see cref="IWrapper"/></param>
/// <exception cref="ArgumentNullException"></exception>
/// <remarks>
///   The primary constructor initializes a new instance of the <see cref="Remote"/> class with a provided <see cref="IWrapper"/>.
/// </remarks>
/// <example>
///   <code>
///     using var remote = new Remote();
///     try
///     {
///       remote.Login();
///       // Perform operations with the remote API
///     }
///     finally
///     {
///       remote.Logout();
///     }
///   </code>
/// </example>
public sealed partial class Remote(IWrapper wrapper) : IRemote
{
    private readonly IWrapper _vmrApi = wrapper ?? throw new ArgumentNullException(nameof(wrapper));
    private bool _isDisposed = false;

    /// <inheritdoc/>
    public LoginResponse LoginStatus { get; private set; } = LoginResponse.Unknown;
    /// <inheritdoc/>
    public bool LoggedIn => LoginStatus is LoginResponse.Ok or LoginResponse.VoicemeeterNotRunning;

    private Kind NoKind => LoginStatus is LoginResponse.VoicemeeterNotRunning
        ? Kind.None : Kind.Unknown;

    /// <summary>
    ///   Initializes a new instance of the <see cref="Remote"/> class with a provided <see cref="RemoteApiWrapper"/>.
    /// </summary>
    /// <param name="apiWrapper"></param>
    public Remote(RemoteApiWrapper apiWrapper) : this(new Wrapper(apiWrapper))
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="Remote"/> class with a new <see cref="RemoteApiWrapper"/> using the specified DLL path.
    /// </summary>
    /// <param name="dllPath"></param>
    /// <inheritdoc cref="Remote(IWrapper)" path="/example"/>
    public Remote(string dllPath) : this(new RemoteApiWrapper(dllPath))
    {
    }

    /// <summary>
    ///   Initializes a new instance of <see cref="Remote"/> class with a new <see cref="RemoteApiWrapper"/> using the default DLL path.
    /// </summary>
    /// <remarks>
    ///   Uses <see cref="PathHelper.GetDllPath()"/> to determine the default path.
    /// </remarks>
    /// <inheritdoc cref="Remote(IWrapper)" path="/example"/>
    public Remote() : this(new RemoteApiWrapper(PathHelper.GetDllPath()))
    {
    }

    /// <summary>
    ///   Calls <see cref="DllWrapperBase.Dispose()"/> when the <see cref="Remote"/> instance is disposed.
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed) return;

        _vmrApi.Dispose();

        _isDisposed = true;
    }
}
