// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

public class AppNotInstalledException(App app)
    : AppStateException("Requested application is not installed.", app)
{ }
