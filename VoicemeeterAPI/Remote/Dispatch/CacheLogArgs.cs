// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using System.Text;
using Microsoft.Extensions.Logging;

public partial class Remote
{
    private sealed class CacheLogArgs
    {
        public static CacheLogArgs Empty { get; } = new();

        public ConnectionState? PreviousState { get; }
        public ConnectionState? CurrentState { init; get; }
        public LoginResponse? CurrentLoginStatus { init; get; }
        public RunResponse? CurrentVoicemeeterState { init; get; }
        public Kind? CurrentVoicemeeterKind { init; get; }
        public App? CurrentVoicemeeterApp { init; get; }
        public VmVersion? CurrentVoicemeeterVersion { init; get; }
        public RunResponse? CurrentMacroButtonsState { init; get; }

        private CacheLogArgs() { }

        private CacheLogArgs(ConnectionState previousState)
            => this.PreviousState = previousState;

        public static CacheLogArgs New(
            ILogger logger,
            LogLevel level,
            ConnectionState previousState,
            ConnectionState currentState
        ) => logger.IsEnabled(level)
        ? new(previousState)
        {
            CurrentState = currentState,
        }
        : Empty;

        public static CacheLogArgs New(
            ILogger logger,
            LogLevel level,
            ConnectionState previousState,
            LoginResponse? currentLoginStatus = null,
            RunResponse? currentVoicemeeterState = null,
            Kind? currentVoicemeeterKind = null,
            App? currentVoicemeeterApp = null,
            VmVersion? currentVoicemeeterVersion = null,
            RunResponse? currentMacroButtonsState = null
        ) => logger.IsEnabled(level)
        ? new(previousState)
        {
            CurrentLoginStatus = currentLoginStatus,
            CurrentVoicemeeterState = currentVoicemeeterState,
            CurrentVoicemeeterKind = currentVoicemeeterKind,
            CurrentVoicemeeterApp = currentVoicemeeterApp,
            CurrentVoicemeeterVersion = currentVoicemeeterVersion,
            CurrentMacroButtonsState = currentMacroButtonsState
        }
        : Empty;

        public override string ToString()
        {
            if (this.PreviousState is null)
            {
                return string.Empty;
            }

            var builder = new StringBuilder();

            return builder
                .Append("{ ")
                .AddNullableArg(nameof(this.CurrentState), this.CurrentState)
                .AddNullableArg(nameof(this.CurrentLoginStatus), this.CurrentLoginStatus)
                .AddNullableArg(nameof(this.CurrentVoicemeeterState), this.CurrentVoicemeeterState)
                .AddNullableArg(nameof(this.CurrentVoicemeeterKind), this.CurrentVoicemeeterKind)
                .AddNullableArg(nameof(this.CurrentVoicemeeterApp), this.CurrentVoicemeeterApp)
                .AddNullableArg(nameof(this.CurrentVoicemeeterVersion), this.CurrentVoicemeeterVersion)
                .AddNullableArg(nameof(this.CurrentMacroButtonsState), this.CurrentMacroButtonsState)
                .AddNullableArg(nameof(this.PreviousState), this.PreviousState)
                .Append("} ")
                .ToString();
        }
    }
}
