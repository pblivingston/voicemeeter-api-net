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

        var badTags = CaseTag.Invalid_Sems;
        var expected = !data.Tags.HasAny(badTags);

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
        var skipTags = CaseTag.PartsIn;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

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
    public void ToString_ReturnsExpected_String(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Incompat_Strings;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        Assert.Equal(data.String, VersionUtils.ToString(data.Packed));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryParse_Parts_ReturnsExpected_Parts_Vm(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.PartsIn | CaseTag.Three_String;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var badTags = CaseTag.Invalid_String;
        var shouldSucceed = !data.Tags.HasAny(badTags);

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
        var skipTags = CaseTag.SemsIn | CaseTag.Four_SemString;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var badTags = CaseTag.Invalid_SemString;
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
    [ClassData(typeof(VersionData))]
    public void Parse_Parts_ReturnsExpected_Parts_Vm(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.PartsIn | CaseTag.Invalid_String;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

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
        var runTags = CaseTag.Invalid_String;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var skipTags = CaseTag.Three_String;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        Assert.Throws<ArgumentException>(() => VersionUtils.Parse(data.String, out int _, out int _, out int _, out int _));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Parse_Parts_ReturnsExpected_Parts_Sem(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.SemsIn | CaseTag.Invalid_SemString;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

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
        var runTags = CaseTag.Invalid_SemString;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var skipTags = CaseTag.Four_SemString;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        Assert.Throws<ArgumentException>(() => VersionUtils.Parse(data.SemString, out int _, out int _, out int _, out int _));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryParse_Packed_ReturnsExpected_Packed_Vm(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Three_String;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var badTags = CaseTag.Invalid_String;
        var shouldSucceed = !data.Tags.HasAny(badTags);

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
        var skipTags = CaseTag.Four_SemString;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var badTags = CaseTag.Invalid_SemString;
        var shouldSucceed = !data.Tags.HasAny(badTags);

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
        var skipTags = CaseTag.Invalid_String;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        Assert.Equal(data.Packed, VersionUtils.Parse(data.String));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Parse_Packed_ThrowsException_Argument_Vm(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_String;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var skipTags = CaseTag.Three_String;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        Assert.Throws<ArgumentException>(() => VersionUtils.Parse(data.String));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Parse_Packed_ReturnsExpected_Packed_Sem(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_SemString;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        Assert.Equal(data.SemPacked, VersionUtils.Parse(data.SemString));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Parse_Packed_ThrowsException_Argument_Sem(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_SemString;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var skipTags = CaseTag.Four_SemString;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        Assert.Throws<ArgumentException>(() => VersionUtils.Parse(data.SemString));
    }
}