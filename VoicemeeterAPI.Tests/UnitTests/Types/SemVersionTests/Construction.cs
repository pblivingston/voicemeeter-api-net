namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.SemVersionTests;

using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VersionData;

public class Construction
{
    [Theory]
    [ClassData(typeof(VersionData))]
    public void ConstructorPackedReturnsExpectedParts(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.SemsIn | CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new SemVersion(data.SemPacked);

        Assert.Multiple(
            () => Assert.Equal(data.SemPacked, version.Packed),
            () => Assert.Equal(0, ((IVersion)version).Kind),
            () => Assert.Equal(data.Major, version.Major),
            () => Assert.Equal(data.Minor, version.Minor),
            () => Assert.Equal(data.Patch, version.Patch)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ConstructorPackedReturnsExpectedKind(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new SemVersion(data.SemPacked);

        Assert.Equal(Kind.None, ((IVersion)version).K);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ConstructorPackedReturnsExpectedSemantic(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new SemVersion(data.SemPacked);

        Assert.Equal(data.SemPacked, ((IVersion)version).Semantic.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ConstructorPackedThrowsExceptionSemPackedOutOfRange(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidSemPacked;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<SemPackedOutOfRangeException>(() => new SemVersion(data.SemPacked));

        Assert.Equal(data.SemPacked, ex.ActualValue);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ConstructorSemsReturnsExpectedPacked(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidSems;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new SemVersion(data.Major, data.Minor, data.Patch);

        Assert.Equal(data.SemPacked, version.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ConstructorSemsThrowsExceptionPartsOutOfRange(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidSems;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<PartsOutOfRangeException>(() => new SemVersion(data.Major, data.Minor, data.Patch));

        Assert.Multiple(
            () => Assert.Equal(data.Major, ex.Major),
            () => Assert.Equal(data.Minor, ex.Minor),
            () => Assert.Equal(data.Patch, ex.Patch)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ConstructorVmVersionReturnsExpectedPacked(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidPacks;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var vm = new VmVersion(data.VmPacked);
        var version = new SemVersion(vm);

        Assert.Equal(data.SemPacked, version.Packed);
    }
}