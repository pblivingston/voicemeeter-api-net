// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;

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

        /// <inheritdoc cref="KindUtils.IsValid{T}(T)"/>
        public static bool IsValid(this Kind kind) => KindUtils.IsValid(kind);
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

        public static bool IsValid<T>(T kind) where T : unmanaged
            => IsKindType<T>()
            && kind is > 0 and <= 3;

        #region Type Validation

        public static bool IsKindType(Type t)
            => t == typeof(int) || t == typeof(Kind);

        public static bool IsKindType<T>() where T : unmanaged
            => IsKindType(typeof(T));

        public static bool TryGetKindType<T>(out Type t) where T : unmanaged
        {
            t = typeof(T);
            if (!IsKindType(t)) return false;
            return true;
        }

        public static Type GetKindType<T>() where T : unmanaged
            => TryGetKindType<T>(out Type t) ? t
            : throw new NotSupportedException($"Type '{t.Name}' is not supported. Use int or Kind.");

        /// <summary>
        ///   Simply throws if the given type is not supported.
        /// </summary>
        /// <param name="t"></param>
        /// <exception cref="NotSupportedException"></exception>
        public static void ValidateKindType(Type t)
        { if (!IsKindType(t)) throw new NotSupportedException($"Type '{t.Name}' is not supported. Use int or Kind."); }

        /// <inheritdoc cref="ValidateKindType(Type)"/>
        public static void ValidateKindType<T>() where T : unmanaged
            => ValidateKindType(typeof(T));

        #endregion
    }
}