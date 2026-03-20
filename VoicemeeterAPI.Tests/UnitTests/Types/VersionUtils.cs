using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types;

public class VersionUtilsTests
{
    public enum Case
    {
        Standard, Banana, Potato,
        MinimumValid, MaximumValid,
        ZeroedBytes, MaxedBytes,
        NegativeKind, NegativeSems,
        KindSpill, BadStrings
    }

    [Flags]
    public enum CaseTag
    {
        None = 0,
        PartsIn = 1 << 0,
        Invalid_Packed = 1 << 1,
        Invalid_Kind = 1 << 2,
        Invalid_Sems = 1 << 3,
        Invalid_String = 1 << 4
    }

    public record CaseRecord(
        int Packed,
        int Kind,
        int Major,
        int Minor,
        int Patch,
        Kind K,
        int SemPacked,
        string VmString,
        string SemString,
        CaseTag Tags = CaseTag.None);

#pragma warning disable xUnit1047
    public static TheoryDataRow<Case, CaseRecord>[] GetCaseData =>
    [
        new(Case.Standard,     new(0x0102_0304, 1, 2, 3, 4, Kind.Standard, 0x0002_0304, "1.2.3.4", "2.3.4")),
        new(Case.Banana,       new(0x0203_0405, 2, 3, 4, 5, Kind.Banana, 0x0003_0405, "2.3.4.5", "3.4.5")),
        new(Case.Potato,       new(0x0304_0506, 3, 4, 5, 6, Kind.Potato, 0x0004_0506, "3.4.5.6", "4.5.6")),
        new(Case.MinimumValid, new(0x0100_0000, 1, 0, 0, 0, Kind.Standard, 0x0000_0000, "1.0.0.0", "0.0.0")),
        new(Case.MaximumValid, new(0x03FF_FFFF, 3, 255, 255, 255, Kind.Potato, 0x00FF_FFFF, "3.255.255.255", "255.255.255")),
        new(Case.ZeroedBytes,  new(0x0000_0000, 0, 0, 0, 0, Kind.None, 0x0000_0000, "0.0.0.0", "0.0.0",
            CaseTag.Invalid_Packed | CaseTag.Invalid_Kind)),
        new(Case.MaxedBytes,   new(unchecked((int)0xFFFF_FFFF), 255, 255, 255, 255, Kind.Unknown, unchecked((int)0xFFFF_FFFF), "255.255.255.255", "255.255.255",
            CaseTag.Invalid_Packed | CaseTag.Invalid_Kind)),
        new(Case.NegativeKind, new(unchecked((int)0xFF00_0000), -1, 0, 0, 0, Kind.Unknown, unchecked((int)0xFF00_0000), "255.0.0.0", "0.0.0",
            CaseTag.Invalid_Packed | CaseTag.Invalid_Kind | CaseTag.PartsIn)),
        new(Case.NegativeSems, new(unchecked((int)0xFFFF_FFFF), 2, -1, -1, -1, Kind.Banana, unchecked((int)0xFFFF_FFFF), "255.255.255.255", "255.255.255",
            CaseTag.Invalid_Packed | CaseTag.Invalid_Sems | CaseTag.PartsIn)),
        new(Case.KindSpill,    new(0x0301_0101, 2, 257, 1, 1, Kind.Banana, 0x0001_0101, "3.1.1.1", "1.1.1",
            CaseTag.Invalid_Sems | CaseTag.PartsIn)),
        new(Case.BadStrings,   new(0x0103_0507, 1, 3, 5, 7, Kind.Standard, 0x0003_0507, "1.3.5.7.9", "Invalid",
            CaseTag.Invalid_String))
    ];
#pragma warning restore xUnit1047

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void IsValid_ReturnsExpected_Bool(Case scenario, CaseRecord data)
    {
        _ = scenario;

        var expected = !data.Tags.HasAny(CaseTag.Invalid_Sems);

        Assert.Equal(expected, VersionUtils.IsValid(data.Major, data.Minor, data.Patch));
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void RawPack_ReturnsExpected_Int(Case scenario, CaseRecord data)
    {
        _ = scenario;

        Assert.Equal(data.Packed, VersionUtils.RawPack(data.Kind, data.Major, data.Minor, data.Patch));
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void RawUnpack_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        Assert.SkipWhen(data.Tags.HasAny(CaseTag.PartsIn), $"Skipping case: {scenario} with any tags: PartsIn");

        VersionUtils.RawUnpack(data.Packed, out int k, out int m, out int n, out int p);

        Assert.Multiple(
            () => Assert.Equal(data.Kind, k),
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void TryParse_Parts_ReturnsExpected_Parts_Vm(Case scenario, CaseRecord data)
    {
        Assert.SkipWhen(data.Tags.HasAny(CaseTag.PartsIn), $"Skipping case: {scenario} with any tags: PartsIn");

        var badTags = CaseTag.Invalid_String | CaseTag.Invalid_Packed;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var kind = 0; var maj = 0; var min = 0; var pat = 0;
        if (shouldSucceed) { kind = data.Kind; maj = data.Major; min = data.Minor; pat = data.Patch; }

        var success = VersionUtils.TryParse(data.VmString, out int k, out int m, out int n, out int p);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(kind, k),
            () => Assert.Equal(maj, m),
            () => Assert.Equal(min, n),
            () => Assert.Equal(pat, p)
        );
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void TryParse_Parts_ReturnsExpected_Parts_Sem(Case scenario, CaseRecord data)
    {
        Assert.SkipWhen(data.Tags.HasAny(CaseTag.PartsIn), $"Skipping case: {scenario} with any tags: PartsIn");

        var badTags = CaseTag.Invalid_String;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var maj = 0; var min = 0; var pat = 0;
        if (shouldSucceed) { maj = data.Major; min = data.Minor; pat = data.Patch; }

        var success = VersionUtils.TryParse(data.SemString, out int k, out int m, out int n, out int p);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(0, k),
            () => Assert.Equal(maj, m),
            () => Assert.Equal(min, n),
            () => Assert.Equal(pat, p)
        );
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void Parse_Parts_ReturnsExpected_Parts_Vm(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.PartsIn | CaseTag.Invalid_String | CaseTag.Invalid_Packed;
        Assert.SkipWhen(data.Tags.HasAny(skipTags),
            $"Skipping case: {scenario} with any tags: PartsIn, Invalid_String, Invalid_Packed");

        VersionUtils.Parse(data.VmString, out int k, out int m, out int n, out int p);

        Assert.Multiple(
            () => Assert.Equal(data.Kind, k),
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void Parse_Parts_ThrowsException_Argument_Vm(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_String | CaseTag.Invalid_Packed;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: Invalid_String, Invalid_Packed");
        Assert.SkipWhen(data.Tags.HasAny(CaseTag.PartsIn), $"Skipping case: {scenario} with any tags: PartsIn");

        Assert.Throws<ArgumentException>(() => VersionUtils.Parse(data.VmString, out int _, out int _, out int _, out int _));
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void Parse_Parts_ReturnsExpected_Parts_Sem(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.PartsIn | CaseTag.Invalid_String;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: PartsIn, Invalid_String");

        VersionUtils.Parse(data.SemString, out int k, out int m, out int n, out int p);

        Assert.Multiple(
            () => Assert.Equal(0, k),
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void Parse_Parts_ThrowsException_Argument_Sem(Case scenario, CaseRecord data)
    {
        Assert.SkipUnless(data.Tags.HasAny(CaseTag.Invalid_String), $"Skipping case: {scenario} without any tags: Invalid_String");
        Assert.SkipWhen(data.Tags.HasAny(CaseTag.PartsIn), $"Skipping case: {scenario} with any tags: PartsIn");

        Assert.Throws<ArgumentException>(() => VersionUtils.Parse(data.SemString, out int _, out int _, out int _, out int _));
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void TryParse_Packed_ReturnsExpected_Packed_Vm(Case scenario, CaseRecord data)
    {
        _ = scenario;

        var badTags = CaseTag.Invalid_String | CaseTag.Invalid_Packed;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var expected = shouldSucceed ? data.Packed : 0x0000_0000;

        var success = VersionUtils.TryParse(data.VmString, out int result);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(expected, result)
        );
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void TryParse_Packed_ReturnsExpected_Packed_Sem(Case scenario, CaseRecord data)
    {
        Assert.SkipWhen(data.Tags.HasAny(CaseTag.Invalid_Packed), $"Skipping case: {scenario} with any tags: Invalid_Packed");

        var badTags = CaseTag.Invalid_String;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var expected = shouldSucceed ? data.SemPacked : 0x0000_0000;

        var success = VersionUtils.TryParse(data.SemString, out int result);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(expected, result)
        );
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void Parse_Packed_ReturnsExpected_Packed_Vm(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_String | CaseTag.Invalid_Packed;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: Invalid_String, Invalid_Packed");

        Assert.Equal(data.Packed, VersionUtils.Parse(data.VmString));
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void Parse_Packed_ThrowsException_Argument_Vm(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_String | CaseTag.Invalid_Packed;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: Invalid_String, Invalid_Packed");

        Assert.Throws<ArgumentException>(() => VersionUtils.Parse(data.VmString));
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void Parse_Packed_ReturnsExpected_Packed_Sem(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_String | CaseTag.Invalid_Packed;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: Invalid_String, Invalid_Packed");

        Assert.Equal(data.SemPacked, VersionUtils.Parse(data.SemString));
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void Parse_Packed_ThrowsException_Argument_Sem(Case scenario, CaseRecord data)
    {
        Assert.SkipUnless(data.Tags.HasAny(CaseTag.Invalid_String), $"Skipping case: {scenario} without any tags: Invalid_String");
        Assert.SkipWhen(data.Tags.HasAny(CaseTag.Invalid_Packed), $"Skipping case: {scenario} with any tags: Invalid_Packed");

        Assert.Throws<ArgumentException>(() => VersionUtils.Parse(data.SemString));
    }
}