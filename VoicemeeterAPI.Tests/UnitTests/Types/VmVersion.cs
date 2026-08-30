namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types;

public class VmVersionTests
{
    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(255, 0x00FF_FFFF, -1)]
    public void PackReturnsExpectedIntWhenAllConditionsMet(int kind, int semPacked, int packed)
        => Assert.Equal(packed, VmVersion.Pack(kind, (SemVersion)semPacked));

    [Fact]
    public void PackThrowsArgumentOutOfRangeExceptionWhenKindDoesNotFitInByte()
        => Assert.Throws<ArgumentOutOfRangeException>(() => VmVersion.Pack(-1, default));

    [Fact]
    public void PackThrowsArgumentOutOfRangeExceptionWhenSemDoesNotFitInThreeBytes()
        => Assert.Throws<ArgumentOutOfRangeException>(() => VmVersion.Pack(0, (SemVersion)(-1)));

    [Fact]
    public void ParseThrowsArgumentExceptionWhenStringHasThreeParts()
        => Assert.Throws<ArgumentException>(() => VmVersion.Parse("2.3.4"));
}
