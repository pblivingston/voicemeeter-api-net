// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using AtgDev.Voicemeeter;
using VoicemeeterAPI.Types;
using VoicemeeterAPI.Types.Responses;

namespace VoicemeeterAPI.Exceptions.Remote
{
    /// <summary>
    ///   Exception thrown when a call to <see cref="RemoteApiWrapper.GetVoicemeeterType(out int)"/> fails.
    /// </summary>
    /// <param name="r">API response</param>
    /// <param name="k">Kind returned by the API</param>
    internal sealed class GetVmKindException(GetVmKindResponse r, Kind k)
        : RemoteException($"GetVoicemeeterKind failed - {r}; returned kind: {k}")
    {
        public GetVmKindResponse Response { get; } = r;
        public Kind Kind { get; } = k;
    }
}