namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VmVersionTests;

using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VersionData;

public class Packing
{
    [Theory]
    [ClassData(typeof(VersionData))]
    public void RawPackPartsReturnsExpectedPacked(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        Assert.Equal(data.VmPacked, VmVersion.RawPack(data.Kind, data.Major, data.Minor, data.Patch));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryPackIntsReturnsExpectedPacked(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        var badTags = CaseTag.InvalidParts;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var expected = shouldSucceed ? data.VmPacked : 0x0000_0000;

        var success = VmVersion.TryPack(data.Kind, data.Major, data.Minor, data.Patch, out var packed);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(expected, packed)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void PackIntsReturnsExpectedPacked(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidParts;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        Assert.Equal(data.VmPacked, VmVersion.Pack(data.Kind, data.Major, data.Minor, data.Patch));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void PackIntsThrowsExceptionPartsOutOfRange(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidParts;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<PartsOutOfRangeException<int>>(() => VmVersion.Pack(data.Kind, data.Major, data.Minor, data.Patch));

        Assert.Multiple(
            () => Assert.Equal(data.Kind, ex.Kind),
            () => Assert.Equal(data.Major, ex.Major),
            () => Assert.Equal(data.Minor, ex.Minor),
            () => Assert.Equal(data.Patch, ex.Patch),
            () => Assert.Null(ex.Semantic)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryPackKindReturnsExpectedPacked(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        var badTags = CaseTag.InvalidParts;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var expected = shouldSucceed ? data.VmPacked : 0x0000_0000;

        var success = VmVersion.TryPack(data.K, data.Major, data.Minor, data.Patch, out var packed);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(expected, packed)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void PackKindReturnsExpectedPacked(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidParts;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        Assert.Equal(data.VmPacked, VmVersion.Pack(data.K, data.Major, data.Minor, data.Patch));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void PackKindThrowsExceptionPartsOutOfRange(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidParts;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<PartsOutOfRangeException<Kind>>(() => VmVersion.Pack(data.K, data.Major, data.Minor, data.Patch));

        Assert.Multiple(
            () => Assert.Equal(data.K, ex.Kind),
            () => Assert.Equal(data.Major, ex.Major),
            () => Assert.Equal(data.Minor, ex.Minor),
            () => Assert.Equal(data.Patch, ex.Patch),
            () => Assert.Null(ex.Semantic)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void RawPackSemanticReturnsExpectedPacked(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var semantic = new SemVersion(data.SemPacked);

        Assert.Equal(data.VmPacked, VmVersion.RawPack(data.Kind, semantic));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryPackSemanticReturnsExpectedPacked(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var badTags = CaseTag.InvalidKind;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var expected = shouldSucceed ? data.VmPacked : 0x0000_0000;

        var semantic = new SemVersion(data.SemPacked);
        var success = VmVersion.TryPack(data.Kind, semantic, out var result);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(expected, result)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void PackSemanticReturnsExpectedPacked(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidKind | CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var semantic = new SemVersion(data.SemPacked);

        Assert.Equal(data.VmPacked, VmVersion.Pack(data.Kind, semantic));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void PackSemanticThrowsExceptionPartsOutOfRange(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidKind;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var skipTags = CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var semantic = new SemVersion(data.SemPacked);

        var ex = Assert.Throws<PartsOutOfRangeException<int>>(() => VmVersion.Pack(data.Kind, semantic));

        Assert.Multiple(
            () => Assert.Equal(data.Kind, ex.Kind),
            () => Assert.Null(ex.Major),
            () => Assert.Null(ex.Minor),
            () => Assert.Null(ex.Patch),
            () => Assert.Equal(semantic, ex.Semantic)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryPackKindSemanticReturnsExpectedPacked(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var badTags = CaseTag.InvalidKind;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var expected = shouldSucceed ? data.VmPacked : 0x0000_0000;

        var semantic = new SemVersion(data.SemPacked);
        var success = VmVersion.TryPack(data.K, semantic, out var result);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(expected, result)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void PackKindSemanticReturnsExpectedPacked(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidKind | CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var semantic = new SemVersion(data.SemPacked);

        Assert.Equal(data.VmPacked, VmVersion.Pack(data.K, semantic));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void PackKindSemanticThrowsExceptionPartsOutOfRange(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidKind;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var skipTags = CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var semantic = new SemVersion(data.SemPacked);

        var ex = Assert.Throws<PartsOutOfRangeException<Kind>>(() => VmVersion.Pack(data.K, semantic));

        Assert.Multiple(
            () => Assert.Equal(data.K, ex.Kind),
            () => Assert.Null(ex.Major),
            () => Assert.Null(ex.Minor),
            () => Assert.Null(ex.Patch),
            () => Assert.Equal(semantic, ex.Semantic)
        );
    }
}
