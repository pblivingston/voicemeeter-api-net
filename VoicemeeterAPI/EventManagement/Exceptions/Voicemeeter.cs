// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI.EventManagement.Exceptions;

public class VoicemeeterException(string message)
    : VmApiException(message)
{ }