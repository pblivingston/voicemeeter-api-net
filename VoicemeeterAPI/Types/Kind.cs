// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

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
        /// <inheritdoc cref="KindUtils.ToApp(Kind)"/>
        public static App ToApp(this Kind kind) => KindUtils.ToApp(kind);
    }

    public static class KindUtils
    {
        /// <summary>
        ///   Converts given Voicemeeter <see cref="Kind"/> to an OS-biased Voicemeeter <see cref="App"/>.
        /// </summary>
        /// <param name="kind"></param>
        /// <returns><see cref="App.Unknown"/> if not a valid Voicemeeter <see cref="Kind"/></returns>
        public static App ToApp(Kind kind)
            => kind is < Kind.None or > Kind.Potato ? App.Unknown
            : ((App)kind).BitAdjust();
    }
}