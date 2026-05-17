namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.SemVersionTests;

using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VersionData;

public class Packing
{
    [Theory]
    [ClassData(typeof(VersionData))]
    public void RawPackReturnsExpectedPacked(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        Assert.Equal(data.SemPacked, SemVersion.RawPack(data.Major, data.Minor, data.Patch));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryPackReturnsExpectedPacked(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        var badTags = CaseTag.InvalidSems;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var expected = shouldSucceed ? data.SemPacked : 0x0000_0000;

        var success = SemVersion.TryPack(data.Major, data.Minor, data.Patch, out var packed);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(expected, packed)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void PackReturnsExpectedPacked(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidSems;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        Assert.Equal(data.SemPacked, SemVersion.Pack(data.Major, data.Minor, data.Patch));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void PackThrowsExceptionPartsOutOfRange(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidSems;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<PartsOutOfRangeException>(() => SemVersion.Pack(data.Major, data.Minor, data.Patch));

        Assert.Multiple(
            () => Assert.Equal(data.Major, ex.Major),
            () => Assert.Equal(data.Minor, ex.Minor),
            () => Assert.Equal(data.Patch, ex.Patch)
        );
    }
}