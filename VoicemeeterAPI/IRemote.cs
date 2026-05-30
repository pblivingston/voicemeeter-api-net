// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using AtgDev.Voicemeeter;
using PBLivingston.VoicemeeterAPI.Types;
using PBLivingston.VoicemeeterAPI.Exceptions;

/// <summary>
///   Interface for interacting with the VoicemeeterRemote API via <see cref="RemoteApiWrapper"/>.
/// </summary>
public interface IRemote : IDisposable
{
    /// <summary>
    ///   Reflects the most recently cached connection state.
    /// </summary>
    public ConnectionState LastConnectionState { get; }

    /// <summary>
    ///   Raised when <see cref="LastConnectionState"/> has changed.
    /// </summary>
    public event EventHandler<ConnectionStateEventArgs> ConnectionStateChanged;

    /// <summary>
    ///   Raised when <see cref="IsParamsDirty()"/> returns true.
    /// </summary>
    public event EventHandler ParamsDirty;

    /// <summary>
    ///   Raised when <see cref="IsButtonsDirty()"/> returns true.
    /// </summary>
    public event EventHandler ButtonsDirty;

    /// <summary>
    ///   Updates <see cref="LastConnectionState"/> and returns the current connection state.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ObjectDisposedException"></exception>
    public ConnectionState GetConnectionState();

    #region Login

    /// <summary>
    ///   Opens communication pipe with VoicemeeterRemote.
    /// </summary>
    /// <returns>
    ///   Ok<br/>
    ///   VoicemeeterNotRunning<br/>
    /// </returns>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="AccessDeniedException">
    ///   Throws if login status is unknown.
    /// </exception>
    /// <exception cref="RemoteException{LoginResponse}">
    ///   Throws if already logged in or the API call returns an error.
    /// </exception>
    /// <remarks>
    ///   <para>Calls:</para>
    ///   <list type="bullet">
    ///     <item><description>
    ///       A-tG: <see cref="RemoteApiWrapper.Login()"/>
    ///     </description></item>
    ///     <item><description>
    ///       C API: long __stdcall VBVMR_Login(void);
    ///     </description></item>
    ///   </list>
    /// </remarks>
    /// <inheritdoc cref="IRemote" path="/example"/>
    public LoginResponse Login();

    /// <summary>
    ///   Opens communication pipe with VoicemeeterRemote. If Voicemeeter is running, confirms it is reachable. Waits up to 15 seconds.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <exception cref="RemoteException{LoginResponse}">
    ///   Throws if already logged in, the API call returns an error, or waiting for Voicemeeter is cancelled or times out.
    /// </exception>
    /// <inheritdoc cref="Login()"/>
    public Task<LoginResponse> LoginAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///   Closes communication pipe with VoicemeeterRemote.
    /// </summary>
    /// <returns>
    ///   LoggedOut<br/>
    ///   Unknown<br/>
    /// </returns>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <remarks>
    ///   <para>Calls:</para>
    ///   <list type="bullet">
    ///     <item><description>
    ///       A-tG: <see cref="RemoteApiWrapper.Logout()"/>
    ///     </description></item>
    ///     <item><description>
    ///       C API: long __stdcall VBVMR_Logout(void);
    ///     </description></item>
    ///   </list>
    /// </remarks>
    /// <inheritdoc cref="IRemote" path="/example"/>
    public LoginResponse Logout();

    /// <summary>
    ///   Runs the specified <see cref="App"/>.
    /// </summary>
    /// <typeparam name="T">int, <see cref="App"/>, <see cref="Kind"/>, or string</typeparam>
    /// <param name="app"></param>
    /// <exception cref="TypeNotSupportedException"></exception>
    /// <exception cref="CannotParseAsTypeException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="AccessDeniedException">
    ///   Throws if not logged in.
    /// </exception>
    /// <exception cref="RunException">
    ///   Throws if the <see cref="App"/> is already running but not responding or the API call returns an error.
    /// </exception>
    /// <remarks>
    ///   <para>
    ///     If the app is Voicemeeter (e.g. 3, <see cref="App.Bananax64"/>, <see cref="Kind.Potato"/>, "Standard", etc.),
    ///     automatically adjusts for OS bitness where necessary.
    ///   </para>
    ///   <para>Calls:</para>
    ///   <list type="bullet">
    ///     <item><description>
    ///       A-tG: <see cref="RemoteApiWrapper.RunVoicemeeter(int)"/>
    ///     </description></item>
    ///     <item><description>
    ///       C API: long __stdcall VBVMR_RunVoicemeeter(long vType);
    ///     </description></item>
    ///   </list>
    /// </remarks>
    public void Run<T>(T app) where T : notnull;

    /// <summary>
    ///   Runs the specified <see cref="App"/> and waits for it to start. If the <see cref="App"/> is Voicemeeter, confirms it is reachable. Waits up to 15 seconds.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>
    ///   Ok<br/>
    ///   Hidden<br/>
    /// </returns>
    /// <exception cref="RunException">
    ///   Throws if the <see cref="App"/> is already running but not responding, the API call returns an error, or waiting for <see cref="App"/> is cancelled or times out.
    /// </exception>
    /// <inheritdoc cref="Run{T}(T)"/>
    public Task<RunResponse> RunAsync<T>(T app, CancellationToken cancellationToken = default) where T : notnull;

