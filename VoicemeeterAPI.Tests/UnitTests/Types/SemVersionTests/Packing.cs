using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VersionUtilsTests;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.SemVersionTests;

public class Packing
{
    public static TheoryDataRow<Case, CaseRecord>[] GetCaseData => VersionUtilsTests.GetCaseData;

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void RawPack_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Sems | CaseTag.Invalid_Packed;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: Invalid_Sems, Invalid_Packed");

        Assert.Equal(data.SemPacked, SemVersion.RawPack(data.Major, data.Minor, data.Patch));
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void TryPack_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        Assert.SkipWhen(data.Tags.HasAny(CaseTag.Invalid_Packed), $"Skipping case: {scenario} with any tags: Invalid_Packed");

        var badTags = CaseTag.Invalid_Sems;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var expected = shouldSucceed ? data.SemPacked : 0x0000_0000;

        var success = SemVersion.TryPack(data.Major, data.Minor, data.Patch, out int packed);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(expected, packed)
        );
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void Pack_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Sems | CaseTag.Invalid_Packed;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: Invalid_Sems, Invalid_Packed");

        Assert.Equal(data.SemPacked, SemVersion.Pack(data.Major, data.Minor, data.Patch));
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void Pack_ThrowsException_Argument(Case scenario, CaseRecord data)
    {
        Assert.SkipUnless(data.Tags.HasAny(CaseTag.Invalid_Sems), $"Skipping case: {scenario} without any tags: Invalid_Sems");

        Assert.Throws<ArgumentException>(() => SemVersion.Pack(data.Major, data.Minor, data.Patch));
    }
}