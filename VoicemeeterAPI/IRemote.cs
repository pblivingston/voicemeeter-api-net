// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;
using AtgDev.Voicemeeter;
using VoicemeeterAPI.Types;
using VoicemeeterAPI.Messages;

namespace VoicemeeterAPI
{
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

        /// <summary>
        ///   Gets the currently running Voicemeeter <see cref="Kind"/>.
        /// </summary>
        /// <remarks>
        ///   <list type="bullet">
        ///     <item><description>
        ///       Calls <see cref="GetKind()"/> if <see cref="LoginStatus"/> is <see cref="LoginResponse.Ok"/>.
        ///     </description></item>
        ///     <item><description>
        ///       <see cref="Kind.None"/> if <see cref="LoginStatus"/> is <see cref="LoginResponse.VoicemeeterNotRunning"/>.
        ///     </description></item>
        ///     <item><description>
        ///       Otherwise, <see cref="Kind.Unknown"/>.
        ///     </description></item>
        ///   </list>
        /// </remarks>
        Kind RunningKind { get; }

        /// <summary>
        ///   Gets the currently running Voicemeeter <see cref="VmVersion"/>.
        /// </summary>
        /// <remarks>
        ///   <list type="bullet">
        ///     <item><description>
        ///       Calls <see cref="GetVoicemeeterVersion()"/> if <see cref="LoginStatus"/> is <see cref="LoginResponse.Ok"/>.
        ///     </description></item>
        ///     <item><description>
        ///       Otherwise, a <see cref="VmVersion"/> with <see cref="RunningKind"/> and version 0.0.0.
        ///     </description></item>
        ///   </list>
        /// </remarks>
        VmVersion RunningVersion { get; }

        #region Login

        /// <summary>
        ///   Open communication pipe with VoicemeeterRemote
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
        ///   Close communication pipe with VoicemeeterRemote
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
        /// <exception cref="GetKindException">
        ///   Throws if the API call fails or returns an invalid kind value.
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
        ///   Gets the currently running Voicemeeter version.
        /// </summary>
        /// <returns><see cref="VmVersion"></returns>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="RemoteAccessException">
        ///   Throws if not <see cref="LoggedIn"/>.
        /// </exception>
        /// <exception cref="GetVersionException">
        ///   Throws if the API call fails or returns an invalid version value.
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
        VmVersion GetVoicemeeterVersion();

        #endregion
    }
}