// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using PBLivingston.VoicemeeterAPI.Types;

public class PartsOutOfRangeException(
    int major,
    int minor,
    int patch,
    string majorName = "maj",
    string minorName = "min",
    string patchName = "pat"
    )
    : VmArgumentOutOfRangeException("Semantic parts must be <= 0xFF and resulting semantic version must be > '0.0.0'")
{
    public int Major { get; } = major;
    public int Minor { get; } = minor;
    public int Patch { get; } = patch;
    public string MajorName { get; } = majorName;
    public string MinorName { get; } = minorName;
    public string PatchName { get; } = patchName;

    public override string Message
        => $"""
        {base.Message}
        Major: {this.Major}; Parameter name: {this.MajorName}
        Minor: {this.Minor}; Parameter name: {this.MinorName}
        Patch: {this.Patch}; Parameter name: {this.PatchName}
        """;
}



#region to be deprecated

public class PartsOutOfRangeException<T> : VmArgumentException
    where T : unmanaged
{
    public T Kind { get; }
    public SemVersion? Semantic { get; }
    public string KindName { get; }
    public string? SemanticName { get; }
    public int? Major { get; }
    public int? Minor { get; }
    public int? Patch { get; }
    public string? MajorName { get; }
    public string? MinorName { get; }
    public string? PatchName { get; }

    public PartsOutOfRangeException(
        T kind,
        int major,
        int minor,
        int patch,
        string kindName = "kind",
        string majorName = "maj",
        string minorName = "min",
        string patchName = "pat"
    )
        : base($"""
        Kind must be '{Types.Kind.Standard}'/'{(int)Types.Kind.Standard}', '{Types.Kind.Banana}'/'{(int)Types.Kind.Banana}', '{Types.Kind.Potato}'/'{(int)Types.Kind.Potato}'
        Semantic parts must be <= 0xFF and resulting semantic version must be > '0.0.0'
        Kind: {kind}; Parameter name: {kindName}
        Major: {major}; Parameter name: {majorName}
        Minor: {minor}; Parameter name: {minorName}
        Patch: {patch}; Parameter name: {patchName}
        """)
    {
        this.Kind = kind;
        this.Major = major;
        this.Minor = minor;
        this.Patch = patch;
        this.MajorName = majorName;
        this.MinorName = minorName;
        this.PatchName = patchName;
        this.KindName = kindName;
    }

    public PartsOutOfRangeException(
        T kind,
        SemVersion semantic,
        string kindName = "kind",
        string semanticName = "sem"
    )
        : base($"""
        Kind must be '{Types.Kind.Standard}'/'{(int)Types.Kind.Standard}', '{Types.Kind.Banana}'/'{(int)Types.Kind.Banana}', '{Types.Kind.Potato}'/'{(int)Types.Kind.Potato}'
        Semantic version must be > '0.0.0'
        Kind: {kind}; Parameter name: {kindName}
        Semantic: {semantic}; Parameter name: {semanticName}
        """)
    {
        this.Kind = kind;
        this.Semantic = semantic;
        this.KindName = kindName;
        this.SemanticName = semanticName;
    }
}

#endregion
