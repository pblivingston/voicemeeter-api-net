using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VersionData;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.SemVersionTests;

public class Packing
{
    [Theory]
    [ClassData(typeof(VersionData))]
    public void RawPack_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        _ = scenario;

        Assert.Equal(data.SemPacked, SemVersion.RawPack(data.Major, data.Minor, data.Patch));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryPack_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        _ = scenario;

        var shouldSucceed = !data.Tags.HasAny(CaseTag.Invalid_Sems);

        var expected = shouldSucceed ? data.SemPacked : 0x0000_0000;

        var success = SemVersion.TryPack(data.Major, data.Minor, data.Patch, out int packed);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(expected, packed)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Pack_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        Assert.SkipWhen(data.Tags.HasAny(CaseTag.Invalid_Sems), $"Skipping case: {scenario} with any tags: Invalid_Sems");

        Assert.Equal(data.SemPacked, SemVersion.Pack(data.Major, data.Minor, data.Patch));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Pack_ThrowsException_Argument(Case scenario, CaseRecord data)
    {
        Assert.SkipUnless(data.Tags.HasAny(CaseTag.Invalid_Sems), $"Skipping case: {scenario} without any tags: Invalid_Sems");

        Assert.Throws<ArgumentException>(() => SemVersion.Pack(data.Major, data.Minor, data.Patch));
    }
}