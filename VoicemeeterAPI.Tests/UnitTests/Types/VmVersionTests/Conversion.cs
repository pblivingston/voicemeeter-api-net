namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VmVersionTests;

using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VersionData;

public class Conversion
{
    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastToIntReturnsExpectedPacked(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidVmPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new VmVersion(data.VmPacked);

        Assert.Equal(data.VmPacked, (int)version);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastFromIntReturnsExpectedPacked(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidVmPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = (VmVersion)data.VmPacked;

        Assert.Equal(data.VmPacked, version.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastFromIntThrowsExceptionVmPackedOutOfRange(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidVmPacked;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<VmPackedOutOfRangeException>(() => (VmVersion)data.VmPacked);

        Assert.Equal(data.VmPacked, ex.ActualValue);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastToTupleIntsReturnsExpectedParts(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidVmPacked | CaseTag.PartsIn;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new VmVersion(data.VmPacked);

        var (k, m, n, p) = ((int, int, int, int))version;

        Assert.Multiple(
            () => Assert.Equal(data.Kind, k),
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastFromTupleIntsReturnsExpectedPacked(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidParts;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = (VmVersion)(data.Kind, data.Major, data.Minor, data.Patch);

        Assert.Equal(data.VmPacked, version.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastFromTupleIntsThrowsExceptionPartsOutOfRange(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidParts;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<PartsOutOfRangeException<int>>(() => (VmVersion)(data.Kind, data.Major, data.Minor, data.Patch));

        Assert.Multiple(
            () => Assert.Equal(data.Kind, ex.Kind),
            () => Assert.Equal(data.Major, ex.Major),
            () => Assert.Equal(data.Minor, ex.Minor),
            () => Assert.Equal(data.Patch, ex.Patch)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastToTupleKindReturnsExpectedParts(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidVmPacked | CaseTag.PartsIn;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new VmVersion(data.VmPacked);

        var (k, m, n, p) = ((Kind, int, int, int))version;

        Assert.Multiple(
            () => Assert.Equal(data.K, k),
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastFromTupleKindReturnsExpectedPacked(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidParts;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = (VmVersion)(data.K, data.Major, data.Minor, data.Patch);

        Assert.Equal(data.VmPacked, version.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastFromTupleKindThrowsExceptionPartsOutOfRange(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidParts;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<PartsOutOfRangeException<Kind>>(() => (VmVersion)(data.K, data.Major, data.Minor, data.Patch));

        Assert.Multiple(
            () => Assert.Equal(data.K, ex.Kind),
            () => Assert.Equal(data.Major, ex.Major),
            () => Assert.Equal(data.Minor, ex.Minor),
            () => Assert.Equal(data.Patch, ex.Patch)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastToTupleSemanticReturnsExpectedParts(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidPacks | CaseTag.KindIn;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new VmVersion(data.VmPacked);

        var (k, sem) = ((int, SemVersion))version;

        Assert.Multiple(
            () => Assert.Equal(data.Kind, k),
            () => Assert.Equal(data.SemPacked, sem.Packed)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastFromTupleSemanticReturnsExpectedPacked(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidKind | CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var sem = new SemVersion(data.SemPacked);

        var version = (VmVersion)(data.Kind, sem);

        Assert.Equal(data.VmPacked, version.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastFromTupleSemanticThrowsExceptionPartsOutOfRange(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidKind;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var skipTags = CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var sem = new SemVersion(data.SemPacked);

        var ex = Assert.Throws<PartsOutOfRangeException<int>>(() => (VmVersion)(data.Kind, sem));

        Assert.Multiple(
            () => Assert.Equal(data.Kind, ex.Kind),
            () => Assert.Equal(sem, ex.Semantic)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastToTupleKindSemanticReturnsExpectedParts(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidPacks | CaseTag.KindIn;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new VmVersion(data.VmPacked);

        var (k, sem) = ((Kind, SemVersion))version;

        Assert.Multiple(
            () => Assert.Equal(data.K, k),
            () => Assert.Equal(data.SemPacked, sem.Packed)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastFromTupleKindSemanticReturnsExpectedPacked(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidKind | CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var sem = new SemVersion(data.SemPacked);

        var version = (VmVersion)(data.K, sem);

        Assert.Equal(data.VmPacked, version.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastFromTupleKindSemanticThrowsExceptionPartsOutOfRange(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidKind;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var skipTags = CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var sem = new SemVersion(data.SemPacked);

        var ex = Assert.Throws<PartsOutOfRangeException<Kind>>(() => (VmVersion)(data.K, sem));

        Assert.Multiple(
            () => Assert.Equal(data.K, ex.Kind),
            () => Assert.Equal(sem, ex.Semantic)
        );
    }
}
