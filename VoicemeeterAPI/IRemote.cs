// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;
using AtgDev.Voicemeeter;
using VoicemeeterAPI.Types;
using VoicemeeterAPI.Messages;

namespace VoicemeeterAPI;

/// <summary>
///   Interface for interacting with the VoicemeeterRemote API via <see cref="RemoteApiWrapper"/>.
/// </summary>
public interface IRemote : IDisposable
{
    /// <summary>
    ///   Gets the current <see cref="LoginResponse"/> login status of the <see cref="IRemote"/> instance.
    /// </summary>
    /// <remarks>
    ///   Initially set to <see cref="LoginResponse.Unknown"/> until a successful login attempt is made.
    /// </remarks>
    LoginResponse LoginStatus { get; }

    /// <summary>
    ///   Simplifies <see cref="LoginStatus"/> checks.
    /// </summary>
    /// <remarks>
    ///   `true` if <see cref="LoginStatus"/> is <see cref="LoginResponse.Ok"/> or <see cref="LoginResponse.VoicemeeterNotRunning"/>, otherwise `false`.
    /// </remarks>
    bool LoggedIn { get; }

    #region Login

    /// <summary>
    ///   Opens communication pipe with VoicemeeterRemote.
    /// </summary>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="RemoteAccessException">
    ///   Throws if <see cref="LoggedIn"/>.
    /// </exception>
    /// <exception cref="LoginException">
    ///   <para>Throws if the API call returns an error or the process times out.</para>
    /// </exception>
    /// <remarks>
    ///   <para>Updates <see cref="LoginStatus"/> on successful login.</para>
    ///   <para>If API call returns <see cref="LoginResponse.Ok"/>, waits for the running Voicemeeter instance to be detected.</para>
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
    void Login();

    /// <summary>
    ///   Closes communication pipe with VoicemeeterRemote.
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    ///   Only throws on disposed instances to allow multiple logout attempts.
    /// </exception>
    /// <remarks>
    ///   <para>Updates <see cref="LoginStatus"/> to <see cref="LoginResponse.LoggedOut"/> on successful logout.</para>
    ///   <para>Updates <see cref="LoginStatus"/> to <see cref="LoginResponse.Unknown"/> if logout times out.</para>
    ///   <para>Does nothing if <see cref="LoginStatus"/> is already <see cref="LoginResponse.LoggedOut"/>.</para>
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
    void Logout();

    /// <summary>
    ///   Runs the specified <see cref="App"/>.
    /// </summary>
    /// <typeparam name="T">int, <see cref="App"/>, <see cref="Kind"/>, or string</typeparam>
    /// <param name="app"></param>
    /// <exception cref="ArgumentException">
    ///   Throws if the given type is not supported or parsing a given string fails.
    /// </exception>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="RemoteAccessException">
    ///   Throws if not <see cref="LoggedIn"/>.
    /// </exception>
    /// <exception cref="RunException">
    ///   Throws if the API call returns an error or the process times out.
    /// </exception>
    /// <remarks>
    ///   <para>
    ///     If the app is a Voicemeeter app (e.g. 3, <see cref="App.Bananax64"/>, <see cref="Kind.Potato"/>, "Standard", etc.),
    ///     automatically adjusts for OS bitness where necessary and waits for the process to start.
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
    void Run<T>(T app) where T : notnull;

    #endregion

    #region General Information

    /// <summary>
    ///   Gets the currently running Voicemeeter kind.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="RemoteAccessException">
    ///   Throws if not <see cref="LoggedIn"/>.
    /// </exception>
    /// <exception cref="RemoteException">
    ///   Throws if the API call returns an error or an invalid kind value.
    /// </exception>
    /// <remarks>
    ///   <para>Updates <see cref="LoginStatus"/> to <see cref="LoginResponse.Ok"/> on successful call.</para>
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
    Kind GetKind();

    /// <summary>
    ///   Attempts to get the running Voicemeeter kind.
    /// </summary>
    /// <param name="kind">
    ///   If <see cref="GetKind()"/> throws
    ///   <list type="bullet">
    ///     <item><description>
    ///       <see cref="Kind.None"/> when <see cref="LoginStatus"/> is <see cref="LoginResponse.VoicemeeterNotRunning"/>
    ///     </description></item>
    ///     <item><description>
    ///       <see cref="Kind.Unknown"/> otherwise
    ///     </description></item>
    ///   </list>
    /// </param>
    /// <returns>
    ///   False if <see cref="GetKind()"/> throws a <see cref="RemoteException"/>
    /// </returns>
    bool TryGetKind(out Kind kind);

    /// <summary>
    ///   Gets the currently running Voicemeeter version.
    /// </summary>
    /// <returns><see cref="VmVersion"/></returns>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="RemoteAccessException">
    ///   Throws if not <see cref="LoggedIn"/>.
    /// </exception>
    /// <exception cref="RemoteException">
    ///   Throws if the API call returns an error or an invalid version value.
    /// </exception>
    /// <remarks>
    ///   <para>Updates <see cref="LoginStatus"/> to <see cref="LoginResponse.Ok"/> on successful call.</para>
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
    VmVersion GetVersion();

    /// <summary>
    ///   Attempts to get the running Voicemeeter version.
    /// </summary>
    /// <param name="vm">
    ///   If <see cref="GetVersion()"/> throws
    ///   <list type="bullet">
    ///     <item><description>
    ///       "0.0.0.0" when <see cref="LoginStatus"/> is <see cref="LoginResponse.VoicemeeterNotRunning"/>
    ///     </description></item>
    ///     <item><description>
    ///       "255.0.0.0" otherwise
    ///     </description></item>
    ///   </list>
    /// </param>
    /// <returns>False if <see cref="GetVersion()"/> throws a <see cref="RemoteException"/></returns>
    bool TryGetVersion(out VmVersion vm);

    #endregion

    #region Get Parameters

    /// <summary>
    ///   Checks if parameters have changed.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="RemoteAccessException">
    ///   Throws if <see cref="LoginStatus"/> is not <see cref="LoginResponse.Ok"/>.
    /// </exception>
    /// <exception cref="RemoteException">
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
    bool ParamsDirty();

    /// <summary>
    ///   Gets the requested Voicemeeter parameter.
    /// </summary>
    /// <param name="param"></param>
    /// <param name="value"></param>
    /// <exception cref="NotSupportedException">
    ///   Throws if the given value type is not supported.
    /// </exception>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="RemoteAccessException">
    ///   Throws if <see cref="LoginStatus"/> is not <see cref="LoginResponse.Ok"/>.
    /// </exception>
    /// <exception cref="RemoteException">
    ///   Throws if the API call returns an error.
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
    void GetParam<T>(string param, out T value);

    #endregion

    #region Macro Buttons

    /// <summary>
    ///   Checks if any button status has changed.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="RemoteAccessException">
    ///   Throws if <see cref="LoginStatus"/> is not <see cref="LoginResponse.Ok"/>.
    /// </exception>
    /// <exception cref="RemoteException">
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
    bool ButtonsDirty();

    #endregion
}
