namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types;

public class VersionUtilsTests
{
    [Theory]
    [InlineData(-1, 255, 255, 255, 255)]
    [InlineData(0, 0, 0, 0, 0)]
    public void PackReturnsExpectedIntWhenPartsFitInByte(int packed, int kind, int maj, int min, int pat)
        => Assert.Equal(packed, VersionUtils.Pack(kind, maj, min, pat));

    [Theory]
    [InlineData(-1, 0, 0, 0)]
    [InlineData(0, 256, 0, 0)]
    [InlineData(0, 0, -1, 0)]
    [InlineData(0, 0, 0, 256)]
    public void PackThrowsArgumentOutOfRangeExceptionWhenPartDoesNotFitInByte(int kind, int maj, int min, int pat)
        => Assert.Throws<ArgumentOutOfRangeException>(() => VersionUtils.Pack(kind, maj, min, pat));

    [Theory]
    [InlineData("0.0.0.0", 0, 0, 0, 0)]
    [InlineData("-1.-2.-3.-4", -1, -2, -3, -4)]
    [InlineData("0.0.0", null, 0, 0, 0)]
    [InlineData("-2.-3.-4", null, -2, -3, -4)]
    public void ParseReturnsExpectedPartsWhenAllConditionsMet(string s, int? kind, int maj, int min, int pat)
    {
        VersionUtils.Parse(s, out var k, out var m, out var n, out var p);

        Assert.Multiple(
            () => Assert.Equal(kind, k),
            () => Assert.Equal(maj, m),
            () => Assert.Equal(min, n),
            () => Assert.Equal(pat, p)
        );
    }

    [Fact]
    public void ParseThrowsArgumentNullExceptionWhenStringIsNullorWhitespace()
        => Assert.Throws<ArgumentNullException>(() => VersionUtils.Parse(""));

    [Theory]
    [InlineData("NotAVersionString")]
    [InlineData("This.Won't.Work.Either")]
    [InlineData("0xFF.0xFF.0xFF")] // decimal digits only
    [InlineData("1.2")] // too short
    [InlineData("1.2.3.4.5")] // too long
    public void ParseThrowsArgumentExceptionWhenStringIsNotValid(string s)
        => Assert.Throws<ArgumentException>(() => VersionUtils.Parse(s));
}
