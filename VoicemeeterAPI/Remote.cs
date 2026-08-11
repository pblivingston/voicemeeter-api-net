// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using AtgDev.Utils.Native;
using AtgDev.Voicemeeter;
using AtgDev.Voicemeeter.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

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
    private readonly IWrapper wrapper;
    private readonly ILogger<Remote> logger;
    private readonly Guid instanceId = Guid.NewGuid();
    private readonly SemaphoreSlim stateLock = new(1, 1);
    private readonly LockObject pDirtyLock = new();
    private readonly LockObject bDirtyLock = new();

    private int isDisposed;
    private LoginResponse loginStatus = LoginResponse.LoggedOut;
    private ConnectionState lastConnectionState = new();

    private bool IsDisposed => Volatile.Read(ref this.isDisposed) != 0;

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
        private set
        {
            using var lk = this.stateLock.EnterScope();
            this.loginStatus = value;
        }
    }
    /// <inheritdoc/>
    public bool ConnectedToVoicemeeter => this.LoginStatus == LoginResponse.Ok;
    /// <inheritdoc/>
    public ConnectionState LastConnectionState
    {
        get
        {
            using var lk = this.stateLock.EnterScope();
            return this.lastConnectionState;
        }
        private set
        {
            using var lk = this.stateLock.EnterScope();
            this.lastConnectionState = value;
        }
    }

    #region Construction

    /// <summary>
    ///   Initializes a new instance of the <see cref="Remote"/> class with a provided <see cref="IWrapper"/>.
    /// </summary>
    /// <param name="wrapper"><see cref="IWrapper"/></param>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"></exception>
    internal Remote(IWrapper wrapper, ILogger<Remote>? logger = null)
    {
        this.wrapper = wrapper ?? throw new ArgumentNullException(nameof(wrapper));
        this.logger = logger ?? NullLogger<Remote>.Instance;
    }

    /// <summary>
    ///   Initializes a new instance of <see cref="Remote"/> class with a new <see cref="RemoteApiWrapper"/> using the default DLL path.
    /// </summary>
    /// <param name="logger"></param>
    /// <remarks>
    ///   Uses <see cref="PathHelper.GetProgramFolder()"/> to determine the default path.
    /// </remarks>
    public Remote(ILogger<Remote>? logger = null)
        : this(new Wrapper(), logger)
    { }

    #endregion

    #region Factory

    public static (Remote, ConnectionState) NewSession(ILogger<Remote>? logger = null)
    {
        var e = nameof(NewSession);

        var remote = new Remote(new Wrapper(), logger);

        using var scope = remote.BeginCallScope();

        remote.MethodStart();

        remote.InternalLogin(e);

        (_, var state) = remote.Login_i(e);

        return (remote, state);
    }

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

    #region Disposal

    private void Dispose(bool disposing)
    {
        if (Interlocked.Exchange(ref this.isDisposed, 1) != 0)
        {
            return;
        }

        var e = nameof(this.Dispose);

        using var scope = Log.CallScope(this.logger, this.instanceId, Guid.NewGuid());

        if (disposing)
        {
            try
            {
                using var lk = this.stateLock.EnterScope();

                if (this.loginStatus != LoginResponse.LoggedOut)
                {
                    this.InternalLogout(e);

                    this.Logout_i(e);
                }

                this.WrapperCall(nameof(this.wrapper.Dispose), e);

                this.wrapper.Dispose();
            }
            finally
            {
                this.stateLock.Dispose();
            }
        }
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
