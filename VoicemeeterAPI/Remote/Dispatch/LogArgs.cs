// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using System.Text;
using Microsoft.Extensions.Logging;

public partial class Remote
{
    private sealed class LogArgs(bool isSentinel)
    {
        public static LogArgs Empty { get; } = new(true);

        private readonly bool args = !isSentinel;

        public string? Param { init; get; }
        public object? Value { init; get; }
        public Kind? Kind { init; get; }
        public VmVersion? Version { init; get; }
        public App? App { init; get; }
        public RunResponse? State { init; get; }
        public LoginResponse? LoginResponse { init; get; }
        public ConnectionState? ConnectionState { init; get; }

        public static LogArgs New(
            ILogger logger,
            LogLevel level,
            string? param = null,
            object? value = null,
            Kind? kind = null,
            VmVersion? version = null,
            App? app = null,
            RunResponse? state = null,
            LoginResponse? loginResponse = null,
            ConnectionState? connectionState = null
        )
        {
            if (!logger.IsEnabled(level)
                || (
                    param is null
                    && value is null
                    && kind is null
                    && version is null
                    && app is null
                    && state is null
                    && loginResponse is null
                    && connectionState is null
                )
            )
            {
                return Empty;
            }

            return new(false)
            {
                Param = param,
                Value = value,
                Kind = kind,
                Version = version,
                App = app,
                State = state,
                LoginResponse = loginResponse,
                ConnectionState = connectionState
            };
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
