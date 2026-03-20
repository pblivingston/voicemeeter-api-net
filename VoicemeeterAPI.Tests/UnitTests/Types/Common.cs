using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types;

public class UtilsTests
{
    [Theory]
    [InlineData(0, true)]
    [InlineData(255, true)]
    [InlineData(-1, false)]
    [InlineData(256, false)]
    public void InByte_ReturnsExpected_Bool(int value, bool expected)
    {
        Assert.Equal(expected, value.InByte());
    }
}