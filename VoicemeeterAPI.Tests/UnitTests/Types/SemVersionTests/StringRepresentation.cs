namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.SemVersionTests;

using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VersionData;

public class StringRepresentation
{
    [Theory]
    [ClassData(typeof(VersionData))]
    public void ToStringReturnsExpectedString(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidSemPacked | CaseTag.InvalidSemString;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new SemVersion(data.SemPacked);

        Assert.Equal(data.SemString, version.ToString());
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryParseSemsReturnsExpectedParts(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.SemsIn;
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

        var success = SemVersion.TryParse(data.SemString, out var m, out var n, out var p);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(maj, m),
            () => Assert.Equal(min, n),
            () => Assert.Equal(pat, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ParseSemsReturnsExpectedParts(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.SemsIn | CaseTag.InvalidSemString;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        SemVersion.Parse(data.SemString, out var m, out var n, out var p);

        Assert.Multiple(
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ParseSemsThrowsExceptionCannotParseAsParts(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidSemString;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<CannotParseAsPartsException>(() => SemVersion.Parse(data.SemString, out var _, out var _, out var _));

        Assert.Equal(data.SemString, ex.ActualValue);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryParseSemPackedReturnsExpectedPacked(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        var badTags = CaseTag.InvalidSemString;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var expected = shouldSucceed ? data.SemPacked : 0x0000_0000;

        var success = SemVersion.TryParse(data.SemString, out int p);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(expected, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ParseSemPackedReturnsExpectedPacked(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidSemString;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        SemVersion.Parse(data.SemString, out var p);

        Assert.Equal(data.SemPacked, p);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ParseSemPackedThrowsExceptionCannotParseAsParts(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidSemString;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<CannotParseAsPartsException>(() => SemVersion.Parse(data.SemString, out var _));

        Assert.Equal(data.SemString, ex.ActualValue);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryParseSemVersionReturnsExpectedVersion(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        var badTags = CaseTag.InvalidSemString;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var expected = shouldSucceed ? data.SemPacked : 0x0000_0000;

        var success = SemVersion.TryParse(data.SemString, out SemVersion s);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(expected, s.Packed)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ParseSemVersionReturnsExpectedVersion(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidSemString;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = SemVersion.Parse(data.SemString);

        Assert.Equal(data.SemPacked, version.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ParseSemVersionThrowsExceptionCannotParseAsType(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidSemString;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<CannotParseAsTypeException>(() => SemVersion.Parse(data.SemString));

        Assert.Multiple(
            () => Assert.Equal(data.SemString, ex.ActualValue),
            () => Assert.Equal(typeof(SemVersion), ex.Type)
        );
    }
}