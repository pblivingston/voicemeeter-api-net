using PBLivingston.VoicemeeterAPI.EventManagement.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VersionData;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VmVersionTests;

public class Unpacking
{
    [Theory]
    [ClassData(typeof(VersionData))]
    public void RawUnpack_Parts_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.PartsIn;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        VmVersion.RawUnpack(data.Packed, out int k, out int m, out int n, out int p);

        Assert.Multiple(
            () => Assert.Equal(data.Kind, k),
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryUnpack_Ints_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.PartsIn;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var badTags = CaseTag.Invalid_Packed;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var kind = 0; var maj = 0; var min = 0; var pat = 0;
        if (shouldSucceed) { kind = data.Kind; maj = data.Major; min = data.Minor; pat = data.Patch; }

        var success = VmVersion.TryUnpack(data.Packed, out int k, out int m, out int n, out int p);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(kind, k),
            () => Assert.Equal(maj, m),
            () => Assert.Equal(min, n),
            () => Assert.Equal(pat, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Unpack_Ints_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.PartsIn | CaseTag.Invalid_Packed;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        VmVersion.Unpack(data.Packed, out int k, out int m, out int n, out int p);

        Assert.Multiple(
            () => Assert.Equal(data.Kind, k),
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Unpack_Ints_ThrowsException_VmPackedOutOfRange(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_Packed;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<VmPackedOutOfRangeException>(() => VmVersion.Unpack(data.Packed, out int _, out int _, out int _, out int _));

        Assert.Equal(data.Packed, ex.ActualValue);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryUnpack_Kind_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.PartsIn;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var badTags = CaseTag.Invalid_Packed;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        Kind kind = default; var maj = 0; var min = 0; var pat = 0;
        if (shouldSucceed) { kind = data.K; maj = data.Major; min = data.Minor; pat = data.Patch; }

        var success = VmVersion.TryUnpack(data.Packed, out Kind k, out int m, out int n, out int p);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(kind, k),
            () => Assert.Equal(maj, m),
            () => Assert.Equal(min, n),
            () => Assert.Equal(pat, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Unpack_Kind_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.PartsIn | CaseTag.Invalid_Packed;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        VmVersion.Unpack(data.Packed, out Kind k, out int m, out int n, out int p);

        Assert.Multiple(
            () => Assert.Equal(data.K, k),
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Unpack_Kind_ThrowsException_VmPackedOutOfRange(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_Packed;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<VmPackedOutOfRangeException>(() => VmVersion.Unpack(data.Packed, out Kind _, out int _, out int _, out int _));

        Assert.Equal(data.Packed, ex.ActualValue);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void RawUnpack_Semantic_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.KindIn | CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        VmVersion.RawUnpack(data.Packed, out int k, out SemVersion s);

        Assert.Multiple(
            () => Assert.Equal(data.Kind, k),
            () => Assert.Equal(data.SemPacked, s.Packed)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryUnpack_Semantic_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.KindIn | CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var badTags = CaseTag.Invalid_Packed;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var kind = 0; int sem = 0;
        if (shouldSucceed) { kind = data.Kind; sem = data.SemPacked; }

        var success = VmVersion.TryUnpack(data.Packed, out int k, out SemVersion s);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(kind, k),
            () => Assert.Equal(sem, s.Packed)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Unpack_Semantic_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.KindIn | CaseTag.Invalid_Packs;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        VmVersion.Unpack(data.Packed, out int k, out SemVersion s);

        Assert.Multiple(
            () => Assert.Equal(data.Kind, k),
            () => Assert.Equal(data.SemPacked, s.Packed)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Unpack_Semantic_ThrowsException_VmPackedOutOfRange(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_Packed;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<VmPackedOutOfRangeException>(() => VmVersion.Unpack(data.Packed, out int _, out SemVersion _));

        Assert.Equal(data.Packed, ex.ActualValue);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryUnpack_KindSemantic_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.KindIn | CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var badTags = CaseTag.Invalid_Packed;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        Kind kind = default; int sem = 0;
        if (shouldSucceed) { kind = data.K; sem = data.SemPacked; }

        var success = VmVersion.TryUnpack(data.Packed, out Kind k, out SemVersion s);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(kind, k),
            () => Assert.Equal(sem, s.Packed)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Unpack_KindSemantic_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.KindIn | CaseTag.Invalid_Packs;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        VmVersion.Unpack(data.Packed, out Kind k, out SemVersion s);

        Assert.Multiple(
            () => Assert.Equal(data.K, k),
            () => Assert.Equal(data.SemPacked, s.Packed)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Unpack_KindSemantic_ThrowsException_VmPackedOutOfRange(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_Packed;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<VmPackedOutOfRangeException>(() => VmVersion.Unpack(data.Packed, out Kind _, out SemVersion _));

        Assert.Equal(data.Packed, ex.ActualValue);
    }
}