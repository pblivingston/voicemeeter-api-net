namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.SemVersionTests;

using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.VersionData;

public class EqualityOrdering
{
    [Theory]
    [ClassData(typeof(VersionData))]
    public void EqualsVersionReturnsExpectedTrue(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidSemPacked | CaseTag.InvalidSems;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var versionA = new SemVersion(data.SemPacked);
        var versionB = new SemVersion(data.Major, data.Minor, data.Patch);

        Assert.True(versionA.Equals(versionB));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void EqualsVersionReturnsExpectedFalse(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0006_0102;
        if (data.SemPacked == testPacked)
        {
            testPacked = 0x0009_0901;
        }

        var versionA = new SemVersion(data.SemPacked);
        var versionB = new SemVersion(testPacked);

        Assert.False(versionA.Equals(versionB));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void EqualsObjectReturnsExpectedTrue(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidSemPacked | CaseTag.InvalidSems;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var versionA = new SemVersion(data.SemPacked);
        var versionB = (object)new SemVersion(data.Major, data.Minor, data.Patch);

        Assert.True(versionA.Equals(versionB));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void EqualsObjectReturnsExpectedFalseVal(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0006_0102;
        if (data.SemPacked == testPacked)
        {
            testPacked = 0x0009_0901;
        }

        var versionA = new SemVersion(data.SemPacked);
        var versionB = (object)new SemVersion(testPacked);

        Assert.False(versionA.Equals(versionB));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void EqualsObjectReturnsExpectedFalseObj(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidPacks;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var sem = new SemVersion(data.SemPacked);
        var vm = new VmVersion(data.VmPacked);

        Assert.False(sem.Equals(vm));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void GetHashCodeReturnsExpectedPacked(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var version = new SemVersion(data.SemPacked);

        Assert.Equal(data.SemPacked, version.GetHashCode());
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CompareToVersionReturnsExpectedInt(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0003_0405;
        var expected = data.SemPacked.CompareTo(testPacked);

        var versionA = new SemVersion(data.SemPacked);
        var versionB = new SemVersion(testPacked);

        Assert.Equal(expected, versionA.CompareTo(versionB));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CompareToObjectReturnsExpectedInt(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0003_0405;
        var expected = data.SemPacked.CompareTo(testPacked);

        var versionA = new SemVersion(data.SemPacked);
        var versionB = (object)new SemVersion(testPacked);

        Assert.Equal(expected, ((IComparable)versionA).CompareTo(versionB));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void CompareToObjectThrowsExceptionArgument(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidPacks;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var sem = new SemVersion(data.SemPacked);
        var vm = new VmVersion(data.VmPacked);

        Assert.Throws<ArgumentException>(() => ((IComparable)sem).CompareTo(vm));
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void EqualToReturnsExpectedBool(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0003_0405;
        var expected = data.SemPacked == testPacked;

        var versionA = new SemVersion(data.SemPacked);
        var versionB = new SemVersion(testPacked);

        Assert.Equal(expected, versionA == versionB);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void NotEqualToReturnsExpectedBool(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0003_0405;
        var expected = data.SemPacked != testPacked;

        var versionA = new SemVersion(data.SemPacked);
        var versionB = new SemVersion(testPacked);

        Assert.Equal(expected, versionA != versionB);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void LessThanReturnsExpectedBool(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0003_0405;
        var expected = data.SemPacked < testPacked;

        var versionA = new SemVersion(data.SemPacked);
        var versionB = new SemVersion(testPacked);

        Assert.Equal(expected, versionA < versionB);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void GreaterThanReturnsExpectedBool(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0003_0405;
        var expected = data.SemPacked > testPacked;

        var versionA = new SemVersion(data.SemPacked);
        var versionB = new SemVersion(testPacked);

        Assert.Equal(expected, versionA > versionB);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void LessThanOrEqualToReturnsExpectedBool(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0003_0405;
        var expected = data.SemPacked <= testPacked;

        var versionA = new SemVersion(data.SemPacked);
        var versionB = new SemVersion(testPacked);

        Assert.Equal(expected, versionA <= versionB);
    }

    [Theory]
    [ClassData(typeof(VersionData))]
    public void GreaterThanOrEqualToReturnsExpectedBool(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidSemPacked;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var testPacked = 0x0003_0405;
        var expected = data.SemPacked >= testPacked;

        var versionA = new SemVersion(data.SemPacked);
        var versionB = new SemVersion(testPacked);

        Assert.Equal(expected, versionA >= versionB);
    }
}