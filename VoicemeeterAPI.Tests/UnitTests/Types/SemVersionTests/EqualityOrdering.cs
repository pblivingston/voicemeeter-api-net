using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VersionData;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.SemVersionTests;

public class EqualityOrdering
{
    [Theory]
    [ClassData(typeof(VersionData))]
    public void Equals_Version_ReturnsExpected_True(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_SemPacked | CaseTag.Invalid_Sems;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var versionA = new SemVersion(data.SemPacked);
        var versionB = new SemVersion(data.Major, data.Minor, data.Patch);

        Assert.True(versionA.Equals(versionB));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Equals_Version_ReturnsExpected_False(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0006_0102;
        if (data.SemPacked == testPacked) { testPacked = 0x0009_0901; }

        var versionA = new SemVersion(data.SemPacked);
        var versionB = new SemVersion(testPacked);

        Assert.False(versionA.Equals(versionB));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Equals_Object_ReturnsExpected_True(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_SemPacked | CaseTag.Invalid_Sems;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var versionA = new SemVersion(data.SemPacked);
        var versionB = (object)new SemVersion(data.Major, data.Minor, data.Patch);

        Assert.True(versionA.Equals(versionB));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Equals_Object_ReturnsExpected_False_Val(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0006_0102;
        if (data.SemPacked == testPacked) { testPacked = 0x0009_0901; }

        var versionA = new SemVersion(data.SemPacked);
        var versionB = (object)new SemVersion(testPacked);

        Assert.False(versionA.Equals(versionB));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void Equals_Object_ReturnsExpected_False_Obj(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Packs;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var sem = new SemVersion(data.SemPacked);
        var vm = new VmVersion(data.Packed);

        Assert.False(sem.Equals(vm));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void GetHashCode_ReturnsExpected_Packed(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new SemVersion(data.SemPacked);

        Assert.Equal(data.SemPacked, version.GetHashCode());
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CompareTo_Version_ReturnsExpected_Int(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0003_0405;
        var expected = data.SemPacked.CompareTo(testPacked);

        var versionA = new SemVersion(data.SemPacked);
        var versionB = new SemVersion(testPacked);

        Assert.Equal(expected, versionA.CompareTo(versionB));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CompareTo_Object_ReturnsExpected_Int(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0003_0405;
        var expected = data.SemPacked.CompareTo(testPacked);

        var versionA = new SemVersion(data.SemPacked);
        var versionB = (object)new SemVersion(testPacked);

        Assert.Equal(expected, ((IComparable)versionA).CompareTo(versionB));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CompareTo_Object_ThrowsException_Argument(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_Packs;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var sem = new SemVersion(data.SemPacked);
        var vm = new VmVersion(data.Packed);

        Assert.Throws<ArgumentException>(() => ((IComparable)sem).CompareTo(vm));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void EqualTo_ReturnsExpected_Bool(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0003_0405;
        var expected = data.SemPacked == testPacked;

        var versionA = new SemVersion(data.SemPacked);
        var versionB = new SemVersion(testPacked);

        Assert.Equal(expected, versionA == versionB);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void NotEqualTo_ReturnsExpected_Bool(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0003_0405;
        var expected = data.SemPacked != testPacked;

        var versionA = new SemVersion(data.SemPacked);
        var versionB = new SemVersion(testPacked);

        Assert.Equal(expected, versionA != versionB);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void LessThan_ReturnsExpected_Bool(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0003_0405;
        var expected = data.SemPacked < testPacked;

        var versionA = new SemVersion(data.SemPacked);
        var versionB = new SemVersion(testPacked);

        Assert.Equal(expected, versionA < versionB);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void GreaterThan_ReturnsExpected_Bool(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0003_0405;
        var expected = data.SemPacked > testPacked;

        var versionA = new SemVersion(data.SemPacked);
        var versionB = new SemVersion(testPacked);

        Assert.Equal(expected, versionA > versionB);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void LessThanOrEqualTo_ReturnsExpected_Bool(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0003_0405;
        var expected = data.SemPacked <= testPacked;

        var versionA = new SemVersion(data.SemPacked);
        var versionB = new SemVersion(testPacked);

        Assert.Equal(expected, versionA <= versionB);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void GreaterThanOrEqualTo_ReturnsExpected_Bool(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_SemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0003_0405;
        var expected = data.SemPacked >= testPacked;

        var versionA = new SemVersion(data.SemPacked);
        var versionB = new SemVersion(testPacked);

        Assert.Equal(expected, versionA >= versionB);
    }
}