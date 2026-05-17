namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VmVersionTests;

using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VersionData;

public class Unpacking
{
    [Theory]
    [ClassData(typeof(VersionData))]
    public void RawUnpackPartsReturnsExpectedParts(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.PartsIn;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        VmVersion.RawUnpack(data.VmPacked, out var k, out var m, out var n, out var p);

        Assert.Multiple(
            () => Assert.Equal(data.Kind, k),
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryUnpackIntsReturnsExpectedParts(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.PartsIn;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var badTags = CaseTag.InvalidVmPacked;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var kind = 0;
        var maj = 0;
        var min = 0;
        var pat = 0;
        if (shouldSucceed)
        {
            kind = data.Kind;
            maj = data.Major;
            min = data.Minor;
            pat = data.Patch;
        }

        var success = VmVersion.TryUnpack(data.VmPacked, out int k, out var m, out var n, out var p);

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
    public void UnpackIntsReturnsExpectedParts(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.PartsIn | CaseTag.InvalidVmPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        VmVersion.Unpack(data.VmPacked, out int k, out var m, out var n, out var p);

        Assert.Multiple(
            () => Assert.Equal(data.Kind, k),
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void UnpackIntsThrowsExceptionVmPackedOutOfRange(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidVmPacked;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<VmPackedOutOfRangeException>(() => VmVersion.Unpack(data.VmPacked, out int _, out var _, out var _, out var _));

        Assert.Equal(data.VmPacked, ex.ActualValue);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryUnpackKindReturnsExpectedParts(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.PartsIn;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var badTags = CaseTag.InvalidVmPacked;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        Kind kind = default;
        var maj = 0;
        var min = 0;
        var pat = 0;
        if (shouldSucceed)
        {
            kind = data.K;
            maj = data.Major;
            min = data.Minor;
            pat = data.Patch;
        }

        var success = VmVersion.TryUnpack(data.VmPacked, out Kind k, out var m, out var n, out var p);

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
    public void UnpackKindReturnsExpectedParts(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.PartsIn | CaseTag.InvalidVmPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        VmVersion.Unpack(data.VmPacked, out Kind k, out var m, out var n, out var p);

        Assert.Multiple(
            () => Assert.Equal(data.K, k),
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void UnpackKindThrowsExceptionVmPackedOutOfRange(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidVmPacked;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<VmPackedOutOfRangeException>(() => VmVersion.Unpack(data.VmPacked, out Kind _, out var _, out var _, out var _));

        Assert.Equal(data.VmPacked, ex.ActualValue);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void RawUnpackSemanticReturnsExpectedParts(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.KindIn | CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        VmVersion.RawUnpack(data.VmPacked, out var k, out var s);

        Assert.Multiple(
            () => Assert.Equal(data.Kind, k),
            () => Assert.Equal(data.SemPacked, s.Packed)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryUnpackSemanticReturnsExpectedParts(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.KindIn | CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var badTags = CaseTag.InvalidVmPacked;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var kind = 0;
        var sem = 0;
        if (shouldSucceed)
        {
            kind = data.Kind;
            sem = data.SemPacked;
        }

        var success = VmVersion.TryUnpack(data.VmPacked, out int k, out var s);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(kind, k),
            () => Assert.Equal(sem, s.Packed)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void UnpackSemanticReturnsExpectedParts(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.KindIn | CaseTag.InvalidPacks;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        VmVersion.Unpack(data.VmPacked, out int k, out var s);

        Assert.Multiple(
            () => Assert.Equal(data.Kind, k),
            () => Assert.Equal(data.SemPacked, s.Packed)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void UnpackSemanticThrowsExceptionVmPackedOutOfRange(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidVmPacked;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<VmPackedOutOfRangeException>(() => VmVersion.Unpack(data.VmPacked, out int _, out var _));

        Assert.Equal(data.VmPacked, ex.ActualValue);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryUnpackKindSemanticReturnsExpectedParts(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.KindIn | CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var badTags = CaseTag.InvalidVmPacked;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        Kind kind = default;
        var sem = 0;
        if (shouldSucceed)
        {
            kind = data.K;
            sem = data.SemPacked;
        }

        var success = VmVersion.TryUnpack(data.VmPacked, out Kind k, out var s);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(kind, k),
            () => Assert.Equal(sem, s.Packed)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void UnpackKindSemanticReturnsExpectedParts(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.KindIn | CaseTag.InvalidPacks;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        VmVersion.Unpack(data.VmPacked, out Kind k, out var s);

        Assert.Multiple(
            () => Assert.Equal(data.K, k),
            () => Assert.Equal(data.SemPacked, s.Packed)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void UnpackKindSemanticThrowsExceptionVmPackedOutOfRange(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidVmPacked;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<VmPackedOutOfRangeException>(() => VmVersion.Unpack(data.VmPacked, out Kind _, out var _));

        Assert.Equal(data.VmPacked, ex.ActualValue);
    }
}
