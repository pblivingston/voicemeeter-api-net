// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using System.Text;

public partial class Remote
{
    private readonly record struct LogArgs
    {
        private readonly bool args = false;

        public string? Param { get; }
        public object? Value { get; }
        public Kind? Kind { get; }
        public VmVersion? Version { get; }
        public App? App { get; }
        public RunResponse? State { get; }
        public LoginResponse? LoginResponse { get; }
        public ConnectionState? ConnectionState { get; }

        public LogArgs(string param)
        {
            this.Param = param;
            this.args = true;
        }
        public LogArgs(object value)
        {
            this.Value = value;
            this.args = true;
        }
        public LogArgs(string param, object value)
        {
            this.Param = param;
            this.Value = value;
            this.args = true;
        }

        public LogArgs(Kind kind)
        {
            this.Kind = kind;
            this.args = true;
        }
        public LogArgs(VmVersion version)
        {
            this.Version = version;
            this.args = true;
        }
        public LogArgs(VmVersion version, RunResponse state)
        {
            this.Version = version;
            this.State = state;
            this.args = true;
        }

        public LogArgs(App app)
        {
            this.App = app;
            this.args = true;
        }
        public LogArgs(App app, RunResponse state)
        {
            this.App = app;
            this.State = state;
            this.args = true;
        }

        public LogArgs(LoginResponse loginResponse)
        {
            this.LoginResponse = loginResponse;
            this.args = true;
        }

        public LogArgs(ConnectionState connectionState)
        {
            this.ConnectionState = connectionState;
            this.args = true;
        }

        public override string ToString()
        {
            if (!this.args)
            {
                return string.Empty;
            }

            var builder = new StringBuilder();

            return builder
                .Append("{ ")
                .AddNullableArg(nameof(this.Param), this.Param)
                .AddNullableArg(nameof(this.Value), this.Value)
                .AddNullableArg(nameof(this.Kind), this.Kind)
                .AddNullableArg(nameof(this.Version), this.Version)
                .AddNullableArg(nameof(this.App), this.App)
                .AddNullableArg(nameof(this.State), this.State)
                .AddNullableArg(nameof(this.LoginResponse), this.LoginResponse)
                .AddNullableArg(nameof(this.ConnectionState), this.ConnectionState)
                .Append("} ")
                .ToString();
        }
    }
}
