namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.SemVersionTests;

using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VersionData;

public class Validation
{
    [Theory]
    [ClassData(typeof(VersionData))]
    public void IsValidReturnsExpectedTrue(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new SemVersion(data.SemPacked);

        Assert.True(version.IsValid());
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void IsValidSemVersionReturnsExpectedTrue(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new SemVersion(data.SemPacked);

        Assert.True(SemVersion.IsValid(version));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void IsValidPackedReturnsExpectedBool(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        var badTags = CaseTag.InvalidSemPacked;
        var expected = !data.Tags.HasAny(badTags);

        Assert.Equal(expected, SemVersion.IsValid(data.SemPacked));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void IsValidSemsReturnsExpectedBool(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        var badTags = CaseTag.InvalidSems;
        var expected = !data.Tags.HasAny(badTags);

        Assert.Equal(expected, SemVersion.IsValid(data.Major, data.Minor, data.Patch));
    }
}