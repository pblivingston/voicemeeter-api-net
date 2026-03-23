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
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: PartsIn, Invalid_Packed");

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
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: KindIn, Invalid_Packed");

        var version = new VmVersion(data.Packed);

        Assert.Equal(data.K, version.K);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Constructor_Packed_ReturnsExpected_Semantic(Case scenario, CaseRecord data)
    {
        Assert.SkipWhen(data.Tags.HasAny(CaseTag.Invalid_Packs), $"Skipping case: {scenario} with any tags: Invalid_Packs");

        var version = new VmVersion(data.Packed);

        Assert.Equal(data.SemPacked, version.Semantic.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Constructor_Packed_ThrowsException_Argument(Case scenario, CaseRecord data)
    {
        Assert.SkipUnless(data.Tags.HasAny(CaseTag.Invalid_Packed), $"Skipping case: {scenario} without any tags: Invalid_Packed");

        Assert.Throws<ArgumentOutOfRangeException>(() => new VmVersion(data.Packed));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Constructor_Ints_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        Assert.SkipWhen(data.Tags.HasAny(CaseTag.Invalid_Parts), $"Skipping case: {scenario} with any tags: Invalid_Parts");

        var version = new VmVersion(data.Kind, data.Major, data.Minor, data.Patch);

        Assert.Equal(data.Packed, version.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Constructor_Ints_ThrowsException_Argument(Case scenario, CaseRecord data)
    {
        Assert.SkipUnless(data.Tags.HasAny(CaseTag.Invalid_Parts), $"Skipping case: {scenario} without any tags: Invalid_Parts");

        Assert.Throws<ArgumentException>(() => new VmVersion(data.Kind, data.Major, data.Minor, data.Patch));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Constructor_Kind_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        Assert.SkipWhen(data.Tags.HasAny(CaseTag.Invalid_Parts), $"Skipping case: {scenario} with any tags: Invalid_Parts");

        var version = new VmVersion(data.K, data.Major, data.Minor, data.Patch);

        Assert.Equal(data.Packed, version.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Constructor_Kind_ThrowsException_Argument(Case scenario, CaseRecord data)
    {
        Assert.SkipUnless(data.Tags.HasAny(CaseTag.Invalid_Parts), $"Skipping case: {scenario} without any tags: Invalid_Parts");

        Assert.Throws<ArgumentException>(() => new VmVersion(data.K, data.Major, data.Minor, data.Patch));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Constructor_SemVersion_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Kind | CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: Invalid_Kind, Invalid_SemPacked");

        var sem = new SemVersion(data.SemPacked);
        var version = new VmVersion(data.Kind, sem);

        Assert.Equal(data.Packed, version.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Constructor_SemVersion_ThrowsException_Argument(Case scenario, CaseRecord data)
    {
        Assert.SkipUnless(data.Tags.HasAny(CaseTag.Invalid_Kind), $"Skipping case: {scenario} without any tags: Invalid_Kind");
        Assert.SkipWhen(data.Tags.HasAny(CaseTag.Invalid_SemPacked), $"Skipping case: {scenario} with any tags: Invalid_SemPacked");

        var sem = new SemVersion(data.SemPacked);

        Assert.Throws<ArgumentException>(() => new VmVersion(data.Kind, sem));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Constructor_KindSemVersion_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Kind | CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: Invalid_Kind, Invalid_SemPacked");

        var sem = new SemVersion(data.SemPacked);
        var version = new VmVersion(data.K, sem);

        Assert.Equal(data.Packed, version.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Constructor_KindSemVersion_ThrowsException_Argument(Case scenario, CaseRecord data)
    {
        Assert.SkipUnless(data.Tags.HasAny(CaseTag.Invalid_Kind), $"Skipping case: {scenario} without any tags: Invalid_Kind");
        Assert.SkipWhen(data.Tags.HasAny(CaseTag.Invalid_SemPacked), $"Skipping case: {scenario} with any tags: Invalid_SemPacked");

        var sem = new SemVersion(data.SemPacked);

        Assert.Throws<ArgumentException>(() => new VmVersion(data.K, sem));
    }
}