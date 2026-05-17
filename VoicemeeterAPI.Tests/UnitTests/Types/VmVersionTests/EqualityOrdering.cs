namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VmVersionTests;

using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VersionData;

public class EqualityOrdering
{
    [Theory]
    [ClassData(typeof(VersionData))]
    public void EqualsVersionReturnsExpectedTrue(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidVmPacked | CaseTag.InvalidParts;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var versionA = new VmVersion(data.VmPacked);
        var versionB = new VmVersion(data.Kind, data.Major, data.Minor, data.Patch);

        Assert.True(versionA.Equals(versionB));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void EqualsVersionReturnsExpectedFalse(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidVmPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0106_0102;
        if (data.VmPacked == testPacked)
        {
            testPacked = 0x0109_0901;
        }

        var versionA = new VmVersion(data.VmPacked);
        var versionB = new VmVersion(testPacked);

        Assert.False(versionA.Equals(versionB));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void EqualsObjectReturnsExpectedTrue(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidVmPacked | CaseTag.InvalidParts;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var versionA = new VmVersion(data.VmPacked);
        var versionB = (object)new VmVersion(data.Kind, data.Major, data.Minor, data.Patch);

        Assert.True(versionA.Equals(versionB));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void EqualsObjectReturnsExpectedFalseVal(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidVmPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0106_0102;
        if (data.VmPacked == testPacked)
        {
            testPacked = 0x0109_0901;
        }

        var versionA = new VmVersion(data.VmPacked);
        var versionB = (object)new VmVersion(testPacked);

        Assert.False(versionA.Equals(versionB));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void EqualsObjectReturnsExpectedFalseObj(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidPacks;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var vm = new VmVersion(data.VmPacked);
        var sem = new SemVersion(data.SemPacked);

        Assert.False(vm.Equals(sem));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void GetHashCodeReturnsExpectedPacked(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidVmPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new VmVersion(data.VmPacked);

        Assert.Equal(data.VmPacked, version.GetHashCode());
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CompareToVersionReturnsExpectedInt(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidVmPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0203_0405;
        var expected = data.VmPacked.CompareTo(testPacked);

        var versionA = new VmVersion(data.VmPacked);
        var versionB = new VmVersion(testPacked);

        Assert.Equal(expected, versionA.CompareTo(versionB));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CompareToObjectReturnsExpectedInt(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidVmPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0203_0405;
        var expected = data.VmPacked.CompareTo(testPacked);

        var versionA = new VmVersion(data.VmPacked);
        var versionB = (object)new VmVersion(testPacked);

        Assert.Equal(expected, ((IComparable)versionA).CompareTo(versionB));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CompareToObjectThrowsExceptionArgument(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidPacks;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var vm = new VmVersion(data.VmPacked);
        var sem = new SemVersion(data.SemPacked);

        Assert.Throws<ArgumentException>(() => ((IComparable)vm).CompareTo(sem));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void EqualToReturnsExpectedBool(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidVmPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0203_0405;
        var expected = data.VmPacked == testPacked;

        var versionA = new VmVersion(data.VmPacked);
        var versionB = new VmVersion(testPacked);

        Assert.Equal(expected, versionA == versionB);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void NotEqualToReturnsExpectedBool(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidVmPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0203_0405;
        var expected = data.VmPacked != testPacked;

        var versionA = new VmVersion(data.VmPacked);
        var versionB = new VmVersion(testPacked);

        Assert.Equal(expected, versionA != versionB);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void LessThanReturnsExpectedBool(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidVmPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0203_0405;
        var expected = data.VmPacked < testPacked;

        var versionA = new VmVersion(data.VmPacked);
        var versionB = new VmVersion(testPacked);

        Assert.Equal(expected, versionA < versionB);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void GreaterThanReturnsExpectedBool(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidVmPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0203_0405;
        var expected = data.VmPacked > testPacked;

        var versionA = new VmVersion(data.VmPacked);
        var versionB = new VmVersion(testPacked);

        Assert.Equal(expected, versionA > versionB);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void LessThanOrEqualToReturnsExpectedBool(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidVmPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0203_0405;
        var expected = data.VmPacked <= testPacked;

        var versionA = new VmVersion(data.VmPacked);
        var versionB = new VmVersion(testPacked);

        Assert.Equal(expected, versionA <= versionB);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void GreaterThanOrEqualToReturnsExpectedBool(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidVmPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0203_0405;
        var expected = data.VmPacked >= testPacked;

        var versionA = new VmVersion(data.VmPacked);
        var versionB = new VmVersion(testPacked);

        Assert.Equal(expected, versionA >= versionB);
    }
}
