// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using VoicemeeterAPI.Utils;

namespace VoicemeeterAPI.Types
{
    /// <summary>
    ///   Voicemeeter kinds returned by <see cref="Remote.GetKind()"/>.
    /// </summary>
    public enum Kind
    {
        Unknown = -1,
        None = 0,
        Standard = 1,
        Banana = 2,
        Potato = 3
    }

    public static class KindExt
    {
        public static App ToApp(this Kind kind) => KindUtils.ToApp(kind);
    }
}