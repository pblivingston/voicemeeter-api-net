// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using AtgDev.Utils.Native;
using AtgDev.Voicemeeter;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using PBLivingston.VoicemeeterAPI.Logging;
using PBLivingston.VoicemeeterAPI.Types;
using PBLivingston.VoicemeeterAPI.Utilities;

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
public partial class Remote : IRemote
{
    private readonly IWrapper wrapper;
    private readonly ILogger<Remote> logger;
    private readonly Guid instanceId;
    private readonly SemaphoreSlim stateLock = new(1, 1);
    private readonly LockObject pDirtyLock = new();
    private readonly LockObject bDirtyLock = new();

    private int isDisposed;
    private LoginResponse loginStatus = LoginResponse.LoggedOut;
    private ConnectionState lastConnectionState = new(LoginResponse.LoggedOut, RunResponse.NotRunning, Kind.None, default);

    /// <inheritdoc/>
    public event EventHandler<ConnectionStateEventArgs>? ConnectionStateChanged;
    /// <inheritdoc/>
    public event EventHandler? ParamsDirty;
    /// <inheritdoc/>
    public event EventHandler? ButtonsDirty;

    /// <inheritdoc/>
    public LoginResponse LoginStatus
    {
        get
        {
            using var lk = this.stateLock.EnterScope();
            return this.loginStatus;
        }
    }
    /// <inheritdoc/>
    public bool LoggedIn => this.LoginStatus < LoginResponse.LoggedOut;
    /// <inheritdoc/>
    public bool Connected => this.LoginStatus == LoginResponse.Ok;
    /// <inheritdoc/>
    public ConnectionState LastConnectionState
    {
        get
        {
            using var lk = this.stateLock.EnterScope();
            return this.lastConnectionState;
        }
    }

    #region Construction

    /// <summary>
    ///   Initializes a new instance of the <see cref="Remote"/> class with a provided <see cref="IWrapper"/>.
    /// </summary>
    /// <param name="wrapper"><see cref="IWrapper"/></param>
    /// <param name="logger"></param>
    /// <param name="identifier"></param>
    /// <exception cref="ArgumentNullException"></exception>
    internal Remote(IWrapper wrapper, ILogger<Remote>? logger = null, Guid? identifier = null)
    {
        this.wrapper = wrapper ?? throw new ArgumentNullException(nameof(wrapper));
        this.logger = logger ?? NullLogger<Remote>.Instance;
        this.instanceId = identifier ?? new();
    }

    /// <summary>
    ///   Initializes a new instance of <see cref="Remote"/> class with a new <see cref="RemoteApiWrapper"/> using the default DLL path.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="identifier"></param>
    /// <remarks>
    ///   Uses <see cref="PathHelperExt.GetInstallDirectory()"/> to determine the default path.
    /// </remarks>
    public Remote(ILogger<Remote>? logger = null, Guid? identifier = null)
        : this(new Wrapper(), logger, identifier)
    { }

    #endregion

    #region Factory

    /// <summary>
    ///   Initializes a new instance of the <see cref="Remote"/> class with a provided <see cref="RemoteApiWrapper"/>.
    /// </summary>
    /// <param name="apiWrapper"></param>
    /// <param name="logger"></param>
    /// <param name="identifier"></param>
    public static Remote FromAtgRemoteApiWrapper(RemoteApiWrapper apiWrapper, ILogger<Remote>? logger = null, Guid? identifier = null)
        => new(new Wrapper(apiWrapper), logger, identifier);

    /// <summary>
    ///   Initializes a new instance of the <see cref="Remote"/> class with a new <see cref="RemoteApiWrapper"/> using the specified installation directory.
    /// </summary>
    /// <param name="installDir"></param>
    /// <param name="logger"></param>
    /// <param name="identifier"></param>
    public static Remote FromInstallationDirectory(string installDir, ILogger<Remote>? logger = null, Guid? identifier = null)
        => new(new Wrapper(installDir), logger, identifier);

    #endregion

    #region Disposal

    protected virtual void Dispose(bool disposing)
    {
        if (Interlocked.Exchange(ref this.isDisposed, 1) != 0)
        {
            return;
        }

        using var scope = LogScope.Instance(this.logger, this.instanceId);

        this.On_Dispose_Start();

        if (disposing)
        {
            try
            {
                using var lk = this.stateLock.EnterScope();

                if (this.loginStatus < LoginResponse.LoggedOut)
                {
                    this.On_Dispose_LoggedIn(this.loginStatus);

                    using var methodScope = this.BeginMethodScope();

                    this.Logout_i();
                }

                this.wrapper.Dispose();
            }
            finally
            {
                this.stateLock.Dispose();
            }
        }

        this.On_Dispose_Success();
    }

    /// <summary>
    ///   Calls <see cref="DllWrapperBase.Dispose()"/>.
    /// </summary>
    /// <remarks>
    ///   Calls <see cref="Logout()"/> if still logged in.
    /// </remarks>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
