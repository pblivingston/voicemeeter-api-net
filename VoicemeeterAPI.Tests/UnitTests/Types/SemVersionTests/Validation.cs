using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VersionData;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.SemVersionTests;

public class Validation
{
    [Theory]
    [ClassData(typeof(VersionData))]
    public void IsValid_ReturnsExpected_True(Case scenario, CaseRecord data)
    {
        Assert.SkipWhen(data.Tags.HasAny(CaseTag.Invalid_SemPacked), $"Skipping case: {scenario} with any tags: Invalid_SemPacked");

        var version = new SemVersion(data.SemPacked);

        Assert.True(version.IsValid());
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void IsValid_SemVersion_ReturnsExpected_True(Case scenario, CaseRecord data)
    {
        Assert.SkipWhen(data.Tags.HasAny(CaseTag.Invalid_SemPacked), $"Skipping case: {scenario} with any tags: Invalid_SemPacked");

        var version = new SemVersion(data.SemPacked);

        Assert.True(SemVersion.IsValid(version));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void IsValid_Packed_ReturnsExpected_Bool(Case scenario, CaseRecord data)
    {
        _ = scenario;

        var expected = !data.Tags.HasAny(CaseTag.Invalid_SemPacked);

        Assert.Equal(expected, SemVersion.IsValid(data.SemPacked));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void IsValid_Sems_ReturnsExpected_Bool(Case scenario, CaseRecord data)
    {
        _ = scenario;

        var expected = !data.Tags.HasAny(CaseTag.Invalid_Sems);

        Assert.Equal(expected, SemVersion.IsValid(data.Major, data.Minor, data.Patch));
    }
}