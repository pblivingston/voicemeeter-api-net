using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VersionUtilsTests;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VmVersionTests;

public class Packing
{
    public static TheoryDataRow<Case, CaseRecord>[] GetCaseData => VersionUtilsTests.GetCaseData;

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void RawPack_Parts_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        _ = scenario;

        Assert.Equal(data.Packed, VmVersion.RawPack(data.Kind, data.Major, data.Minor, data.Patch));
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void TryPack_Ints_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        _ = scenario;

        var badTags = CaseTag.Invalid_Kind | CaseTag.Invalid_Sems;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var expected = shouldSucceed ? data.Packed : 0x0000_0000;

        var success = VmVersion.TryPack(data.Kind, data.Major, data.Minor, data.Patch, out int packed);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(expected, packed)
        );
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void Pack_Ints_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Kind | CaseTag.Invalid_Sems;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: Invalid_Kind, Invalid_Sems");

        Assert.Equal(data.Packed, VmVersion.Pack(data.Kind, data.Major, data.Minor, data.Patch));
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void Pack_Ints_ThrowsException_Argument(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_Kind | CaseTag.Invalid_Sems;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: Invalid_Kind, Invalid_Sems");

        Assert.Throws<ArgumentException>(() => VmVersion.Pack(data.Kind, data.Major, data.Minor, data.Patch));
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void TryPack_Kind_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        _ = scenario;

        var badTags = CaseTag.Invalid_Kind | CaseTag.Invalid_Sems;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var expected = shouldSucceed ? data.Packed : 0x0000_0000;

        var success = VmVersion.TryPack(data.K, data.Major, data.Minor, data.Patch, out int packed);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(expected, packed)
        );
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void Pack_Kind_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Kind | CaseTag.Invalid_Sems;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: Invalid_Kind, Invalid_Sems");

        Assert.Equal(data.Packed, VmVersion.Pack(data.K, data.Major, data.Minor, data.Patch));
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void Pack_Kind_ThrowsException_Argument(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_Kind | CaseTag.Invalid_Sems;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: Invalid_Kind, Invalid_Sems");

        Assert.Throws<ArgumentException>(() => VmVersion.Pack(data.K, data.Major, data.Minor, data.Patch));
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void RawPack_Semantic_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Packed | CaseTag.KindSpill;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: Invalid_Packed, KindSpill");

        var semantic = new SemVersion(data.SemPacked);

        Assert.Equal(data.Packed, VmVersion.RawPack(data.Kind, semantic));
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void TryPack_Semantic_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Packed | CaseTag.KindSpill;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: Invalid_Packed, KindSpill");

        var badTags = CaseTag.Invalid_Kind;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var expected = shouldSucceed ? data.Packed : 0x0000_0000;

        var semantic = new SemVersion(data.SemPacked);
        var success = VmVersion.TryPack(data.Kind, semantic, out int result);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(data.Packed, result)
        );
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void Pack_Semantic_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Kind | CaseTag.Invalid_Packed | CaseTag.KindSpill;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: Invalid_Kind, Invalid_Sems, KindSpill");

        var semantic = new SemVersion(data.SemPacked);

        Assert.Equal(data.Packed, VmVersion.Pack(data.Kind, semantic));
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void Pack_Semantic_ThrowsException_Argument(Case scenario, CaseRecord data)
    {
        Assert.SkipUnless(data.Tags.HasAny(CaseTag.Invalid_Kind), $"Skipping case: {scenario} without any tags: Invalid_Kind");
        Assert.SkipWhen(data.Tags.HasAny(CaseTag.Invalid_Packed), $"Skipping case: {scenario} with any tags: Invalid_Packed");

        var semantic = new SemVersion(data.SemPacked);

        Assert.Throws<ArgumentException>(() => VmVersion.Pack(data.Kind, semantic));
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void TryPack_KindSemantic_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Packed | CaseTag.KindSpill;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: Invalid_Packed, KindSpill");

        var badTags = CaseTag.Invalid_Kind;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var expected = shouldSucceed ? data.Packed : 0x0000_0000;

        var semantic = new SemVersion(data.SemPacked);
        var success = VmVersion.TryPack(data.K, semantic, out int result);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(data.Packed, result)
        );
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void Pack_KindSemantic_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Kind | CaseTag.Invalid_Packed | CaseTag.KindSpill;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: Invalid_Kind, Invalid_Sems, KindSpill");

        var semantic = new SemVersion(data.SemPacked);

        Assert.Equal(data.Packed, VmVersion.Pack(data.K, semantic));
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void Pack_KindSemantic_ThrowsException_Argument(Case scenario, CaseRecord data)
    {
        Assert.SkipUnless(data.Tags.HasAny(CaseTag.Invalid_Kind), $"Skipping case: {scenario} without any tags: Invalid_Kind");
        Assert.SkipWhen(data.Tags.HasAny(CaseTag.Invalid_Packed), $"Skipping case: {scenario} with any tags: Invalid_Packed");

        var semantic = new SemVersion(data.SemPacked);

        Assert.Throws<ArgumentException>(() => VmVersion.Pack(data.K, semantic));
    }
}