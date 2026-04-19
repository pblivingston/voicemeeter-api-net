// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI.EventManagement.Exceptions;

public class VmApiException : Exception
{
    public VmApiException(string message, Exception innerException)
        : base(message, innerException)
    { }

    public VmApiException(string message)
        : base(message)
    { }

    public VmApiException()
        : base()
    { }
}

public class VmApiArgumentException : VmApiException
{
    public string? ParamName { get; }

    public VmApiArgumentException(string message, string paramName, Exception innerException)
        : base($"{message}\r\nParameter name: {paramName}", innerException)
    {
        ParamName = paramName;
    }

    public VmApiArgumentException(string message, string paramName)
        : base($"{message}\r\nParameter name: {paramName}")
    {
        ParamName = paramName;
    }

    public VmApiArgumentException(string message, Exception innerException)
        : base(message, innerException)
    { }

    public VmApiArgumentException(string message)
        : base(message)
    { }

    public VmApiArgumentException()
        : base()
    { }
}

public class VmApiArgumentOutOfRangeException : VmApiArgumentException
{
    public object? ActualValue { get; }

    public VmApiArgumentOutOfRangeException(string paramName, object actualValue, string message)
        : base($"'{paramName}' was out of range. {message}.")
    {
        ActualValue = actualValue;
    }

    public VmApiArgumentOutOfRangeException(string paramName, string message)
        : base($"'{paramName}' was out of range. {message}.", paramName)
    { }

    public VmApiArgumentOutOfRangeException(string message, Exception innerException)
        : base(message, innerException)
    { }

    public VmApiArgumentOutOfRangeException(string paramName)
        : base($"'{paramName}' was out of range", paramName)
    { }

    public VmApiArgumentOutOfRangeException()
        : base()
    { }
}

public class TypeNotSupportedException(Type type, string paramName, Type[] supportedTypes)
    : VmApiArgumentException($"Type '{type.Name}' is not supported\r\nSupported types: {Utilities.SupportedTypes.ListString(supportedTypes)}", paramName)
{
    public Type Type { get; } = type;
    public Type[] SupportedTypes { get; } = supportedTypes;
}

public class CannotParseAsTypeException(string actualValue, Type type, string paramName)
    : VmApiArgumentException($"Cannot parse string as '{type.Name}'", paramName)
{
    public string ActualValue { get; } = actualValue;
    public Type Type { get; } = type;
}

public class CannotParseAsPartsException(string actualValue, string paramName)
    : VmApiArgumentException("Cannot parse string as requested version parts", paramName)
{
    public string ActualValue { get; } = actualValue;
}

public class PartsOutOfRangeException : VmApiArgumentException
{
    public int? Major { get; }
    public int? Minor { get; }
    public int? Patch { get; }
    public string? MajorName { get; }
    public string? MinorName { get; }
    public string? PatchName { get; }

    public PartsOutOfRangeException(int major, int minor, int patch, string majorName = "maj", string minorName = "min", string patchName = "pat")
        : base($"""
        Parts must be <= 0xFF and resulting version must be > '0.0.0'
        Major: {major}; Parameter name: {majorName}
        Minor: {minor}; Parameter name: {minorName}
        Patch: {patch}; Parameter name: {patchName}
        """)
    {
        Major = major; Minor = minor; Patch = patch;
        MajorName = majorName; MinorName = minorName; PatchName = patchName;
    }

    public PartsOutOfRangeException(string message, int major, int minor, int patch, string majorName = "maj", string minorName = "min", string patchName = "pat")
        : base(message)
    {
        Major = major; Minor = minor; Patch = patch;
        MajorName = majorName; MinorName = minorName; PatchName = patchName;
    }

    public PartsOutOfRangeException(string message)
        : base(message)
    { }
}

public class PartsOutOfRangeException<T> : PartsOutOfRangeException
    where T : unmanaged
{
    public T Kind { get; }
    public SemVersion? Semantic { get; }
    public string KindName { get; }
    public string? SemanticName { get; }

    public PartsOutOfRangeException(T kind, int major, int minor, int patch, string kindName = "kind", string majorName = "maj", string minorName = "min", string patchName = "pat")
        : base($"""
        Kind must be '{Types.Kind.Standard}'/'{(int)Types.Kind.Standard}', '{Types.Kind.Banana}'/'{(int)Types.Kind.Banana}', '{Types.Kind.Potato}'/'{(int)Types.Kind.Potato}'
        Semantic parts must be <= 0xFF and resulting semantic version must be > '0.0.0'
        Kind: {kind}; Parameter name: {kindName}
        Major: {major}; Parameter name: {majorName}
        Minor: {minor}; Parameter name: {minorName}
        Patch: {patch}; Parameter name: {patchName}
        """, major, minor, patch, majorName, minorName, patchName)
    {
        Kind = kind; KindName = kindName;
    }

    public PartsOutOfRangeException(T kind, SemVersion semantic, string kindName = "kind", string semanticName = "sem")
        : base($"""
        Kind must be '{Types.Kind.Standard}'/'{(int)Types.Kind.Standard}', '{Types.Kind.Banana}'/'{(int)Types.Kind.Banana}', '{Types.Kind.Potato}'/'{(int)Types.Kind.Potato}'
        Semantic version must be > '0.0.0'
        Kind: {kind}; Parameter name: {kindName}
        Semantic: {semantic}; Parameter name: {semanticName}
        """)
    {
        Kind = kind; Semantic = semantic;
        KindName = kindName; SemanticName = semanticName;
    }
}

public class PackedOutOfRangeException(string paramName, int actualValue, string message)
    : VmApiArgumentOutOfRangeException(paramName, actualValue, message)
{ }

public class VmPackedOutOfRangeException(string paramName, int actualValue)
    : PackedOutOfRangeException(paramName, actualValue, "First byte should be <= 0x03 and > 0x00. Remaining three bytes should be > 0x00_0000")
{ }

public class SemPackedOutOfRangeException(string paramName, int actualValue)
    : PackedOutOfRangeException(paramName, actualValue, "First byte should be 0x00. Remaining three bytes should be > 0x00_0000")
{ }