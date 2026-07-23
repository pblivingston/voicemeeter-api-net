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
        public RunResponse? CurrentButtonsState { init; get; }
        public Kind? CurrentRunningKind { init; get; }
        public VmVersion? CurrentRunningVersion { init; get; }

        private CacheLogArgs() { }

        private CacheLogArgs(ConnectionState previousState)
            => this.PreviousState = previousState;

        public static CacheLogArgs New(
            ILogger logger,
            LogLevel level,
            ConnectionState previousState,
            ConnectionState? currentState = null,
            LoginResponse? currentLoginStatus = null,
            RunResponse? currentButtonsState = null,
            Kind? currentRunningKind = null,
            VmVersion? currentRunningVersion = null
        ) => logger.IsEnabled(level)
        ? new(previousState)
        {
            CurrentState = currentState,
            CurrentLoginStatus = currentLoginStatus,
            CurrentButtonsState = currentButtonsState,
            CurrentRunningKind = currentRunningKind,
            CurrentRunningVersion = currentRunningVersion
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
                .AddNullableArg(nameof(this.CurrentButtonsState), this.CurrentButtonsState)
                .AddNullableArg(nameof(this.CurrentRunningKind), this.CurrentRunningKind)
                .AddNullableArg(nameof(this.CurrentRunningVersion), this.CurrentRunningVersion)
                .AddNullableArg(nameof(this.PreviousState), this.PreviousState)
                .Append("} ")
                .ToString();
        }
    }
}
