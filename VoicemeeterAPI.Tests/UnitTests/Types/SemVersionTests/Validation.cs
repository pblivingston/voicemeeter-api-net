using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VersionUtilsTests;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.SemVersionTests;

public class Validation
{
    public static TheoryDataRow<Case, CaseRecord>[] GetCaseData => VersionUtilsTests.GetCaseData;

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void IsValid_ReturnsExpected_True(Case scenario, CaseRecord data)
    {
        Assert.SkipWhen(data.Tags.HasAny(CaseTag.Invalid_Packed), $"Skipping case: {scenario} with any tags: Invalid_Packed");

        var version = new SemVersion(data.SemPacked);

        Assert.True(version.IsValid());
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void IsValid_SemVersion_ReturnsExpected_True(Case scenario, CaseRecord data)
    {
        Assert.SkipWhen(data.Tags.HasAny(CaseTag.Invalid_Packed), $"Skipping case: {scenario} with any tags: Invalid_Packed");

        var version = new SemVersion(data.SemPacked);

        Assert.True(SemVersion.IsValid(version));
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void IsValid_Packed_ReturnsExpected_Bool(Case scenario, CaseRecord data)
    {
        _ = scenario;

        var expected = !data.Tags.HasAny(CaseTag.Invalid_Packed);

        Assert.Equal(expected, SemVersion.IsValid(data.SemPacked));
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void IsValid_Sems_ReturnsExpected_Bool(Case scenario, CaseRecord data)
    {
        _ = scenario;

        var expected = !data.Tags.HasAny(CaseTag.Invalid_Sems);

        Assert.Equal(expected, SemVersion.IsValid(data.Major, data.Minor, data.Patch));
    }
}