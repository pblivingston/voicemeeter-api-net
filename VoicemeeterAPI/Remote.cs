// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using System.Runtime.CompilerServices;
using AtgDev.Utils.Native;
using AtgDev.Voicemeeter;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
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
public sealed partial class Remote : IRemote
{
    private readonly IWrapper vmrApi;
    private readonly ILogger<Remote> logger;
    private readonly Guid instanceId;

    private bool isDisposed;
    private LoginResponse loginStatus = LoginResponse.LoggedOut;

    /// <inheritdoc/>
    public event EventHandler<ConnectionStateEventArgs>? ConnectionStateChanged;
    /// <inheritdoc/>
    public event EventHandler? ParamsDirty;
    /// <inheritdoc/>
    public event EventHandler? ButtonsDirty;

    /// <inheritdoc/>
    public ConnectionState LastConnectionState { get; private set; } = new(LoginResponse.LoggedOut, RunResponse.NotRunning, Kind.None, default);

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
        this.vmrApi = wrapper ?? throw new ArgumentNullException(nameof(wrapper));
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

    #region Connection State

    /// <inheritdoc cref="IRemote.GetConnectionState()"/>
    internal ConnectionState GetConnectionState_i()
    {
        this.LoginGuard(requiredStatus: LoginResponse.Unknown);

        this.On_GetConnectionState_Start();

        if (this.loginStatus >= LoginResponse.LoggedOut)
        {
            return this.LastConnectionState;
        }

        Kind kind;
        VmVersion version;
        RunResponse mbState;
        using (this.BeginMethodScope())
        {
            kind = this.GetInfo_Kind(true);
            version = this.GetInfo_Version(true);
            mbState = this.GetInfo_AppState(App.MacroButtons, true);
        }

        if (kind != version.K)
        {
            throw this.On_GetConnectionState_KindMismatch(kind, version);
        }

        this.On_Method_Success();

        ConnectionState state = new(this.loginStatus, mbState, kind, version);

        this.On_ConnectionState_Changed(state);

        return state;
    }

    /// <inheritdoc/>
    public ConnectionState GetConnectionState()
    {
        using var scope = this.BeginInstanceScope();

        return this.GetConnectionState_i();
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
        using var scope = this.BeginInstanceScope();

        if (this.isDisposed)
        {
            this.On_Dispose_AlreadyDisposed();
            return;
        }

        if (this.LastConnectionState.LoggedIn)
        {
            this.On_Dispose_LoggedIn(this.loginStatus);

            using (this.BeginMethodScope())
            {
                this.Logout_i(true);
            }
        }

        this.On_Dispose_Start();

        this.vmrApi.Dispose();

        this.On_Dispose_Success();
        this.isDisposed = true;
    }

    #endregion

    #region Login Guard

    private void LoginGuard(LoginResponse requiredStatus = LoginResponse.Ok, [CallerMemberName] string methodName = "")
    {
        using var scope = this.BeginMethodScope(methodName);

        if (this.isDisposed)
        {
            throw this.On_Guard_ObjectDisposed();
        }

        if (this.loginStatus > requiredStatus)
        {
            throw this.On_Guard_AccessDenied(this.loginStatus);
        }
    }

    #endregion
}
