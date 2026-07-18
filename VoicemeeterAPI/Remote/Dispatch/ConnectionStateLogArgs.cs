// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using System.Text;

public partial class Remote
{
    private readonly record struct ConnectionStateLogArgs
    {
        public ConnectionState PreviousState { get; }
        public ConnectionState? CurrentState { get; }
        public LoginResponse? CurrentLoginStatus { get; }
        public RunResponse? CurrentButtonsState { get; }
        public Kind? CurrentRunningKind { get; }
        public VmVersion? CurrentRunningVersion { get; }

        public ConnectionStateLogArgs(ConnectionState previousState, ConnectionState currentState)
        {
            this.PreviousState = previousState;
            this.CurrentState = currentState;
        }

        public ConnectionStateLogArgs(ConnectionState previousState, LoginResponse currentLoginStatus)
        {
            this.PreviousState = previousState;
            this.CurrentLoginStatus = currentLoginStatus;
        }

        public ConnectionStateLogArgs(ConnectionState previousState, RunResponse currentButtonsState)
        {
            this.PreviousState = previousState;
            this.CurrentButtonsState = currentButtonsState;
        }

        public ConnectionStateLogArgs(ConnectionState previousState, Kind currentRunningKind)
        {
            this.PreviousState = previousState;
            this.CurrentRunningKind = currentRunningKind;
        }

        public ConnectionStateLogArgs(ConnectionState previousState, VmVersion currentRunningVersion)
        {
            this.PreviousState = previousState;
            this.CurrentRunningVersion = currentRunningVersion;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            return builder
                .AddNullableArg(nameof(this.CurrentState), this.CurrentState)
                .AddNullableArg(nameof(this.CurrentLoginStatus), this.CurrentLoginStatus)
                .AddNullableArg(nameof(this.CurrentButtonsState), this.CurrentButtonsState)
                .AddNullableArg(nameof(this.CurrentRunningKind), this.CurrentRunningKind)
                .AddNullableArg(nameof(this.CurrentRunningVersion), this.CurrentRunningVersion)
                .AddArg(nameof(this.PreviousState), this.PreviousState)
                .ToString();
        }
    }
}
