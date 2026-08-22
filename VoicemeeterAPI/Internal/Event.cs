// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI.Internal;

internal enum Event
{
    // 1xxx REMOTE

    // 11xx Start/Entry
    RemoteMethodStart = 1100,
    RemoteWrapperCall = 1101,

    WaitForRunningStart = 1120,
    YieldForEngineSettle = 1121,

    ConnectionStateChanged = 1130,

    // 12xx Success/Exit
    RemoteMethodSuccess = 1200,

    WaitForRunningDetected = 1220,

    // 13xx Warning
    RemoteNotConnected = 1300,
    RemoteLostConnection = 1301,

    StaleConnectionState = 1330,

    RemoteOperationTimeout = 1350,

    // 14xx Dev Error
    RemoteContractViolation = 1400, // NotLoggedInException, AlreadyLoggedInException, etc.

    RemoteInvalidArgument = 1410, // VmApiArgumentException, VmApiArgumentOutOfRangeException, CannotParseAsTypeException, etc.

    RemoteMethodError = 1499,

    // 15xx Env Error
    RemoteLoginFailed = 1500,

    AppCriticalState = 1510,

    UnhandledResponse = 1599,

    // 19xx Teardown/Cleanup

    // 2xxx VOICEMEETER

    // etc.
}
