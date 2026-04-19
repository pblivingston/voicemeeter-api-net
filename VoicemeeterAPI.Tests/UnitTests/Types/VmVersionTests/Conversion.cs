using PBLivingston.VoicemeeterAPI.EventManagement.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VersionData;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VmVersionTests;

public class Conversion
{
    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastTo_Int_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Packed;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new VmVersion(data.Packed);

        Assert.Equal(data.Packed, (int)version);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastFrom_Int_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Packed;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = (VmVersion)data.Packed;

        Assert.Equal(data.Packed, version.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastFrom_Int_ThrowsException_VmPackedOutOfRange(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_Packed;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<VmPackedOutOfRangeException>(() => (VmVersion)data.Packed);

        Assert.Equal(data.Packed, ex.ActualValue);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastTo_TupleInts_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Packed | CaseTag.PartsIn;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new VmVersion(data.Packed);

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
    public void CastFrom_TupleInts_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Parts;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = (VmVersion)(data.Kind, data.Major, data.Minor, data.Patch);

        Assert.Equal(data.Packed, version.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastFrom_TupleInts_ThrowsException_PartsOutOfRange(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_Parts;
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
    public void CastTo_TupleKind_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Packed | CaseTag.PartsIn;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new VmVersion(data.Packed);

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
    public void CastFrom_TupleKind_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Parts;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = (VmVersion)(data.K, data.Major, data.Minor, data.Patch);

        Assert.Equal(data.Packed, version.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastFrom_TupleKind_ThrowsException_PartsOutOfRange(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_Parts;
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
    public void CastTo_TupleSemantic_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Packs | CaseTag.KindIn;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new VmVersion(data.Packed);

        var (k, sem) = ((int, SemVersion))version;

        Assert.Multiple(
            () => Assert.Equal(data.Kind, k),
            () => Assert.Equal(data.SemPacked, sem.Packed)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastFrom_TupleSemantic_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Kind | CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var sem = new SemVersion(data.SemPacked);

        var version = (VmVersion)(data.Kind, sem);

        Assert.Equal(data.Packed, version.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastFrom_TupleSemantic_ThrowsException_PartsOutOfRange(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_Kind;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var skipTags = CaseTag.Invalid_SemPacked;
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
    public void CastTo_TupleKindSemantic_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Packs | CaseTag.KindIn;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new VmVersion(data.Packed);

        var (k, sem) = ((Kind, SemVersion))version;

        Assert.Multiple(
            () => Assert.Equal(data.K, k),
            () => Assert.Equal(data.SemPacked, sem.Packed)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastFrom_TupleKindSemantic_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Kind | CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var sem = new SemVersion(data.SemPacked);

        var version = (VmVersion)(data.K, sem);

        Assert.Equal(data.Packed, version.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CastFrom_TupleKindSemantic_ThrowsException_PartsOutOfRange(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_Kind;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var skipTags = CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var sem = new SemVersion(data.SemPacked);

        var ex = Assert.Throws<PartsOutOfRangeException<Kind>>(() => (VmVersion)(data.K, sem));

        Assert.Multiple(
            () => Assert.Equal(data.K, ex.Kind),
            () => Assert.Equal(sem, ex.Semantic)
        );
    }
}