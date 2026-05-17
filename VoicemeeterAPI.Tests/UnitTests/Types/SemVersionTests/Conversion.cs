namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.SemVersionTests;

using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VersionData;

public class Conversion
{
    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastFromVmVersionReturnsExpectedPacked(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidPacks;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var vm = new VmVersion(data.VmPacked);

        var version = (SemVersion)vm;

        Assert.Equal(data.SemPacked, version.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastToIntReturnsExpectedPacked(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new SemVersion(data.SemPacked);

        Assert.Equal(data.SemPacked, (int)version);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastFromIntReturnsExpectedPacked(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = (SemVersion)data.SemPacked;

        Assert.Equal(data.SemPacked, version.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastFromIntThrowsExceptionSemPackedOutOfRange(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidSemPacked;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<SemPackedOutOfRangeException>(() => (SemVersion)data.SemPacked);

        Assert.Equal(data.SemPacked, ex.ActualValue);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastToTupleSemsReturnsExpectedParts(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidSemPacked | CaseTag.SemsIn;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new SemVersion(data.SemPacked);

        var (m, n, p) = ((int, int, int))version;

        Assert.Multiple(
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastFromTupleSemsReturnsExpectedPacked(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidSems;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = (SemVersion)(data.Major, data.Minor, data.Patch);

        Assert.Equal(data.SemPacked, version.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastFromTupleSemsThrowsExceptionPartsOutOfRange(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidSems;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<PartsOutOfRangeException>(() => (SemVersion)(data.Major, data.Minor, data.Patch));

        Assert.Multiple(
            () => Assert.Equal(data.Major, ex.Major),
            () => Assert.Equal(data.Minor, ex.Minor),
            () => Assert.Equal(data.Patch, ex.Patch)
        );
    }
}
