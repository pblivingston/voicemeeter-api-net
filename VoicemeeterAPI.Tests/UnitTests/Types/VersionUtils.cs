namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types;

using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VersionData;

public class VersionUtilsTests
{
    [Theory]
    [ClassData(typeof(VersionData))]
    public void IsValidReturnsExpectedBool(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        var badTags = CaseTag.InvalidSems;
        var expected = !data.Tags.HasAny(badTags);

        Assert.Equal(expected, VersionUtils.IsValid(data.Major, data.Minor, data.Patch));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void RawPackReturnsExpectedInt(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        Assert.Equal(data.VmPacked, VersionUtils.RawPack(data.Kind, data.Major, data.Minor, data.Patch));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void RawUnpackReturnsExpectedParts(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.PartsIn;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        VersionUtils.RawUnpack(data.VmPacked, out var k, out var m, out var n, out var p);

        Assert.Multiple(
            () => Assert.Equal(data.Kind, k),
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ToStringReturnsExpectedString(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.IncompatStrings;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        Assert.Equal(data.VmString, VersionUtils.ToString(data.VmPacked));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryParsePartsReturnsExpectedPartsVm(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.PartsIn | CaseTag.ThreeString;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var badTags = CaseTag.InvalidVmString;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var kind = 0;
        var maj = 0;
        var min = 0;
        var pat = 0;
        if (shouldSucceed)
        {
            kind = data.Kind;
            maj = data.Major;
            min = data.Minor;
            pat = data.Patch;
        }

        var success = VersionUtils.TryParse(data.VmString, out var k, out var m, out var n, out var p);

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
    public void TryParsePartsReturnsExpectedPartsSem(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.SemsIn | CaseTag.FourSemString;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var badTags = CaseTag.InvalidSemString;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var maj = 0;
        var min = 0;
        var pat = 0;
        if (shouldSucceed)
        {
            maj = data.Major;
            min = data.Minor;
            pat = data.Patch;
        }

        var success = VersionUtils.TryParse(data.SemString, out var k, out var m, out var n, out var p);

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
    public void ParsePartsReturnsExpectedPartsVm(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.PartsIn | CaseTag.InvalidVmString;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        VersionUtils.Parse(data.VmString, out var k, out var m, out var n, out var p);

        Assert.Multiple(
            () => Assert.Equal(data.Kind, k),
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ParsePartsThrowsExceptionCannotParseAsPartsVm(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidVmString;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var skipTags = CaseTag.ThreeString;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var ex = Assert.Throws<CannotParseAsPartsException>(() => VersionUtils.Parse(data.VmString, out var _, out var _, out var _, out var _));

        Assert.Equal(data.VmString, ex.ActualValue);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ParsePartsReturnsExpectedPartsSem(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.SemsIn | CaseTag.InvalidSemString;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        VersionUtils.Parse(data.SemString, out var k, out var m, out var n, out var p);

        Assert.Multiple(
            () => Assert.Equal(0, k),
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ParsePartsThrowsExceptionCannotParseAsPartsSem(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidSemString;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var skipTags = CaseTag.FourSemString;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var ex = Assert.Throws<CannotParseAsPartsException>(() => VersionUtils.Parse(data.SemString, out var _, out var _, out var _, out var _));

        Assert.Equal(data.SemString, ex.ActualValue);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryParsePackedReturnsExpectedPackedVm(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.ThreeString;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var badTags = CaseTag.InvalidVmString;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var expected = shouldSucceed ? data.VmPacked : 0x0000_0000;

        var success = VersionUtils.TryParse(data.VmString, out var result);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(expected, result)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryParsePackedReturnsExpectedPackedSem(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.FourSemString;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var badTags = CaseTag.InvalidSemString;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var expected = shouldSucceed ? data.SemPacked : 0x0000_0000;

        var success = VersionUtils.TryParse(data.SemString, out var result);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(expected, result)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ParsePackedReturnsExpectedPackedVm(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidVmString;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        Assert.Equal(data.VmPacked, VersionUtils.Parse(data.VmString));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ParsePackedThrowsExceptionCannotParseAsPartsVm(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidVmString;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var skipTags = CaseTag.ThreeString;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var ex = Assert.Throws<CannotParseAsPartsException>(() => VersionUtils.Parse(data.VmString));

        Assert.Equal(data.VmString, ex.ActualValue);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ParsePackedReturnsExpectedPackedSem(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidSemString;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        Assert.Equal(data.SemPacked, VersionUtils.Parse(data.SemString));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ParsePackedThrowsExceptionArgumentSem(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidSemString;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var skipTags = CaseTag.FourSemString;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var ex = Assert.Throws<CannotParseAsPartsException>(() => VersionUtils.Parse(data.SemString));

        Assert.Equal(data.SemString, ex.ActualValue);
    }
}
