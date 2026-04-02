// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using AtgDev.Utils.Native;
using AtgDev.Voicemeeter;
using AtgDev.Voicemeeter.Utils;
using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI;

/// <summary>
///   Implements the <see cref="IRemote"/> interface to provide methods for interacting with the VoicemeeterRemote API.
/// </summary>
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
public sealed partial class Remote : IRemote
{
    private readonly IWrapper _vmrApi;
    private bool _isDisposed = false;

    /// <inheritdoc/>
    public LoginResponse LoginStatus { get; private set; } = LoginResponse.LoggedOut;
    /// <inheritdoc/>
    public bool LoggedIn => LoginStatus is LoginResponse.Ok or LoginResponse.VoicemeeterNotRunning;

    private Kind NoKind => LoginStatus is LoginResponse.VoicemeeterNotRunning
        ? Kind.None : Kind.Unknown;

    /// <summary>
    ///   Initializes a new instance of the <see cref="Remote"/> class with a provided <see cref="IWrapper"/>.
    /// </summary>
    /// <param name="wrapper"><see cref="IWrapper"/></param>
    /// <exception cref="ArgumentNullException"></exception>
    internal Remote(IWrapper wrapper)
    {
        _vmrApi = wrapper ?? throw new ArgumentNullException(nameof(wrapper));
    }

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
    public Remote(string dllPath) : this(new RemoteApiWrapper(dllPath))
    {
    }

    /// <summary>
    ///   Initializes a new instance of <see cref="Remote"/> class with a new <see cref="RemoteApiWrapper"/> using the default DLL path.
    /// </summary>
    /// <remarks>
    ///   Uses <see cref="PathHelper.GetDllPath()"/> to determine the default path.
    /// </remarks>
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

    private void LoginGuard(LoginResponse requiredStatus = LoginResponse.Ok, [CallerMemberName] string methodName = "")
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(Remote), $"Called method: {methodName}");

        if (LoginStatus > requiredStatus)
            throw new RemoteAccessException(methodName, LoginStatus);
    }

    private bool Retry(Func<bool> action, int timeoutMs = 1000, int sleepMs = 100)
    {
        var timeout = TimeSpan.FromMilliseconds(timeoutMs);
        var stopwatch = Stopwatch.StartNew();
        while (stopwatch.Elapsed < timeout)
        {
            if (action()) return true;
            Thread.Sleep(sleepMs);
        }
        return false;
    }
}
