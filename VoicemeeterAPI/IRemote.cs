// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using AtgDev.Voicemeeter;

/// <summary>
///   Interface for interacting with the VoicemeeterRemote API via <see cref="RemoteApiWrapper"/>.
/// </summary>
public interface IRemote : IDisposable
{
    /// <summary>
    ///   Raised when <see cref="ConnectionState"/> has changed.
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
    ///   Reflects the most recently cached connection state.
    /// </summary>
    public ConnectionState ConnectionState { get; }

    #region Login

    /// <summary>
    ///   Opens communication pipe with VoicemeeterRemote.
    /// </summary>
    /// <returns>
    ///   The current connection state.
    /// </returns>
    /// <remarks>
    ///   <para>Calls:</para>
    ///   <list type="bullet">
    ///     <item><description>
    ///       A-tG's wrapper: <see cref="RemoteApiWrapper.Login()"/>
    ///     </description></item>
    ///     <item><description>
    ///       VoicemeeterRemote: long __stdcall VBVMR_Login(void);
    ///     </description></item>
    ///   </list>
    /// </remarks>
    public ConnectionState Login();

    /// <summary>
    ///   Opens communication pipe with VoicemeeterRemote.<br/>
    ///   If Voicemeeter is running, clears dirty states.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <inheritdoc cref="Login()"/>
    public Task<ConnectionState> LoginAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///   Closes communication pipe with VoicemeeterRemote.
    /// </summary>
    /// <remarks>
    ///   <para>Calls:</para>
    ///   <list type="bullet">
    ///     <item><description>
    ///       A-tG's wrapper: <see cref="RemoteApiWrapper.Logout()"/>
    ///     </description></item>
    ///     <item><description>
    ///       VoicemeeterRemote: long __stdcall VBVMR_Logout(void);
    ///     </description></item>
    ///   </list>
    /// </remarks>
    public void Logout();

    /// <summary>
    ///   Runs the specified application.
    /// </summary>
    /// <param name="app"></param>
    /// <remarks>
    ///   <para>Calls:</para>
    ///   <list type="bullet">
    ///     <item><description>
    ///       A-tG's wrapper: <see cref="RemoteApiWrapper.RunVoicemeeter(int)"/>
    ///     </description></item>
    ///     <item><description>
    ///       VoicemeeterRemote: long __stdcall VBVMR_RunVoicemeeter(long vType);
    ///     </description></item>
    ///   </list>
    /// </remarks>
    public void Run(App app);

    /// <summary>
    ///   Runs the corresponding Voicemeeter application with respect to OS bitness.
    /// </summary>
    /// <param name="kind"></param>
    /// <inheritdoc cref="Run(App)"/>
    public void Run(Kind kind);

    /// <summary>
    ///   Runs the specified application and waits for it to start.
    /// </summary>
    /// <param name="app"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    ///   The application launched or Voicemeeter application detected and its current state.
    /// </returns>
    /// <inheritdoc cref="Run(App)"/>
    public Task<(App App, RunResponse State)> RunAsync(App app, CancellationToken cancellationToken = default);

    /// <summary>
    ///   Runs the corresponding Voicemeeter application with respect to OS bitness and waits for it to start.
    /// </summary>
    /// <param name="kind"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    ///   The Voicemeeter application detected and its current state.
    /// </returns>
    /// <inheritdoc cref="Run(App)"/>
    public Task<(App App, RunResponse State)> RunAsync(Kind kind, CancellationToken cancellationToken = default);

    #endregion

    #region General Information

    /// <summary>
    ///   Determines the current login status.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    ///   <para>Calls:</para>
    ///   <list type="bullet">
    ///     <item><description>
    ///       A-tG's wrapper: <see cref="RemoteApiWrapper.GetVoicemeeterType(out int)"/>
    ///     </description></item>
    ///     <item><description>
    ///       VoicemeeterRemote: long __stdcall VBVMR_GetVoicemeeterType(long * pType);
    ///     </description></item>
    ///   </list>
    /// </remarks>
    public LoginResponse GetLoginStatus();