    #endregion

    #region General Information

    /// <summary>
    ///   Gets the currently running Voicemeeter kind.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="AccessDeniedException">
    ///   Throws if not logged in.
    /// </exception>
    /// <exception cref="GetInfoException">
    ///   Throws if the API call returns an error or an invalid kind value.
    /// </exception>
    /// <remarks>
    ///   <para>Calls:</para>
    ///   <list type="bullet">
    ///     <item><description>
    ///       A-tG: <see cref="RemoteApiWrapper.GetVoicemeeterType(out int)"/>
    ///     </description></item>
    ///     <item><description>
    ///       C API: long __stdcall VBVMR_GetVoicemeeterType(long * pType);
    ///     </description></item>
    ///   </list>
    /// </remarks>
    public Kind GetKind();

    /// <summary>
    ///   Gets the currently running Voicemeeter version.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="AccessDeniedException">
    ///   Throws if not logged in.
    /// </exception>
    /// <exception cref="GetInfoException">
    ///   Throws if the API call returns an error or an invalid version value.
    /// </exception>
    /// <remarks>
    ///   <para>Calls:</para>
    ///   <list type="bullet">
    ///     <item><description>
    ///       A-tG: <see cref="RemoteApiWrapper.GetVoicemeeterVersion(out int)"/>
    ///     </description></item>
    ///     <item><description>
    ///       C API: long __stdcall VBVMR_GetVoicemeeterVersion(long * pVersion);
    ///     </description></item>
    ///   </list>
    /// </remarks>
    public VmVersion GetVersion();

    /// <summary>
    ///   Gets the state of the requested <see cref="App"/>.
    /// </summary>
    /// <param name="app"></param>
    /// <returns>
    ///   Ok<br/>
    ///   Hidden<br/>
    ///   NotRunning<br/>
    ///   NotResponding<br/>
    /// </returns>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="RemoteException{RunResponse}"></exception>
    public RunResponse GetAppState(App app);

    #endregion

    #region Get Parameters

    /// <summary>
    ///   Checks if parameters have changed.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="AccessDeniedException">
    ///   Throws if not logged in or Voicemeeter is not running.
    /// </exception>
    /// <exception cref="RemoteException{Response}">
    ///   Throws if the API call returns an error.
    /// </exception>
    /// <remarks>
    ///   <para>Calls:</para>
    ///   <list type="bullet">
    ///     <item><description>
    ///       A-tG: <see cref="RemoteApiWrapper.IsParametersDirty()"/>
    ///     </description></item>
    ///     <item><description>
    ///       C API: long __stdcall VBVMR_IsParametersDirty(void);
    ///     </description></item>
    ///   </list>
    /// </remarks>
    public bool IsParamsDirty();

    /// <summary>
    ///   Gets the requested Voicemeeter parameter.
    /// </summary>
    /// <typeparam name="T">float, int, bool, or string</typeparam>
    /// <param name="param"></param>
    /// <param name="value"></param>
    /// <exception cref="TypeNotSupportedException">
    ///   Throws if the given value type is not supported.
    /// </exception>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="AccessDeniedException">
    ///   Throws if not logged in or Voicemeeter is not running.
    /// </exception>
    /// <exception cref="GetParamException{T}">
    ///   Throws if the API call returns an error or the requested parameter does not return the requested type.
    /// </exception>
    /// <remarks>
    ///   <para>Calls:</para>
    ///   <list type="bullet">
    ///     <item><description>
    ///       A-tG float: <see cref="RemoteApiWrapper.GetParameter(string, out float)"/>
    ///     </description></item>
    ///     <item><description>
    ///       A-tG string: <see cref="RemoteApiWrapper.GetParameter(string, out string)"/>
    ///     </description></item>
    ///     <item><description>
    ///       C API float: long __stdcall VBVMR_GetParameterFloat(char * szParamName, float * pValue);
    ///     </description></item>
    ///     <item><description>
    ///       C API string: long __stdcall VBVMR_GetParameterStringW(char * szParamName, unsigned short * wszString);
    ///     </description></item>
    ///   </list>
    /// </remarks>
    public void GetParam<T>(string param, out T value) where T : notnull;

    #endregion

    #region Macro Buttons

    /// <summary>
    ///   Checks if any button status has changed.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="AccessDeniedException">
    ///   Throws if not logged in or Voicemeeter is not running.
    /// </exception>
    /// <exception cref="RemoteException{Response}">
    ///   Throws if the API call returns an error.
    /// </exception>
    /// <remarks>
    ///   <para>Calls:</para>
    ///   <list type="bullet">
    ///     <item><description>
    ///       A-tG: <see cref="RemoteApiWrapper.MacroButtonIsDirty()"/>
    ///     </description></item>
    ///     <item><description>
    ///       C API: long __stdcall VBVMR_MacroButton_IsDirty(void);
    ///     </description></item>
    ///   </list>
    /// </remarks>
    public bool IsButtonsDirty();

    #endregion
}
