using PBLivingston.VoicemeeterAPI.EventManagement.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VersionData;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VmVersionTests;

public class Packing
{
    [Theory]
    [ClassData(typeof(VersionData))]
    public void RawPack_Parts_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        _ = scenario;

        Assert.Equal(data.Packed, VmVersion.RawPack(data.Kind, data.Major, data.Minor, data.Patch));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryPack_Ints_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        _ = scenario;

        var badTags = CaseTag.Invalid_Parts;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var expected = shouldSucceed ? data.Packed : 0x0000_0000;

        var success = VmVersion.TryPack(data.Kind, data.Major, data.Minor, data.Patch, out int packed);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(expected, packed)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Pack_Ints_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Parts;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        Assert.Equal(data.Packed, VmVersion.Pack(data.Kind, data.Major, data.Minor, data.Patch));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Pack_Ints_ThrowsException_PartsOutOfRange(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_Parts;
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
    public void TryPack_Kind_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        _ = scenario;

        var badTags = CaseTag.Invalid_Parts;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var expected = shouldSucceed ? data.Packed : 0x0000_0000;

        var success = VmVersion.TryPack(data.K, data.Major, data.Minor, data.Patch, out int packed);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(expected, packed)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Pack_Kind_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Parts;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        Assert.Equal(data.Packed, VmVersion.Pack(data.K, data.Major, data.Minor, data.Patch));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Pack_Kind_ThrowsException_PartsOutOfRange(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_Parts;
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
    public void RawPack_Semantic_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var semantic = new SemVersion(data.SemPacked);

        Assert.Equal(data.Packed, VmVersion.RawPack(data.Kind, semantic));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryPack_Semantic_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var badTags = CaseTag.Invalid_Kind;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var expected = shouldSucceed ? data.Packed : 0x0000_0000;

        var semantic = new SemVersion(data.SemPacked);
        var success = VmVersion.TryPack(data.Kind, semantic, out int result);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(expected, result)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Pack_Semantic_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Kind | CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var semantic = new SemVersion(data.SemPacked);

        Assert.Equal(data.Packed, VmVersion.Pack(data.Kind, semantic));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Pack_Semantic_ThrowsException_PartsOutOfRange(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_Kind;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var skipTags = CaseTag.Invalid_SemPacked;
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
    public void TryPack_KindSemantic_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var badTags = CaseTag.Invalid_Kind;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var expected = shouldSucceed ? data.Packed : 0x0000_0000;

        var semantic = new SemVersion(data.SemPacked);
        var success = VmVersion.TryPack(data.K, semantic, out int result);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(expected, result)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Pack_KindSemantic_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Kind | CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var semantic = new SemVersion(data.SemPacked);

        Assert.Equal(data.Packed, VmVersion.Pack(data.K, semantic));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Pack_KindSemantic_ThrowsException_PartsOutOfRange(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_Kind;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var skipTags = CaseTag.Invalid_SemPacked;
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