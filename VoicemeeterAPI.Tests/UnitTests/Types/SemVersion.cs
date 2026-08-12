namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types;

public class SemVersionTests
{
    [Fact]
    public void GenericDeconstructThrowsArgumentExceptionWhenTypeNotSupported()
        => Assert.Throws<ArgumentException>(() => ((IVersion)(SemVersion)0x0002_0304).Deconstruct(out float _, out var _, out var _, out var _));

    [Theory]
    [InlineData(0, false)]
    [InlineData(0x0002_0304, true)]
    [InlineData(0x0102_0304, false)]
    public void IsValidReturnsExpectedBool(int packed, bool valid)
        => Assert.Equal(valid, SemVersion.IsValid(packed));

    [Fact]
    public void ParseThrowsArgumentExceptionWhenStringHasFourParts()
        => Assert.Throws<ArgumentException>(() => SemVersion.Parse("1.2.3.4"));
}
