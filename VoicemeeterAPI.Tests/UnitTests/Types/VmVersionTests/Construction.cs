namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VmVersionTests;

using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VersionData;

public class Construction
{
    [Theory]
    [ClassData(typeof(VersionData))]
    public void ConstructorPackedReturnsExpectedParts(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.PartsIn | CaseTag.InvalidVmPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new VmVersion(data.VmPacked);

        Assert.Multiple(
            () => Assert.Equal(data.VmPacked, version.Packed),
            () => Assert.Equal(data.Kind, version.Kind),
            () => Assert.Equal(data.Major, version.Major),
            () => Assert.Equal(data.Minor, version.Minor),
            () => Assert.Equal(data.Patch, version.Patch)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ConstructorPackedReturnsExpectedKind(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.KindIn | CaseTag.InvalidVmPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new VmVersion(data.VmPacked);

        Assert.Equal(data.K, version.K);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ConstructorPackedReturnsExpectedSemantic(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidPacks;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new VmVersion(data.VmPacked);

        Assert.Equal(data.SemPacked, version.Semantic.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ConstructorPackedThrowsExceptionVmPackedOutOfRange(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidVmPacked;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<VmPackedOutOfRangeException>(() => new VmVersion(data.VmPacked));

        Assert.Equal(data.VmPacked, ex.ActualValue);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ConstructorIntsReturnsExpectedPacked(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidParts;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new VmVersion(data.Kind, data.Major, data.Minor, data.Patch);

        Assert.Equal(data.VmPacked, version.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ConstructorIntsThrowsExceptionPartsOutOfRange(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidParts;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<PartsOutOfRangeException<int>>(() => new VmVersion(data.Kind, data.Major, data.Minor, data.Patch));

        Assert.Multiple(
            () => Assert.Equal(data.Kind, ex.Kind),
            () => Assert.Equal(data.Major, ex.Major),
            () => Assert.Equal(data.Minor, ex.Minor),
            () => Assert.Equal(data.Patch, ex.Patch)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ConstructorKindReturnsExpectedPacked(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidParts;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new VmVersion(data.K, data.Major, data.Minor, data.Patch);

        Assert.Equal(data.VmPacked, version.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ConstructorKindThrowsExceptionPartsOutOfRange(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidParts;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<PartsOutOfRangeException<Kind>>(() => new VmVersion(data.K, data.Major, data.Minor, data.Patch));

        Assert.Multiple(
            () => Assert.Equal(data.K, ex.Kind),
            () => Assert.Equal(data.Major, ex.Major),
            () => Assert.Equal(data.Minor, ex.Minor),
            () => Assert.Equal(data.Patch, ex.Patch)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ConstructorSemVersionReturnsExpectedPacked(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidKind | CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var sem = new SemVersion(data.SemPacked);
        var version = new VmVersion(data.Kind, sem);

        Assert.Equal(data.VmPacked, version.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ConstructorSemVersionThrowsExceptionPartsOutOfRange(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidKind;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var skipTags = CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var sem = new SemVersion(data.SemPacked);

        var ex = Assert.Throws<PartsOutOfRangeException<int>>(() => new VmVersion(data.Kind, sem));

        Assert.Multiple(
            () => Assert.Equal(data.Kind, ex.Kind),
            () => Assert.Equal(sem, ex.Semantic)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ConstructorKindSemVersionReturnsExpectedPacked(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidKind | CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var sem = new SemVersion(data.SemPacked);
        var version = new VmVersion(data.K, sem);

        Assert.Equal(data.VmPacked, version.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ConstructorKindSemVersionThrowsExceptionPartsOutOfRange(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidKind;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var skipTags = CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var sem = new SemVersion(data.SemPacked);

        var ex = Assert.Throws<PartsOutOfRangeException<Kind>>(() => new VmVersion(data.K, sem));

        Assert.Multiple(
            () => Assert.Equal(data.K, ex.Kind),
            () => Assert.Equal(sem, ex.Semantic)
        );
    }
}