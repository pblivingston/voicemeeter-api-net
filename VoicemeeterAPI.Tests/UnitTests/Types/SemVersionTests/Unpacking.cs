using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VersionData;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.SemVersionTests;

public class Unpacking
{
    [Theory]
    [ClassData(typeof(VersionData))]
    public void RawUnpack_ReturnsExpected_Sems(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.SemsIn;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        SemVersion.RawUnpack(data.SemPacked, out int m, out int n, out int p);

        Assert.Multiple(
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryUnpack_ReturnsExpected_Sems(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.SemsIn;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var badTags = CaseTag.Invalid_SemPacked;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var success = SemVersion.TryUnpack(data.SemPacked, out int m, out int n, out int p);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Unpack_ReturnsExpected_Sems(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.SemsIn | CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        SemVersion.Unpack(data.SemPacked, out int m, out int n, out int p);

        Assert.Multiple(
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Unpack_ThrowsException_ArgumentOutOfRange(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_SemPacked;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        Assert.Throws<ArgumentOutOfRangeException>(() => SemVersion.Unpack(data.SemPacked, out int _, out int _, out int _));
    }
}