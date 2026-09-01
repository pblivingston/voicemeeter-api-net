namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types;

public class AppTests
{
    [Theory]
    [InlineData(App.Unknown, Kind.Unknown)]
    [InlineData(App.None, Kind.None)]
    [InlineData(App.Standard, Kind.Standard)]
    [InlineData(App.Potatox64, Kind.Potato)]
    [InlineData(App.MacroButtons, Kind.Unknown)]
    public void ToKindReturnsExpectedKind(App app, Kind kind)
        => Assert.Equal(kind, app.ToKind());

    [Theory]
    [InlineData(-1, false)]
    [InlineData(0, false)]
    [InlineData(5, true)]
    [InlineData(10, true)]
    [InlineData(22, true)]
    [InlineData(8, false)]
    [InlineData(18, false)]
    [InlineData(28, false)]
    public void IsValidReturnsExpectedBool(int app, bool valid)
        => Assert.Equal(valid, ((App)app).IsValid());

    [Theory]
    [InlineData(App.None, false)]
    [InlineData(App.Potato, true)]
    [InlineData(App.Bananax64, true)]
    [InlineData(App.CABLEControlPanel, false)]
    public void IsVoicemeeterReturnsExpectedBool(App app, bool vm)
        => Assert.Equal(vm, app.IsVoicemeeter());
}
