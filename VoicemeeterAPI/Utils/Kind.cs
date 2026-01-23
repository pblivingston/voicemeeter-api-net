// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using VoicemeeterAPI.Types;

namespace VoicemeeterAPI.Utils
{
    public static class KindUtils
    {
        /// <summary>
        ///   Converts given Voicemeeter <see cref="Kind"/> to an OS-biased Voicemeeter <see cref="App"/>.
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        public static App ToApp(Kind kind)
            => kind is < Kind.None or > Kind.Potato ? App.Unknown
            : ((App)kind).BitAdjust();
    }
}