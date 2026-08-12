namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types;

public class KindTests
{
    [Theory]
    [InlineData(Kind.Standard, true, App.Standardx64)]
    [InlineData(Kind.Potato, false, App.Potato)]
    [InlineData(Kind.None, true, App.None)]
    [InlineData(Kind.Unknown, false, App.Unknown)]
    public void ToAppReturnsExpectedApp(Kind kind, bool is64BitOS, App app)
        => Assert.Equal(app, kind.ToApp(is64BitOS));

    [Theory]
    [InlineData(0, false)]
    [InlineData(3, true)]
    [InlineData(-1, false)]
    [InlineData(5, false)]
    public void IsValidReturnsExpectedBool(int kind, bool valid)
        => Assert.Equal(valid, KindUtils.IsValid(kind));
}
