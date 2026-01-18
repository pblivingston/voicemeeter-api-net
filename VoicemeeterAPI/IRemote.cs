// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;
using AtgDev.Voicemeeter;
using VoicemeeterAPI.Exceptions;
using VoicemeeterAPI.Exceptions.Remote;
using VoicemeeterAPI.Types;
using VoicemeeterAPI.Types.Responses;

namespace VoicemeeterAPI
{
    /// <summary>
    ///   Interface for interacting with the VoicemeeterRemote API via <see cref="RemoteApiWrapper"/>.
    /// </summary>
    /// <remarks>
    ///   <para>Provides methods for logging in and out, as well as checking the login status.</para>
    ///   <para>Implements <see cref="IDisposable"/> to ensure proper cleanup of the underlying API wrapper.</para>
    /// </remarks>
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
        ///   Gets the currently running OS-agnostic Voicemeeter <see cref="Kind"/>.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     Initially set to <see cref="Kind.Unknown"/> until a successful kind retrieval is made.
        ///   </para>
        ///   <para>
        ///     Set to <see cref="Kind.None"/> if Voicemeeter is not running.
        ///   </para>
        /// </remarks>
        Kind RunningKind { get; }

        #region Login

        /// <summary>
        ///   Open communication pipe with VoicemeeterRemote
        /// </summary>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="RemoteAccessException">
        ///   Throws if <see cref="LoginStatus"/> indicates already logged in.
        ///     <list type="bullet">
        ///     <item><description><see cref="LoginResponse.Ok"/></description></item>
        ///     <item><description><see cref="LoginResponse.VoicemeeterNotRunning"/></description></item>
        ///     </list>
        /// </exception>
        /// <exception cref="LoginException">
        ///   Throws if <see cref="LoginResponse"/> is an error.
        ///     <list type="bullet">
        ///     <item><description><see cref="LoginResponse.NoClient"/> (-1)</description></item>
        ///     <item><description><see cref="LoginResponse.AlreadyLoggedIn"/> (-2)</description></item>
        ///     </list>
        /// </exception>
        /// <remarks>
        ///   <para>Updates <see cref="LoginStatus"/> and <see cref="RunningKind"/> on successful login.</para>
        ///   <list type="bullet">
        ///   <item><description>A-tG: <see cref="RemoteApiWrapper.Login()"/></description></item>
        ///   <item><description>C API: long __stdcall VBVMR_Login(void);</description></item>
        ///   </list>
        /// </remarks>
        /// <inheritdoc cref="IRemote" path="/example"/>
        void Login();

        /// <summary>
        ///   Close communication pipe with VoicemeeterRemote
        /// </summary>
        /// <param name="ms">
        ///   Time in milliseconds to wait for the logout process to complete.
        /// </param>
        /// <exception cref="ObjectDisposedException">
        ///   Only throws on disposed instances to allow multiple logout attempts.
        /// </exception>
        /// <remarks>
        ///   <para>
        ///     Updates <see cref="LoginStatus"/> to <see cref="LoginResponse.LoggedOut"/>
        ///     on successful logout.
        ///   </para>
        ///   <para>
        ///     Updates <see cref="LoginStatus"/> to <see cref="LoginResponse.Unknown"/>
        ///     if logout times out.
        ///   </para>
        ///   <para>
        ///     Updates <see cref="RunningKind"/> to <see cref="Kind.Unknown"/>
        ///     on logout or timeout.
        ///   </para>
        ///   <para>
        ///     Does nothing if <see cref="LoginStatus"/> is already <see cref="LoginResponse.LoggedOut"/>.
        ///   </para>
        ///   <list type="bullet">
        ///   <item><description>A-tG: <see cref="RemoteApiWrapper.Logout()"/></description></item>
        ///   <item><description>C API: long __stdcall VBVMR_Logout(void);</description></item>
        ///   </list>
        /// </remarks>
        /// <inheritdoc cref="IRemote" path="/example"/>
        void Logout(int ms = 1000);

        #endregion

        #region General Information

        /// <summary>
        ///   Gets the currently running Voicemeeter kind.
        /// </summary>
        /// <returns>
        ///   Currently running OS-agnostic Voicemeeter <see cref="Kind"/>.
        /// </returns>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="RemoteAccessException">
        ///   Throws if <see cref="LoginStatus"/> indicates not logged in.
        ///     <list type="bullet">
        ///     <item><description><see cref="LoginResponse.LoggedOut"/></description></item>
        ///     <item><description><see cref="LoginResponse.Unknown"/></description></item>   
        ///     </list>
        /// </exception>
        /// <exception cref="GetVmKindException">
        ///   Throws if the API call fails or returns an invalid kind value.
        /// </exception>
        /// <remarks>
        ///   Updates <see cref="RunningKind"/> on successful retrieval.
        /// </remarks>
        Kind GetVoicemeeterKind();

        #endregion
    }
}