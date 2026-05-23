// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI.Logging;

internal enum Event
{
    // 100-999 General

    BitAdjust = 100, // debug AFTER - include before/after values
    TypeNotSupported, // error + throw
    CannotParseAsType, // error + throw
    ArgumentOutOfRange, // error + throw


    // 1000-1999 Remote

    ConnectionState_Changed = 1000, // info
    ConnectionState_KindMismatch, // critical
    ConnectionState_StateMismatch_LoginStatus, // trace/warning
    ConnectionState_StateMismatch_MacroButtonsIsRunning, // trace/warning
    ConnectionState_StateMismatch_RunningKind, // trace/warning
    ConnectionState_StateMismatch_RunningVersion, // trace/warning

    Dispose_Start = 1010, // debug
    Dispose_LoggedIn, // debug
    Dispose_AlreadyDisposed, // error
    Dispose_Success, // debug

    Guard_ObjectDisposed = 1080, // critical + throw
    Guard_AccessDenied, // error + throw

    Retry_Start = 1090, // debug
    Retry_Attempt, // trace
    Retry_Success, // debug
    Retry_Timeout, // debug


    // 1100-1199 Remote - Login

    Login_Start = 1100, // info
    Login_VmNotRunning, // warning

    Logout_Start = 1110, // info
    Logout_Timeout, // error

    Run_Start = 1120, // info
    Run_Error, // error + throw

    WaitForRunning_Start = 1190, // info
    WaitForRunning_Detected, // info
    WaitForRunning_Timeout, // warning


    // 1200-1299 Remote - General Information

    GetInfo_Start = 1200, // trace/info
    GetInfo_Success_Kind, // trace/info
    GetInfo_Success_Version, // trace/info
    GetInfo_Error, // error + throw


    // 1300-1399 Remote - Get Parameters

    GetParam_Start = 1300, // trace
    GetParam_Success_Float, // trace
    GetParam_Success_Int, // trace
    GetParam_Success_Bool, // trace
    GetParam_Success_String, // trace
    GetParam_Error_Float, // error + throw
    GetParam_Error_String, // error + throw


    // 1400-1499 Remote - Set Parameters

    // 1500-1599 Remote - MIDI

    // 1600-1699 Remote - Device Enumeration

    // 1700-1799 Remote - Audio Callback

    // 1800-1899 Remote - Macro Buttons

    MbRunning_NotRunning = 1800, // trace/warning

    // 1900-1999 Remote - Misc.

    Method_Success = 1900, // info
    Method_Error_Response, // error + throw
    Method_Error_LoginResponse, // error + throw
    Method_Error_RunResponse, // error + throw
    Method_YieldForSettle = 1909, // debug

    Query_Start = 1910, // trace/info
    Query_Success_Response, // trace/info
    Query_Success_RunResponse, // trace/info

    // 2000-2999 Voicemeeter object model logging

    // etc.
}
