namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types;

public class VmVersionTests
{
    [Fact]
    public void GenericDeconstructThrowsArgumentExceptionWhenTypeNotSupported()
        => Assert.Throws<ArgumentException>(() => ((IVersion)(VmVersion)0x0102_0304).Deconstruct(out App _, out var _));

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(255, 0x00FF_FFFF, 0xFFFF_FFFF)]
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
