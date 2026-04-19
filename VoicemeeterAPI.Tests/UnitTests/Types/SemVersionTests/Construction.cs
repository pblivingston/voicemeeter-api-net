using PBLivingston.VoicemeeterAPI.EventManagement.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VersionData;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.SemVersionTests;

public class Construction
{
    [Theory]
    [ClassData(typeof(VersionData))]
    public void Constructor_Packed_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.SemsIn | CaseTag.Invalid_SemPacked;
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
    public void Constructor_Packed_ReturnsExpected_Kind(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new SemVersion(data.SemPacked);

        Assert.Equal(Kind.None, ((IVersion)version).K);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Constructor_Packed_ReturnsExpected_Semantic(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new SemVersion(data.SemPacked);

        Assert.Equal(data.SemPacked, ((IVersion)version).Semantic.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Constructor_Packed_ThrowsException_SemPackedOutOfRange(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_SemPacked;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<SemPackedOutOfRangeException>(() => new SemVersion(data.SemPacked));

        Assert.Equal(data.SemPacked, ex.ActualValue);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Constructor_Sems_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Sems;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new SemVersion(data.Major, data.Minor, data.Patch);

        Assert.Equal(data.SemPacked, version.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Constructor_Sems_ThrowsException_PartsOutOfRange(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_Sems;
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
    public void Constructor_VmVersion_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Packs;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var vm = new VmVersion(data.Packed);
        var version = new SemVersion(vm);

        Assert.Equal(data.SemPacked, version.Packed);
    }
}