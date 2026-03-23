using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VersionData;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VmVersionTests;

public class Deconstruction
{
    [Theory]
    [ClassData(typeof(VersionData))]
    public void Deconstructor_Sems_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.SemsIn | CaseTag.Invalid_Packed;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new VmVersion(data.Packed);
        ((IVersion)version).Deconstruct(out int m, out int n, out int p);

        Assert.Multiple(
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Deconstructor_Ints_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.PartsIn | CaseTag.Invalid_Packed;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new VmVersion(data.Packed);
        version.Deconstruct(out int k, out int m, out int n, out int p);

        Assert.Multiple(
            () => Assert.Equal(data.Kind, k),
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Deconstructor_Kind_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.PartsIn | CaseTag.Invalid_Packed;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new VmVersion(data.Packed);
        version.Deconstruct(out Kind k, out int m, out int n, out int p);

        Assert.Multiple(
            () => Assert.Equal(data.K, k),
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Deconstructor_Semantic_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.KindIn | CaseTag.Invalid_Packs;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new VmVersion(data.Packed);
        version.Deconstruct(out int k, out SemVersion sem);

        Assert.Multiple(
            () => Assert.Equal(data.Kind, k),
            () => Assert.Equal(data.SemPacked, sem.Packed)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Deconstructor_KindSemantic_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.KindIn | CaseTag.Invalid_Packs;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new VmVersion(data.Packed);
        version.Deconstruct(out Kind k, out SemVersion sem);

        Assert.Multiple(
            () => Assert.Equal(data.K, k),
            () => Assert.Equal(data.SemPacked, sem.Packed)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Deconstructor_GenericParts_ReturnsExpected_Ints(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.PartsIn | CaseTag.Invalid_Packed;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new VmVersion(data.Packed);

        ((IVersion)version).Deconstruct(out int k, out int m, out int n, out int p);

        Assert.Multiple(
            () => Assert.Equal(data.Kind, k),
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Deconstructor_GenericParts_ReturnsExpected_Kind(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.PartsIn | CaseTag.Invalid_Packed;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new VmVersion(data.Packed);

        ((IVersion)version).Deconstruct(out Kind k, out int m, out int n, out int p);

        Assert.Multiple(
            () => Assert.Equal(data.K, k),
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Deconstructor_GenericSemantic_ReturnsExpected_Semantic(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.KindIn | CaseTag.Invalid_Packs;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new VmVersion(data.Packed);

        ((IVersion)version).Deconstruct(out int k, out SemVersion sem);

        Assert.Multiple(
            () => Assert.Equal(data.Kind, k),
            () => Assert.Equal(data.SemPacked, sem.Packed)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Deconstructor_GenericSemantic_ReturnsExpected_KindSemantic(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.KindIn | CaseTag.Invalid_Packs;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new VmVersion(data.Packed);

        ((IVersion)version).Deconstruct(out Kind k, out SemVersion sem);

        Assert.Multiple(
            () => Assert.Equal(data.K, k),
            () => Assert.Equal(data.SemPacked, sem.Packed)
        );
    }

    [Fact]
    public void Deconstructor_GenericParts_ThrowsException_NotSupported()
    {
        var version = new VmVersion(0x0204_0608);

        Assert.Throws<NotSupportedException>(() => ((IVersion)version).Deconstruct(out bool _, out int _, out int _, out int _));
    }

    [Fact]
    public void Deconstructor_GenericSemantic_ThrowsException_NotSupported()
    {
        var version = new VmVersion(0x0204_0608);

        Assert.Throws<NotSupportedException>(() => ((IVersion)version).Deconstruct(out float _, out SemVersion _));
    }
}