// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System.Diagnostics;
using System.Runtime.CompilerServices;
using AtgDev.Utils.Native;
using AtgDev.Voicemeeter;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using PBLivingston.VoicemeeterAPI.Types;
using PBLivingston.VoicemeeterAPI.Utilities;

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
    private readonly ILogger<Remote> _logger;
    private readonly Guid _remoteGuid = new();

    private bool _isDisposed = false;
    private LoginResponse _loginStatus = LoginResponse.LoggedOut;
    private ConnectionStateEventArgs _lastState = new(LoginResponse.LoggedOut, false, Kind.None, default);

    /// <inheritdoc/>
    public event EventHandler<ConnectionStateEventArgs>? ConnectionStateChanged;
    /// <inheritdoc/>
    public event EventHandler? ParamsDirty;
    /// <inheritdoc/>
    public event EventHandler? ButtonsDirty;

    #region Construction

    /// <summary>
    ///   Initializes a new instance of the <see cref="Remote"/> class with a provided <see cref="IWrapper"/>.
    /// </summary>
    /// <param name="wrapper"><see cref="IWrapper"/></param>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"></exception>
    internal Remote(IWrapper wrapper, ILogger<Remote>? logger = null)
    {
        _vmrApi = wrapper ?? throw new ArgumentNullException(nameof(wrapper));
        _logger = logger ?? NullLogger<Remote>.Instance;
    }

    /// <summary>
    ///   Initializes a new instance of <see cref="Remote"/> class with a new <see cref="RemoteApiWrapper"/> using the default DLL path.
    /// </summary>
    /// <param name="logger"></param>
    /// <remarks>
    ///   Uses <see cref="PathHelperExt.GetInstallDirectory()"/> to determine the default path.
    /// </remarks>
    public Remote(ILogger<Remote>? logger = null)
        : this(new Wrapper(), logger)
    { }

    #endregion

    #region Factory

    /// <summary>
    ///   Initializes a new instance of the <see cref="Remote"/> class with a provided <see cref="RemoteApiWrapper"/>.
    /// </summary>
    /// <param name="apiWrapper"></param>
    /// <param name="logger"></param>
    public static Remote FromAtgRemoteApiWrapper(RemoteApiWrapper apiWrapper, ILogger<Remote>? logger = null)
        => new(new Wrapper(apiWrapper), logger);

    /// <summary>
    ///   Initializes a new instance of the <see cref="Remote"/> class with a new <see cref="RemoteApiWrapper"/> using the specified installation directory.
    /// </summary>
    /// <param name="installDir"></param>
    /// <param name="logger"></param>
    public static Remote FromInstallationDirectory(string installDir, ILogger<Remote>? logger = null)
        => new(new Wrapper(installDir), logger);

    #endregion

    #region Connection State

    /// <inheritdoc/>
    public ConnectionStateEventArgs GetConnectionState()
    {
        if (!_lastState.LoggedIn)
            return _lastState;

        var kind = GetInfo_Kind(true);
        var version = GetInfo_Version(true);
        bool mbRunning = Query_ButtonsRunning(true);

        if (kind != version.K)
            throw On_ConnectionState_KindMismatch(kind, version);

        ConnectionStateEventArgs state = new(_loginStatus, mbRunning, kind, version);

        if (state != _lastState)
            On_ConnectionState_Changed(state);

        return state;
    }

    #endregion

    #region Disposal

    /// <summary>
    ///   Calls <see cref="DllWrapperBase.Dispose()"/>.
    /// </summary>
    /// <remarks>
    ///   Calls <see cref="Logout(int, int)"/> if still logged in.
    /// </remarks>
    public void Dispose()
    {
        using var scope = BeginInstanceScope();

        if (_isDisposed)
        {
            On_Dispose_AlreadyDisposed();
            return;
        }

        if (_lastState.LoggedIn)
        {
            On_Dispose_LoggedIn(_loginStatus);

            using (BeginMethodScope())
                Logout(timeoutMs: 0, nested: true);
        }

        On_Dispose_Start();

        _vmrApi.Dispose();

        On_Dispose_Success();
        _isDisposed = true;
    }

    #endregion

    #region Helpers

    private IDisposable? BeginInstanceScope() => _logger.BeginScope("Instance: {GUID}", _remoteGuid);

    private IDisposable? BeginMethodScope([CallerMemberName] string methodName = "") => _logger.BeginScope("Method: {MethodName}", methodName);

    private void LoginGuard(LoginResponse requiredStatus = LoginResponse.Ok, [CallerMemberName] string methodName = "")
    {
        using var scope = BeginMethodScope(methodName);

        if (_isDisposed)
            throw On_Guard_ObjectDisposed();

        if (_loginStatus > requiredStatus)
            throw On_Guard_AccessDenied(_loginStatus);
    }

    #endregion

    #region Retry

    private bool Retry(Func<bool> action, int timeoutMs = 1000, int sleepMs = 100, [CallerMemberName] string methodName = "")
    {
        using var scope = BeginMethodScope(methodName);

        On_Retry_Start();

        var attempt = 1;
        var timeout = TimeSpan.FromMilliseconds(timeoutMs);
        var stopwatch = Stopwatch.StartNew();
        do
        {
            if (action())
            {
                On_Retry_Success(attempt);
                return true;
            }

            On_Retry_Attempt(attempt++);

            Thread.Sleep(sleepMs);
        }
        while (stopwatch.Elapsed < timeout);

        On_Retry_Timeout(attempt);
        return false;
    }

    private (T LastResult, bool Success) Retry<T>(Func<(T Result, bool Success)> action, int timeoutMs = 1000, int sleepMs = 100, [CallerMemberName] string methodName = "")
    {
        using var scope = BeginMethodScope(methodName);

        On_Retry_Start();

        (T, bool Success) res;
        var attempt = 1;
        var timeout = TimeSpan.FromMilliseconds(timeoutMs);
        var stopwatch = Stopwatch.StartNew();
        do
        {
            res = action();

            if (res.Success)
            {
                On_Retry_Success(attempt);
                return res;
            }

            On_Retry_Attempt(attempt++);

            Thread.Sleep(sleepMs);
        }
        while (stopwatch.Elapsed < timeout);

        On_Retry_Timeout(attempt);
        return res;
    }

    #endregion
}
