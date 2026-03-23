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
    [ClassData(typeof(VersionData))]
    public void Pack_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Sems;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        Assert.Equal(data.SemPacked, SemVersion.Pack(data.Major, data.Minor, data.Patch));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Pack_ThrowsException_Argument(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_Sems;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        Assert.Throws<ArgumentException>(() => SemVersion.Pack(data.Major, data.Minor, data.Patch));
    }
}