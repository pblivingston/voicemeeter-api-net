using PBLivingston.VoicemeeterAPI.EventManagement.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VersionData;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.SemVersionTests;

public class Conversion
{
    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastFrom_VmVersion_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Packs;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var vm = new VmVersion(data.Packed);

        var version = (SemVersion)vm;

        Assert.Equal(data.SemPacked, version.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastTo_Int_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new SemVersion(data.SemPacked);

        Assert.Equal(data.SemPacked, (int)version);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastFrom_Int_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = (SemVersion)data.SemPacked;

        Assert.Equal(data.SemPacked, version.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastFrom_Int_ThrowsException_SemPackedOutOfRange(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_SemPacked;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<SemPackedOutOfRangeException>(() => (SemVersion)data.SemPacked);

        Assert.Equal(data.SemPacked, ex.ActualValue);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastTo_TupleSems_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_SemPacked | CaseTag.SemsIn;
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
    public void CastFrom_TupleSems_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Sems;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = (SemVersion)(data.Major, data.Minor, data.Patch);

        Assert.Equal(data.SemPacked, version.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastFrom_TupleSems_ThrowsException_PartsOutOfRange(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_Sems;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<PartsOutOfRangeException>(() => (SemVersion)(data.Major, data.Minor, data.Patch));

        Assert.Multiple(
            () => Assert.Equal(data.Major, ex.Major),
            () => Assert.Equal(data.Minor, ex.Minor),
            () => Assert.Equal(data.Patch, ex.Patch)
        );
    }
}