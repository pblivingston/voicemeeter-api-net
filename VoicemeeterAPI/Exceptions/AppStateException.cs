// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

public class AppStateException(string message, App app)
    : VmApiException(message)
{
    public App App { get; } = app;

    public override string Message
        => base.Message + Environment.NewLine +
            $"App: {this.App}";
}

public class AppNotInstalledException(App app)
    : AppStateException($"Requested application is not installed.", app)
{ }