    /// <summary>
    ///   Gets the currently running Voicemeeter kind.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    ///   <para>Calls:</para>
    ///   <list type="bullet">
    ///     <item><description>
    ///       A-tG's wrapper: <see cref="RemoteApiWrapper.GetVoicemeeterType(out int)"/>
    ///     </description></item>
    ///     <item><description>
    ///       VoicemeeterRemote: long __stdcall VBVMR_GetVoicemeeterType(long * pType);
    ///     </description></item>
    ///   </list>
    /// </remarks>
    public Kind GetKind();

    /// <summary>
    ///   Gets the currently running Voicemeeter version.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    ///   <para>Calls:</para>
    ///   <list type="bullet">
    ///     <item><description>
    ///       A-tG's wrapper: <see cref="RemoteApiWrapper.GetVoicemeeterVersion(out int)"/>
    ///     </description></item>
    ///     <item><description>
    ///       VoicemeeterRemote: long __stdcall VBVMR_GetVoicemeeterVersion(long * pVersion);
    ///     </description></item>
    ///   </list>
    /// </remarks>
    public VmVersion GetVersion();

    /// <summary>
    ///   Gets the state of the requested application.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public RunResponse GetAppState(App app);

    /// <summary>
    ///   Gets the currently running Voicemeeter application and its state.
    /// </summary>
    /// <returns></returns>
    public (App App, RunResponse State) GetVoicemeeterState();

    /// <summary>
    ///   Updates <see cref="ConnectionState"/>.
    /// </summary>
    /// <returns>
    ///   The current connection state.
    /// </returns>
    public ConnectionState RefreshConnectionState();

    #endregion

    #region Get Parameters

    /// <summary>
    ///   Checks if parameters have changed.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    ///   <para>Calls:</para>
    ///   <list type="bullet">
    ///     <item><description>
    ///       A-tG's wrapper: <see cref="RemoteApiWrapper.IsParametersDirty()"/>
    ///     </description></item>
    ///     <item><description>
    ///       VoicemeeterRemote: long __stdcall VBVMR_IsParametersDirty(void);
    ///     </description></item>
    ///   </list>
    /// </remarks>
    public Result<Response, bool> IsParamsDirty();

    /// <summary>
    ///   Gets the requested Voicemeeter parameter.
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    /// <remarks>
    ///   <para>Calls:</para>
    ///   <list type="bullet">
    ///     <item><description>
    ///       A-tG's wrapper: <see cref="RemoteApiWrapper.GetParameter(string, out float)"/>
    ///     </description></item>
    ///     <item><description>
    ///       VoicemeeterRemote: long __stdcall VBVMR_GetParameterFloat(char * szParamName, float * pValue);
    ///     </description></item>
    ///   </list>
    /// </remarks>
    public Result<Response, float> GetParamFloat(string param);

    /// <inheritdoc cref="GetParamFloat(string)"/>
    public Result<Response, int> GetParamInt(string param);

    /// <inheritdoc cref="GetParamFloat(string)"/>
    public Result<Response, bool> GetParamBool(string param);

    /// <remarks>
    ///   <para>Calls:</para>
    ///   <list type="bullet">
    ///     <item><description>
    ///       A-tG's wrapper: <see cref="RemoteApiWrapper.GetParameter(string, out string)"/>
    ///     </description></item>
    ///     <item><description>
    ///       VoicemeeterRemote: long __stdcall VBVMR_GetParameterStringW(char * szParamName, unsigned short * wszString);
    ///     </description></item>
    ///   </list>
    /// </remarks>
    /// <inheritdoc cref="GetParamFloat(string)"/>
    public Result<Response, string> GetParamString(string param);

    #endregion

    #region Macro Buttons

    /// <summary>
    ///   Checks if any button status has changed.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    ///   <para>Calls:</para>
    ///   <list type="bullet">
    ///     <item><description>
    ///       A-tG's wrapper: <see cref="RemoteApiWrapper.MacroButtonIsDirty()"/>
    ///     </description></item>
    ///     <item><description>
    ///       VoicemeeterRemote: long __stdcall VBVMR_MacroButton_IsDirty(void);
    ///     </description></item>
    ///   </list>
    /// </remarks>
    public Result<Response, bool> IsButtonsDirty();

    #endregion
}
