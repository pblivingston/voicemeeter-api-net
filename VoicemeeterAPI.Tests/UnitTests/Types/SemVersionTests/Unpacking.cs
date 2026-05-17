namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.SemVersionTests;

using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VersionData;

public class Unpacking
{
    [Theory]
    [ClassData(typeof(VersionData))]
    public void RawUnpackReturnsExpectedSems(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.SemsIn;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        SemVersion.RawUnpack(data.SemPacked, out var m, out var n, out var p);

        Assert.Multiple(
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryUnpackReturnsExpectedSems(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.SemsIn;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var badTags = CaseTag.InvalidSemPacked;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var success = SemVersion.TryUnpack(data.SemPacked, out var m, out var n, out var p);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void UnpackReturnsExpectedSems(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.SemsIn | CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        SemVersion.Unpack(data.SemPacked, out var m, out var n, out var p);

        Assert.Multiple(
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void UnpackThrowsExceptionSemPackedOutOfRange(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidSemPacked;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<SemPackedOutOfRangeException>(() => SemVersion.Unpack(data.SemPacked, out var _, out var _, out var _));

        Assert.Equal(data.SemPacked, ex.ActualValue);
    }
}