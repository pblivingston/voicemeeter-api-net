namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VmVersionTests;

using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VersionData;

public class StringRepresentation
{
    [Theory]
    [ClassData(typeof(VersionData))]
    public void ToStringReturnsExpectedString(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidVmPacked | CaseTag.InvalidVmString;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new VmVersion(data.VmPacked);

        Assert.Equal(data.VmString, version.ToString());
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryParseIntsReturnsExpectedParts(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.PartsIn;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var badTags = CaseTag.InvalidVmString;
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

        var success = VmVersion.TryParse(data.VmString, out int k, out var m, out var n, out var p);

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
    public void ParseIntsReturnsExpectedParts(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.PartsIn | CaseTag.InvalidVmString;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        VmVersion.Parse(data.VmString, out int k, out var m, out var n, out var p);

        Assert.Multiple(
            () => Assert.Equal(data.Kind, k),
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ParseIntsThrowsExceptionCannotParseAsParts(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidVmString;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<CannotParseAsPartsException>(() => VmVersion.Parse(data.VmString, out int _, out var _, out var _, out var _));

        Assert.Equal(data.VmString, ex.ActualValue);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryParseKindReturnsExpectedParts(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.PartsIn;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var badTags = CaseTag.InvalidVmString;
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

        var success = VmVersion.TryParse(data.VmString, out Kind k, out var m, out var n, out var p);

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
    public void ParseKindReturnsExpectedParts(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.PartsIn | CaseTag.InvalidVmString;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        VmVersion.Parse(data.VmString, out Kind k, out var m, out var n, out var p);

        Assert.Multiple(
            () => Assert.Equal(data.K, k),
            () => Assert.Equal(data.Major, m),
            () => Assert.Equal(data.Minor, n),
            () => Assert.Equal(data.Patch, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ParseKindThrowsExceptionCannotParseAsParts(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidVmString;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<CannotParseAsPartsException>(() => VmVersion.Parse(data.VmString, out Kind _, out var _, out var _, out var _));

        Assert.Equal(data.VmString, ex.ActualValue);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryParseSemanticReturnsExpectedParts(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.KindIn | CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var badTags = CaseTag.InvalidVmString;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var kind = 0;
        var sem = 0;
        if (shouldSucceed)
        {
            kind = data.Kind;
            sem = data.SemPacked;
        }

        var success = VmVersion.TryParse(data.VmString, out int k, out var s);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(kind, k),
            () => Assert.Equal(sem, s.Packed)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ParseSemanticReturnsExpectedParts(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.KindIn | CaseTag.InvalidSemPacked | CaseTag.InvalidVmString;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        VmVersion.Parse(data.VmString, out int k, out var s);

        Assert.Multiple(
            () => Assert.Equal(data.Kind, k),
            () => Assert.Equal(data.SemPacked, s.Packed)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ParseSemanticThrowsExceptionCannotParseAsParts(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidVmString;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<CannotParseAsPartsException>(() => VmVersion.Parse(data.VmString, out int _, out var _));

        Assert.Equal(data.VmString, ex.ActualValue);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryParseKindSemanticReturnsExpectedParts(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.KindIn | CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var badTags = CaseTag.InvalidVmString;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        Kind kind = default;
        var sem = 0;
        if (shouldSucceed)
        {
            kind = data.K;
            sem = data.SemPacked;
        }

        var success = VmVersion.TryParse(data.VmString, out Kind k, out var s);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(kind, k),
            () => Assert.Equal(sem, s.Packed)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ParseKindSemanticReturnsExpectedParts(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.KindIn | CaseTag.InvalidSemPacked | CaseTag.InvalidVmString;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        VmVersion.Parse(data.VmString, out Kind k, out var s);

        Assert.Multiple(
            () => Assert.Equal(data.K, k),
            () => Assert.Equal(data.SemPacked, s.Packed)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ParseKindSemanticThrowsExceptionCannotParseAsParts(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidVmString;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<CannotParseAsPartsException>(() => VmVersion.Parse(data.VmString, out Kind _, out var _));

        Assert.Equal(data.VmString, ex.ActualValue);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryParsePackedReturnsExpectedPacked(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        var badTags = CaseTag.InvalidVmString;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var expected = shouldSucceed ? data.VmPacked : 0x0000_0000;

        var success = VmVersion.TryParse(data.VmString, out int p);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(expected, p)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ParsePackedReturnsExpectedPacked(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidVmString;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        VmVersion.Parse(data.VmString, out var p);

        Assert.Equal(data.VmPacked, p);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ParsePackedThrowsExceptionCannotParseAsParts(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidVmString;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<CannotParseAsPartsException>(() => VmVersion.Parse(data.VmString, out var _));

        Assert.Equal(data.VmString, ex.ActualValue);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void TryParseVmVersionReturnsExpectedVersion(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        var badTags = CaseTag.InvalidVmString;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var expected = shouldSucceed ? data.VmPacked : 0x0000_0000;

        var success = VmVersion.TryParse(data.VmString, out VmVersion v);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(expected, v.Packed)
        );
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ParseVmVersionReturnsExpectedVersion(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidVmString;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = VmVersion.Parse(data.VmString);

        Assert.Equal(data.VmPacked, version.Packed);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void ParseVmVersionThrowsExceptionArgument(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidVmString;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<CannotParseAsTypeException>(() => VmVersion.Parse(data.VmString));

        Assert.Multiple(
            () => Assert.Equal(data.VmString, ex.ActualValue),
            () => Assert.Equal(typeof(VmVersion), ex.Type)
        );
    }
}
