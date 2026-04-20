// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System.Diagnostics;
using System.Runtime.CompilerServices;
using AtgDev.Utils.Native;
using AtgDev.Voicemeeter;
using AtgDev.Voicemeeter.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using PBLivingston.VoicemeeterAPI.EventManagement;
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
    private readonly ILogger<Remote> _logger;
    private readonly Guid _remoteGuid = new();
    private bool _isDisposed = false;
    private ConnectionStateEventArgs _lastState = new(LoginResponse.LoggedOut, Kind.None, default);

    /// <inheritdoc/>
    public LoginResponse LoginStatus { get; private set; } = LoginResponse.LoggedOut;
    /// <inheritdoc/>
    public bool LoggedIn => LoginStatus < LoginResponse.LoggedOut;
    /// <inheritdoc/>
    public bool Connected => LoginStatus == LoginResponse.Ok;

    /// <inheritdoc/>
    public Kind RunningKind { get; private set; } = Kind.None;
    /// <inheritdoc/>
    public VmVersion RunningVersion { get; private set; } = default;

    /// <inheritdoc/>
    public ConnectionStateEventArgs ConnectionState => new(LoginStatus, RunningKind, RunningVersion);

    /// <inheritdoc/>
    public event EventHandler<ConnectionStateEventArgs>? ConnectionStateChanged;

    /// <inheritdoc/>
    public event EventHandler? ParamsDirty;
    /// <inheritdoc/>
    public event EventHandler? ButtonsDirty;

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
    ///   Uses <see cref="PathHelper.GetDllPath()"/> to determine the default path.
    /// </remarks>
    public Remote(ILogger<Remote>? logger = null)
        : this(new Wrapper(new RemoteApiWrapper(PathHelper.GetDllPath())), logger)
    { }

    /// <summary>
    ///   Initializes a new instance of the <see cref="Remote"/> class with a provided <see cref="RemoteApiWrapper"/>.
    /// </summary>
    /// <param name="apiWrapper"></param>
    /// <param name="logger"></param>
    public Remote FromAtgWrapper(RemoteApiWrapper apiWrapper, ILogger<Remote>? logger = null)
        => new(new Wrapper(apiWrapper), logger);

    /// <summary>
    ///   Initializes a new instance of the <see cref="Remote"/> class with a new <see cref="RemoteApiWrapper"/> using the specified DLL path.
    /// </summary>
    /// <param name="dllPath"></param>
    /// <param name="logger"></param>
    public Remote FromDllPath(string dllPath, ILogger<Remote>? logger = null)
        => FromAtgWrapper(new RemoteApiWrapper(dllPath), logger);

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
            RemoteDispatch.Dispose_AlreadyDisposed(_logger);
            return;
        }

        if (LoggedIn)
        {
            RemoteDispatch.Dispose_LoggedIn(_logger, LoginStatus);

            using (BeginMethodScope())
            {
                Logout(timeoutMs: 0, nested: true);
            }
        }

        RemoteDispatch.Dispose_Start(_logger);

        _vmrApi.Dispose();

        RemoteDispatch.Dispose_Success(_logger);
        _isDisposed = true;
    }

    private IDisposable? BeginInstanceScope() => _logger.BeginScope("Instance: {GUID}", _remoteGuid);

    private IDisposable? BeginMethodScope([CallerMemberName] string methodName = "") => _logger.BeginScope("Method: {MethodName}", methodName);

    private void LoginGuard(LoginResponse requiredStatus = LoginResponse.Ok, [CallerMemberName] string methodName = "")
    {
        using var scope = BeginMethodScope(methodName);

        if (_isDisposed)
            RemoteDispatch.Guard_ObjectDisposed(_logger);

        if (LoginStatus > requiredStatus)
            RemoteDispatch.Guard_AccessDenied(_logger, LoginStatus);
    }

    private bool Retry(Func<bool> action, int timeoutMs = 1000, int sleepMs = 100, [CallerMemberName] string methodName = "")
    {
        using var scope = BeginMethodScope(methodName);

        RemoteDispatch.Retry_Start(_logger);

        var attempt = 1;
        var timeout = TimeSpan.FromMilliseconds(timeoutMs);
        var stopwatch = Stopwatch.StartNew();
        do
        {
            if (action())
            {
                RemoteDispatch.Retry_Success(_logger, attempt);
                return true;
            }

            RemoteDispatch.Retry_Attempt(_logger, attempt++);

            Thread.Sleep(sleepMs);
        }
        while (stopwatch.Elapsed < timeout);

        RemoteDispatch.Retry_Timeout(_logger, attempt);
        return false;
    }

    private void OnConnectionStateChanged(ConnectionStateEventArgs currentState, [CallerMemberName] string methodName = "")
    {
        RemoteDispatch.ConnectionStateChanged(_logger, this, ConnectionStateChanged, _lastState, currentState, methodName);
        _lastState = currentState;
    }

    private void OnParamsDirty()
    {
        RemoteDispatch.Dirty_Dirty(_logger, this, ParamsDirty, nameof(IsParamsDirty));
    }

    private void OnButtonsDirty()
    {
        RemoteDispatch.Dirty_Dirty(_logger, this, ButtonsDirty, nameof(IsButtonsDirty));
    }
}
