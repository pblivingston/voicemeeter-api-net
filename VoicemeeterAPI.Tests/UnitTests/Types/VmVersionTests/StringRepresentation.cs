using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VersionData;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VmVersionTests;

public class StringRepresentationTests
{
    [Theory]
    [ClassData(typeof(VersionData))]
    public void ToString_ReturnsExpected_String(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Packed | CaseTag.Invalid_String;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new VmVersion(data.Packed);

        Assert.Equal(data.String, version.ToString());
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryParse_Ints_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.PartsIn;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var badTags = CaseTag.Invalid_String;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var kind = 0; var maj = 0; var min = 0; var pat = 0;
        if (shouldSucceed) { kind = data.Kind; maj = data.Major; min = data.Minor; pat = data.Patch; }

        var success = VmVersion.TryParse(data.String, out int k, out int m, out int n, out int p);

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
    public void Parse_Ints_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.PartsIn | CaseTag.Invalid_String;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        VmVersion.Parse(data.String, out int k, out int m, out int n, out int p);

        Assert.Multiple(
            () => Assert.Equal(data.Kind, k),
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Parse_Ints_ThrowsException_Argument(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_String;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        Assert.Throws<ArgumentException>(() => VmVersion.Parse(data.String, out int _, out int _, out int _, out int _));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryParse_Kind_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.PartsIn;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var badTags = CaseTag.Invalid_String;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        Kind kind = default; var maj = 0; var min = 0; var pat = 0;
        if (shouldSucceed) { kind = data.K; maj = data.Major; min = data.Minor; pat = data.Patch; }

        var success = VmVersion.TryParse(data.String, out Kind k, out int m, out int n, out int p);

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
    public void Parse_Kind_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.PartsIn | CaseTag.Invalid_String;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        VmVersion.Parse(data.String, out Kind k, out int m, out int n, out int p);

        Assert.Multiple(
            () => Assert.Equal(data.K, k),
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Parse_Kind_ThrowsException_Argument(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_String;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        Assert.Throws<ArgumentException>(() => VmVersion.Parse(data.String, out Kind _, out int _, out int _, out int _));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryParse_Semantic_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.KindIn | CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var badTags = CaseTag.Invalid_String;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var kind = 0; int sem = 0;
        if (shouldSucceed) { kind = data.Kind; sem = data.SemPacked; }

        var success = VmVersion.TryParse(data.String, out int k, out SemVersion s);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(kind, k),
            () => Assert.Equal(sem, s.Packed)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Parse_Semantic_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.KindIn | CaseTag.Invalid_SemPacked | CaseTag.Invalid_String;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        VmVersion.Parse(data.String, out int k, out SemVersion s);

        Assert.Multiple(
            () => Assert.Equal(data.Kind, k),
            () => Assert.Equal(data.SemPacked, s.Packed)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Parse_Semantic_ThrowsException_Argument(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_String;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        Assert.Throws<ArgumentException>(() => VmVersion.Parse(data.String, out int _, out SemVersion _));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryParse_KindSemantic_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.KindIn | CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var badTags = CaseTag.Invalid_String;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        Kind kind = default; int sem = 0;
        if (shouldSucceed) { kind = data.K; sem = data.SemPacked; }

        var success = VmVersion.TryParse(data.String, out Kind k, out SemVersion s);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(kind, k),
            () => Assert.Equal(sem, s.Packed)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Parse_KindSemantic_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.KindIn | CaseTag.Invalid_SemPacked | CaseTag.Invalid_String;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        VmVersion.Parse(data.String, out Kind k, out SemVersion s);

        Assert.Multiple(
            () => Assert.Equal(data.K, k),
            () => Assert.Equal(data.SemPacked, s.Packed)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Parse_KindSemantic_ThrowsException_Argument(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_String;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        Assert.Throws<ArgumentException>(() => VmVersion.Parse(data.String, out Kind _, out SemVersion _));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryParse_Packed_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        _ = scenario;

        var badTags = CaseTag.Invalid_String;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var expected = shouldSucceed ? data.Packed : 0x0000_0000;

        var success = VmVersion.TryParse(data.String, out int p);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(expected, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Parse_Packed_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_String;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        VmVersion.Parse(data.String, out int p);

        Assert.Equal(data.Packed, p);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Parse_Packed_ThrowsException_Argument(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_String;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        Assert.Throws<ArgumentException>(() => VmVersion.Parse(data.String, out int _));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryParse_VmVersion_ReturnsExpected_Version(Case scenario, CaseRecord data)
    {
        _ = scenario;

        var badTags = CaseTag.Invalid_String;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var expected = shouldSucceed ? data.Packed : 0x0000_0000;

        var success = VmVersion.TryParse(data.String, out VmVersion v);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(expected, v.Packed)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Parse_VmVersion_ReturnsExpected_Version(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_String;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = VmVersion.Parse(data.String);

        Assert.Equal(data.Packed, version.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Parse_VmVersion_ThrowsException_Argument(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_String;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        Assert.Throws<ArgumentException>(() => VmVersion.Parse(data.String));
    }
}