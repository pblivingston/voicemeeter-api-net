using System.Runtime.CompilerServices;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests;

internal static class CaseTagExt
{
    public static bool HasAny<T>(this T tags, T mask) where T : struct, Enum
        => (Unsafe.As<T, int>(ref tags) & Unsafe.As<T, int>(ref mask)) != 0;
}