namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VmVersionTests;

using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VersionData;

public class Validation
{
    [Theory]
    [ClassData(typeof(VersionData))]
    public void IsValidReturnsExpectedTrue(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidVmPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new VmVersion(data.VmPacked);

        Assert.True(version.IsValid());
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void IsValidVmVersionReturnsExpectedTrue(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidVmPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new VmVersion(data.VmPacked);

        Assert.True(VmVersion.IsValid(version));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void IsValidPackedReturnsExpectedBool(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        var badTags = CaseTag.InvalidVmPacked;
        var expected = !data.Tags.HasAny(badTags);

        Assert.Equal(expected, VmVersion.IsValid(data.VmPacked));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void IsValidIntsReturnsExpectedBool(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        var badTags = CaseTag.InvalidParts;
        var expected = !data.Tags.HasAny(badTags);

        Assert.Equal(expected, VmVersion.IsValid(data.Kind, data.Major, data.Minor, data.Patch));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void IsValidKindReturnsExpectedBool(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        var badTags = CaseTag.InvalidParts;
        var expected = !data.Tags.HasAny(badTags);

        Assert.Equal(expected, VmVersion.IsValid(data.K, data.Major, data.Minor, data.Patch));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void IsValidSemanticReturnsExpectedBool(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var badTags = CaseTag.InvalidKind;
        var expected = !data.Tags.HasAny(badTags);

        var semantic = new SemVersion(data.SemPacked);

        Assert.Equal(expected, VmVersion.IsValid(data.Kind, semantic));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void IsValidKindSemanticReturnsExpectedBool(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var badTags = CaseTag.InvalidKind;
        var expected = !data.Tags.HasAny(badTags);

        var semantic = new SemVersion(data.SemPacked);

        Assert.Equal(expected, VmVersion.IsValid(data.K, semantic));
    }
}
