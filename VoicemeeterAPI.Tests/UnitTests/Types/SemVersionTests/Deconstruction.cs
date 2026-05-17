namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.SemVersionTests;

using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;
using PBLivingston.VoicemeeterAPI.Utilities;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VersionData;

public class Deconstruction
{
    [Theory]
    [ClassData(typeof(VersionData))]
    public void DeconstructorSemsReturnsExpectedParts(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.SemsIn | CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new SemVersion(data.SemPacked);
        version.Deconstruct(out var m, out var n, out var p);

        Assert.Multiple(
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void DeconstructorIntsReturnsExpectedParts(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.SemsIn | CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new SemVersion(data.SemPacked);
        ((IVersion)version).Deconstruct<int>(out var k, out var m, out var n, out var p);

        Assert.Multiple(
            () => Assert.Equal(0, k),
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void DeconstructorKindReturnsExpectedParts(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.SemsIn | CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new SemVersion(data.SemPacked);
        ((IVersion)version).Deconstruct<Kind>(out var k, out var m, out var n, out var p);

        Assert.Multiple(
            () => Assert.Equal(Kind.None, k),
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void DeconstructorSemanticReturnsExpectedParts(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new SemVersion(data.SemPacked);
        ((IVersion)version).Deconstruct<int>(out var k, out var sem);

        Assert.Multiple(
            () => Assert.Equal(0, k),
            () => Assert.Equal(data.SemPacked, sem.Packed)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void DeconstructorKindSemanticReturnsExpectedParts(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new SemVersion(data.SemPacked);
        ((IVersion)version).Deconstruct<Kind>(out var k, out var sem);

        Assert.Multiple(
            () => Assert.Equal(Kind.None, k),
            () => Assert.Equal(data.SemPacked, sem.Packed)
        );
    }

    [Fact]
    public void DeconstructorGenericPartsThrowsExceptionTypeNotSupported()
    {
        var version = new SemVersion(0x0004_0608);

        var ex = Assert.Throws<TypeNotSupportedException>(() => ((IVersion)version).Deconstruct<bool>(out var _, out var _, out var _, out var _));

        Assert.Multiple(
            () => Assert.Equal(typeof(bool), ex.Type),
            () => Assert.Equal(SupportedTypes.KindTypes, ex.SupportedTypes)
        );
    }

    [Fact]
    public void DeconstructorGenericSemanticThrowsExceptionTypeNotSupported()
    {
        var version = new SemVersion(0x0004_0608);

        var ex = Assert.Throws<TypeNotSupportedException>(() => ((IVersion)version).Deconstruct<float>(out var _, out var _));

        Assert.Multiple(
            () => Assert.Equal(typeof(float), ex.Type),
            () => Assert.Equal(SupportedTypes.KindTypes, ex.SupportedTypes)
        );
    }
}