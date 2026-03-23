using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VersionData;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.SemVersionTests;

public class Deconstruction
{
    [Theory]
    [ClassData(typeof(VersionData))]
    public void Deconstructor_Sems_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.SemsIn | CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: SemsIn, Invalid_SemPacked");

        var version = new SemVersion(data.SemPacked);
        version.Deconstruct(out int m, out int n, out int p);

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
        var skipTags = CaseTag.SemsIn | CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: SemsIn, Invalid_SemPacked");

        var version = new SemVersion(data.SemPacked);
        ((IVersion)version).Deconstruct(out int k, out int m, out int n, out int p);

        Assert.Multiple(
            () => Assert.Equal(0, k),
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Deconstructor_Kind_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.SemsIn | CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: SemsIn, Invalid_SemPacked");

        var version = new SemVersion(data.SemPacked);
        ((IVersion)version).Deconstruct(out Kind k, out int m, out int n, out int p);

        Assert.Multiple(
            () => Assert.Equal(Kind.None, k),
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Deconstructor_Semantic_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        Assert.SkipWhen(data.Tags.HasAny(CaseTag.Invalid_SemPacked), $"Skipping case: {scenario} with any tags: Invalid_SemPacked");

        var version = new SemVersion(data.SemPacked);
        ((IVersion)version).Deconstruct(out int k, out SemVersion sem);

        Assert.Multiple(
            () => Assert.Equal(0, k),
            () => Assert.Equal(data.SemPacked, sem.Packed)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Deconstructor_KindSemantic_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        Assert.SkipWhen(data.Tags.HasAny(CaseTag.Invalid_SemPacked), $"Skipping case: {scenario} with any tags: Invalid_SemPacked");

        var version = new SemVersion(data.SemPacked);
        ((IVersion)version).Deconstruct(out Kind k, out SemVersion sem);

        Assert.Multiple(
            () => Assert.Equal(Kind.None, k),
            () => Assert.Equal(data.SemPacked, sem.Packed)
        );
    }

    [Fact]
    public void Deconstructor_GenericParts_ThrowsException_NotSupported()
    {
        var version = new SemVersion(0x0004_0608);

        Assert.Throws<NotSupportedException>(() => ((IVersion)version).Deconstruct(out bool _, out int _, out int _, out int _));
    }

    [Fact]
    public void Deconstructor_GenericSemantic_ThrowsException_NotSupported()
    {
        var version = new SemVersion(0x0004_0608);

        Assert.Throws<NotSupportedException>(() => ((IVersion)version).Deconstruct(out float _, out SemVersion _));
    }
}