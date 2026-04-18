// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI.Utilities;

internal static class SupportedTypes
{
    public static string ListString(Type[] types) => string.Join(", ", types.Select(t => t.Name));

    public static readonly Type[] KindTypes = [typeof(int), typeof(Kind)];
    public static readonly string KindTypes_Str = ListString(KindTypes);
    public static bool IsKindType(Type t)
        => t == typeof(int) || t == typeof(Kind);

    public static readonly Type[] VersionTypes = [typeof(SemVersion), typeof(VmVersion)];
    public static readonly string VersionTypes_Str = ListString(VersionTypes);
    public static bool IsVersionType(Type t)
        => t == typeof(SemVersion) || t == typeof(VmVersion);

    public static readonly Type[] RunTypes = [typeof(int), typeof(App), typeof(Kind), typeof(string)];
    public static readonly string RunTypes_Str = ListString(RunTypes);
    public static bool IsRunType(Type t)
        => t == typeof(int) || t == typeof(App) || t == typeof(Kind) || t == typeof(string);

    public static readonly Type[] ParamTypes = [typeof(float), typeof(int), typeof(bool), typeof(string)];
    public static readonly string ParamTypes_Str = ListString(ParamTypes);
    public static bool IsParamType(Type t)
        => t == typeof(float) || t == typeof(int) || t == typeof(bool) || t == typeof(string);
}