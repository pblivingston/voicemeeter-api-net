using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VersionData;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VmVersionTests;

public class EqualityOrdering
{
    [Theory]
    [ClassData(typeof(VersionData))]
    public void Equals_Version_ReturnsExpected_True(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Packed | CaseTag.Invalid_Parts;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var versionA = new VmVersion(data.Packed);
        var versionB = new VmVersion(data.Kind, data.Major, data.Minor, data.Patch);

        Assert.True(versionA.Equals(versionB));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Equals_Version_ReturnsExpected_False(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Packed;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0106_0102;
        if (data.Packed == testPacked) { testPacked = 0x0109_0901; }

        var versionA = new VmVersion(data.Packed);
        var versionB = new VmVersion(testPacked);

        Assert.False(versionA.Equals(versionB));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Equals_Object_ReturnsExpected_True(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Packed | CaseTag.Invalid_Parts;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var versionA = new VmVersion(data.Packed);
        var versionB = (object)new VmVersion(data.Kind, data.Major, data.Minor, data.Patch);

        Assert.True(versionA.Equals(versionB));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Equals_Object_ReturnsExpected_False_Val(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Packed;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0106_0102;
        if (data.Packed == testPacked) { testPacked = 0x0109_0901; }

        var versionA = new VmVersion(data.Packed);
        var versionB = (object)new VmVersion(testPacked);

        Assert.False(versionA.Equals(versionB));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Equals_Object_ReturnsExpected_False_Obj(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Packs;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var vm = new VmVersion(data.Packed);
        var sem = new SemVersion(data.SemPacked);

        Assert.False(vm.Equals(sem));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void GetHashCode_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Packed;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new VmVersion(data.Packed);

        Assert.Equal(data.Packed, version.GetHashCode());
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CompareTo_Version_ReturnsExpected_Int(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Packed;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0203_0405;
        var expected = data.Packed.CompareTo(testPacked);

        var versionA = new VmVersion(data.Packed);
        var versionB = new VmVersion(testPacked);

        Assert.Equal(expected, versionA.CompareTo(versionB));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CompareTo_Object_ReturnsExpected_Int(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Packed;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0203_0405;
        var expected = data.Packed.CompareTo(testPacked);

        var versionA = new VmVersion(data.Packed);
        var versionB = (object)new VmVersion(testPacked);

        Assert.Equal(expected, ((IComparable)versionA).CompareTo(versionB));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CompareTo_Object_ThrowsException_Argument(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Packs;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var vm = new VmVersion(data.Packed);
        var sem = new SemVersion(data.SemPacked);

        Assert.Throws<ArgumentException>(() => ((IComparable)vm).CompareTo(sem));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void EqualTo_ReturnsExpected_Bool(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Packed;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0203_0405;
        var expected = data.Packed == testPacked;

        var versionA = new VmVersion(data.Packed);
        var versionB = new VmVersion(testPacked);

        Assert.Equal(expected, versionA == versionB);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void NotEqualTo_ReturnsExpected_Bool(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Packed;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0203_0405;
        var expected = data.Packed != testPacked;

        var versionA = new VmVersion(data.Packed);
        var versionB = new VmVersion(testPacked);

        Assert.Equal(expected, versionA != versionB);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void LessThan_ReturnsExpected_Bool(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Packed;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0203_0405;
        var expected = data.Packed < testPacked;

        var versionA = new VmVersion(data.Packed);
        var versionB = new VmVersion(testPacked);

        Assert.Equal(expected, versionA < versionB);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void GreaterThan_ReturnsExpected_Bool(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Packed;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0203_0405;
        var expected = data.Packed > testPacked;

        var versionA = new VmVersion(data.Packed);
        var versionB = new VmVersion(testPacked);

        Assert.Equal(expected, versionA > versionB);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void LessThanOrEqualTo_ReturnsExpected_Bool(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Packed;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0203_0405;
        var expected = data.Packed <= testPacked;

        var versionA = new VmVersion(data.Packed);
        var versionB = new VmVersion(testPacked);

        Assert.Equal(expected, versionA <= versionB);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void GreaterThanOrEqualTo_ReturnsExpected_Bool(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Packed;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0203_0405;
        var expected = data.Packed >= testPacked;

        var versionA = new VmVersion(data.Packed);
        var versionB = new VmVersion(testPacked);

        Assert.Equal(expected, versionA >= versionB);
    }
}