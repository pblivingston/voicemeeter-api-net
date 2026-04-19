using PBLivingston.VoicemeeterAPI.EventManagement.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VersionData;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.SemVersionTests;

public class StringRepresentation
{
    [Theory]
    [ClassData(typeof(VersionData))]
    public void ToString_ReturnsExpected_String(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_SemPacked | CaseTag.Invalid_SemString;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new SemVersion(data.SemPacked);

        Assert.Equal(data.SemString, version.ToString());
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryParse_Sems_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.SemsIn;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var badTags = CaseTag.Invalid_SemString;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var maj = 0; var min = 0; var pat = 0;
        if (shouldSucceed) { maj = data.Major; min = data.Minor; pat = data.Patch; }

        var success = SemVersion.TryParse(data.SemString, out int m, out int n, out int p);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(maj, m),
            () => Assert.Equal(min, n),
            () => Assert.Equal(pat, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Parse_Sems_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.SemsIn | CaseTag.Invalid_SemString;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        SemVersion.Parse(data.SemString, out int m, out int n, out int p);

        Assert.Multiple(
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Parse_Sems_ThrowsException_CannotParseAsParts(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_SemString;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<CannotParseAsPartsException>(() => SemVersion.Parse(data.SemString, out int _, out int _, out int _));

        Assert.Equal(data.SemString, ex.ActualValue);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryParse_SemPacked_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        _ = scenario;

        var badTags = CaseTag.Invalid_SemString;
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
    public void Parse_SemPacked_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_SemString;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        SemVersion.Parse(data.SemString, out int p);

        Assert.Equal(data.SemPacked, p);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Parse_SemPacked_ThrowsException_CannotParseAsParts(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_SemString;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<CannotParseAsPartsException>(() => SemVersion.Parse(data.SemString, out int _));

        Assert.Equal(data.SemString, ex.ActualValue);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryParse_SemVersion_ReturnsExpected_Version(Case scenario, CaseRecord data)
    {
        _ = scenario;

        var badTags = CaseTag.Invalid_SemString;
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
    public void Parse_SemVersion_ReturnsExpected_Version(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_SemString;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = SemVersion.Parse(data.SemString);

        Assert.Equal(data.SemPacked, version.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Parse_SemVersion_ThrowsException_CannotParseAsType(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_SemString;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<CannotParseAsTypeException>(() => SemVersion.Parse(data.SemString));

        Assert.Multiple(
            () => Assert.Equal(data.SemString, ex.ActualValue),
            () => Assert.Equal(typeof(SemVersion), ex.Type)
        );
    }
}