using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VersionData;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VmVersionTests;

public class Construction
{
    [Theory]
    [ClassData(typeof(VersionData))]
    public void Constructor_Packed_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.PartsIn | CaseTag.Invalid_Packed;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new VmVersion(data.Packed);

        Assert.Multiple(
            () => Assert.Equal(data.Packed, version.Packed),
            () => Assert.Equal(data.Kind, version.Kind),
            () => Assert.Equal(data.Major, version.Major),
            () => Assert.Equal(data.Minor, version.Minor),
            () => Assert.Equal(data.Patch, version.Patch)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Constructor_Packed_ReturnsExpected_Kind(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.KindIn | CaseTag.Invalid_Packed;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new VmVersion(data.Packed);

        Assert.Equal(data.K, version.K);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Constructor_Packed_ReturnsExpected_Semantic(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Packs;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new VmVersion(data.Packed);

        Assert.Equal(data.SemPacked, version.Semantic.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Constructor_Packed_ThrowsException_ArgumentOutOfRange(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_Packed;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        Assert.Throws<ArgumentOutOfRangeException>(() => new VmVersion(data.Packed));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Constructor_Ints_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Parts;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new VmVersion(data.Kind, data.Major, data.Minor, data.Patch);

        Assert.Equal(data.Packed, version.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Constructor_Ints_ThrowsException_Argument(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_Parts;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        Assert.Throws<ArgumentException>(() => new VmVersion(data.Kind, data.Major, data.Minor, data.Patch));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Constructor_Kind_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Parts;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new VmVersion(data.K, data.Major, data.Minor, data.Patch);

        Assert.Equal(data.Packed, version.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Constructor_Kind_ThrowsException_Argument(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_Parts;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        Assert.Throws<ArgumentException>(() => new VmVersion(data.K, data.Major, data.Minor, data.Patch));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Constructor_SemVersion_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Kind | CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var sem = new SemVersion(data.SemPacked);
        var version = new VmVersion(data.Kind, sem);

        Assert.Equal(data.Packed, version.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Constructor_SemVersion_ThrowsException_Argument(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_Kind;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var skipTags = CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var sem = new SemVersion(data.SemPacked);

        Assert.Throws<ArgumentException>(() => new VmVersion(data.Kind, sem));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Constructor_KindSemVersion_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Kind | CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var sem = new SemVersion(data.SemPacked);
        var version = new VmVersion(data.K, sem);

        Assert.Equal(data.Packed, version.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Constructor_KindSemVersion_ThrowsException_Argument(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_Kind;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var skipTags = CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var sem = new SemVersion(data.SemPacked);

        Assert.Throws<ArgumentException>(() => new VmVersion(data.K, sem));
    }
}