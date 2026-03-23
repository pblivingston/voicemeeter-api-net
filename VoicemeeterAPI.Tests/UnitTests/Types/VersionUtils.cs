using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VersionData;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types;

public class VersionUtilsTests
{
    [Theory]
    [ClassData(typeof(VersionData))]
    public void IsValid_ReturnsExpected_Bool(Case scenario, CaseRecord data)
    {
        _ = scenario;

        var expected = !data.Tags.HasAny(CaseTag.Invalid_Sems);

        Assert.Equal(expected, VersionUtils.IsValid(data.Major, data.Minor, data.Patch));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void RawPack_ReturnsExpected_Int(Case scenario, CaseRecord data)
    {
        _ = scenario;

        Assert.Equal(data.Packed, VersionUtils.RawPack(data.Kind, data.Major, data.Minor, data.Patch));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
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
    [ClassData(typeof(VersionData))]
    public void TryParse_Parts_ReturnsExpected_Parts_Vm(Case scenario, CaseRecord data)
    {
        Assert.SkipWhen(data.Tags.HasAny(CaseTag.PartsIn), $"Skipping case: {scenario} with any tags: PartsIn");

        var shouldSucceed = !data.Tags.HasAny(CaseTag.Invalid_String);

        var kind = 0; var maj = 0; var min = 0; var pat = 0;
        if (shouldSucceed) { kind = data.Kind; maj = data.Major; min = data.Minor; pat = data.Patch; }

        var success = VersionUtils.TryParse(data.String, out int k, out int m, out int n, out int p);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(kind, k),
            () => Assert.Equal(maj, m),
            () => Assert.Equal(min, n),
            () => Assert.Equal(pat, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryParse_Parts_ReturnsExpected_Parts_Sem(Case scenario, CaseRecord data)
    {
        Assert.SkipWhen(data.Tags.HasAny(CaseTag.SemsIn), $"Skipping case: {scenario} with any tags: SemsIn");

        var shouldSucceed = !data.Tags.HasAny(CaseTag.Invalid_SemString);

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
    [ClassData(typeof(VersionData))]
    public void Parse_Parts_ReturnsExpected_Parts_Vm(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.PartsIn | CaseTag.Invalid_String;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: PartsIn, Invalid_String");

        VersionUtils.Parse(data.String, out int k, out int m, out int n, out int p);

        Assert.Multiple(
            () => Assert.Equal(data.Kind, k),
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Parse_Parts_ThrowsException_Argument_Vm(Case scenario, CaseRecord data)
    {
        Assert.SkipUnless(data.Tags.HasAny(CaseTag.Invalid_String), $"Skipping case: {scenario} without any tags: Invalid_String");

        Assert.Throws<ArgumentException>(() => VersionUtils.Parse(data.String, out int _, out int _, out int _, out int _));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Parse_Parts_ReturnsExpected_Parts_Sem(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.SemsIn | CaseTag.Invalid_SemString;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: SemsIn, Invalid_SemString");

        VersionUtils.Parse(data.SemString, out int k, out int m, out int n, out int p);

        Assert.Multiple(
            () => Assert.Equal(0, k),
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Parse_Parts_ThrowsException_Argument_Sem(Case scenario, CaseRecord data)
    {
        Assert.SkipUnless(data.Tags.HasAny(CaseTag.Invalid_SemString), $"Skipping case: {scenario} without any tags: Invalid_SemString");

        Assert.Throws<ArgumentException>(() => VersionUtils.Parse(data.SemString, out int _, out int _, out int _, out int _));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryParse_Packed_ReturnsExpected_Packed_Vm(Case scenario, CaseRecord data)
    {
        _ = scenario;

        var shouldSucceed = !data.Tags.HasAny(CaseTag.Invalid_String);

        var expected = shouldSucceed ? data.Packed : 0x0000_0000;

        var success = VersionUtils.TryParse(data.String, out int result);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(expected, result)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryParse_Packed_ReturnsExpected_Packed_Sem(Case scenario, CaseRecord data)
    {
        _ = scenario;

        var shouldSucceed = !data.Tags.HasAny(CaseTag.Invalid_SemString);

        var expected = shouldSucceed ? data.SemPacked : 0x0000_0000;

        var success = VersionUtils.TryParse(data.SemString, out int result);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(expected, result)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Parse_Packed_ReturnsExpected_Packed_Vm(Case scenario, CaseRecord data)
    {
        Assert.SkipWhen(data.Tags.HasAny(CaseTag.Invalid_String), $"Skipping case: {scenario} with any tags: Invalid_String");

        Assert.Equal(data.Packed, VersionUtils.Parse(data.String));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Parse_Packed_ThrowsException_Argument_Vm(Case scenario, CaseRecord data)
    {
        Assert.SkipUnless(data.Tags.HasAny(CaseTag.Invalid_String), $"Skipping case: {scenario} without any tags: Invalid_String");

        Assert.Throws<ArgumentException>(() => VersionUtils.Parse(data.String));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Parse_Packed_ReturnsExpected_Packed_Sem(Case scenario, CaseRecord data)
    {
        Assert.SkipWhen(data.Tags.HasAny(CaseTag.Invalid_SemString), $"Skipping case: {scenario} with any tags: Invalid_SemString");

        Assert.Equal(data.SemPacked, VersionUtils.Parse(data.SemString));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Parse_Packed_ThrowsException_Argument_Sem(Case scenario, CaseRecord data)
    {
        Assert.SkipUnless(data.Tags.HasAny(CaseTag.Invalid_SemString), $"Skipping case: {scenario} without any tags: Invalid_SemString");

        Assert.Throws<ArgumentException>(() => VersionUtils.Parse(data.SemString));
    }
}