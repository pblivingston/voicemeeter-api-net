using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VersionData;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VmVersionTests;

public class Validation
{
    [Theory]
    [ClassData(typeof(VersionData))]
    public void IsValid_ReturnsExpected_True(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Packed;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new VmVersion(data.Packed);

        Assert.True(version.IsValid());
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void IsValid_VmVersion_ReturnsExpected_True(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Packed;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new VmVersion(data.Packed);

        Assert.True(VmVersion.IsValid(version));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void IsValid_Packed_ReturnsExpected_Bool(Case scenario, CaseRecord data)
    {
        _ = scenario;

        var badTags = CaseTag.Invalid_Packed;
        var expected = !data.Tags.HasAny(badTags);

        Assert.Equal(expected, VmVersion.IsValid(data.Packed));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void IsValid_Ints_ReturnsExpected_Bool(Case scenario, CaseRecord data)
    {
        _ = scenario;

        var badTags = CaseTag.Invalid_Parts;
        var expected = !data.Tags.HasAny(badTags);

        Assert.Equal(expected, VmVersion.IsValid(data.Kind, data.Major, data.Minor, data.Patch));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void IsValid_Kind_ReturnsExpected_Bool(Case scenario, CaseRecord data)
    {
        _ = scenario;

        var badTags = CaseTag.Invalid_Parts;
        var expected = !data.Tags.HasAny(badTags);

        Assert.Equal(expected, VmVersion.IsValid(data.K, data.Major, data.Minor, data.Patch));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void IsValid_Semantic_ReturnsExpected_Bool(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var badTags = CaseTag.Invalid_Kind;
        var expected = !data.Tags.HasAny(badTags);

        var semantic = new SemVersion(data.SemPacked);

        Assert.Equal(expected, VmVersion.IsValid(data.Kind, semantic));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void IsValid_KindSemantic_ReturnsExpected_Bool(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var badTags = CaseTag.Invalid_Kind;
        var expected = !data.Tags.HasAny(badTags);

        var semantic = new SemVersion(data.SemPacked);

        Assert.Equal(expected, VmVersion.IsValid(data.K, semantic));
    }
}