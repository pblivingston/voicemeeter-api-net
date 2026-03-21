using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VersionUtilsTests;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.SemVersionTests;

public class Construction
{
    public static TheoryDataRow<Case, CaseRecord>[] GetCaseData => VersionUtilsTests.GetCaseData;

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void Constructor_Packed_ReturnsExpected_Parts(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Sems | CaseTag.Invalid_Packed;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: Invalid_Sems, Invalid_Packed");

        var version = new SemVersion(data.SemPacked);

        Assert.Multiple(
            () => Assert.Equal(data.SemPacked, version.Packed),
            () => Assert.Equal(0, ((IVersion)version).Kind),
            () => Assert.Equal(data.Major, version.Major),
            () => Assert.Equal(data.Minor, version.Minor),
            () => Assert.Equal(data.Patch, version.Patch)
        );
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void Constructor_Packed_ReturnsExpected_Kind(Case scenario, CaseRecord data)
    {
        Assert.SkipWhen(data.Tags.HasAny(CaseTag.Invalid_Packed), $"Skipping case: {scenario} with any tags: Invalid_Packed");

        var version = new SemVersion(data.SemPacked);

        Assert.Equal(Kind.None, ((IVersion)version).K);
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void Constructor_Packed_ReturnsExpected_Semantic(Case scenario, CaseRecord data)
    {
        Assert.SkipWhen(data.Tags.HasAny(CaseTag.Invalid_Packed), $"Skipping case: {scenario} with any tags: Invalid_Packed");

        var version = new SemVersion(data.SemPacked);

        Assert.Equal(data.SemPacked, ((IVersion)version).Semantic.Packed);
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void Constructor_Packed_ThrowsException_Argument(Case scenario, CaseRecord data)
    {
        Assert.SkipUnless(data.Tags.HasAny(CaseTag.Invalid_Packed), $"Skipping case: {scenario} without any tags: Invalid_Packed");

        Assert.Throws<ArgumentOutOfRangeException>(() => new SemVersion(data.SemPacked));
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void Constructor_Sems_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Sems | CaseTag.Invalid_Packed;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: Invalid_Sems, Invalid_Packed");

        var version = new SemVersion(data.Major, data.Minor, data.Patch);

        Assert.Equal(data.SemPacked, version.Packed);
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void Constructor_Sems_ThrowsException_Argument(Case scenario, CaseRecord data)
    {
        Assert.SkipUnless(data.Tags.HasAny(CaseTag.Invalid_Sems), $"Skipping case: {scenario} without any tags: Invalid_Sems");

        Assert.Throws<ArgumentException>(() => new SemVersion(data.Major, data.Minor, data.Patch));
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void Constructor_VmVersion_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        Assert.SkipWhen(data.Tags.HasAny(CaseTag.Invalid_Packed), $"Skipping case: {scenario} with any tags: Invalid_Packed");

        var vm = new VmVersion(data.Packed);
        var version = new SemVersion(vm);

        Assert.Equal(data.SemPacked, version.Packed);
    }
}