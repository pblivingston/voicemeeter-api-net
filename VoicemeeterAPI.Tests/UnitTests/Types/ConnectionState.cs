namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types;

public class ConnectionStateTests
{
    [Theory]
    [InlineData(LoginResponse.LoggedOut, RunResponse.NotRunning, App.None, 0, RunResponse.NotRunning)]
    [InlineData(LoginResponse.LoggedOut, RunResponse.Ok, App.Standard, 0x0102_0304, RunResponse.Ok)]
    [InlineData(LoginResponse.LoggedOut, RunResponse.Hidden, App.Banana, 0x0203_0405, RunResponse.Hidden)]
    [InlineData(LoginResponse.LoggedOut, RunResponse.NotResponding, App.Potato, 0x0304_0506, RunResponse.NotResponding)]
    [InlineData(LoginResponse.VoicemeeterNotRunning, RunResponse.NotRunning, App.None, 0, RunResponse.NotRunning)]
    [InlineData(LoginResponse.VoicemeeterNotRunning, RunResponse.NotResponding, App.Standardx64, 0x0102_0304, RunResponse.NotResponding)]
    [InlineData(LoginResponse.Ok, RunResponse.Ok, App.Bananax64, 0x0203_0405, RunResponse.Ok)]
    [InlineData(LoginResponse.Ok, RunResponse.Hidden, App.Potatox64, 0x0304_0506, RunResponse.Hidden)]
    public void ConstructorReturnsExpectedPartsWhenValid(LoginResponse loginStatus, RunResponse vmState, App vmApp, int vmVersion, RunResponse buttonsState)
    {
        ConnectionState state = new(loginStatus, vmState, vmApp, (VmVersion)vmVersion, buttonsState);

        Assert.Multiple(
            () => Assert.Equal(loginStatus, state.LoginStatus),
            () => Assert.Equal(vmState, state.VoicemeeterState),
            () => Assert.Equal(vmApp, state.VoicemeeterApp),
            () => Assert.Equal(vmVersion, (int)state.VoicemeeterVersion),
            () => Assert.Equal(buttonsState, state.MacroButtonsState)
        );
    }

    [Fact]
    public void PackThrowsArgumentOutOfRangeExceptionWhenLoginStatusOutOfRange()
        => Assert.Throws<ArgumentOutOfRangeException>(
            () => ConnectionState.Pack(LoginResponse.Unknown, RunResponse.NotRunning, App.None, default, RunResponse.NotRunning)
        );

    [Fact]
    public void PackThrowsArgumentOutOfRangeExceptionWhenVoicemeeterStateOutOfRange()
        => Assert.Throws<ArgumentOutOfRangeException>(
            () => ConnectionState.Pack(LoginResponse.LoggedOut, RunResponse.NotInstalled, App.None, default, RunResponse.NotRunning)
        );

    [Fact]
    public void PackThrowsArgumentOutOfRangeExceptionWhenVoicemeeterAppOutOfRange()
        => Assert.Throws<ArgumentOutOfRangeException>(
            () => ConnectionState.Pack(LoginResponse.Ok, RunResponse.Ok, App.MacroButtons, VmVersion.MaxValid, RunResponse.NotRunning)
        );

    [Fact]
    public void PackThrowsArgumentOutOfRangeExceptionWhenVoicemeeterVersionOutOfRange()
        => Assert.Throws<ArgumentOutOfRangeException>(
            () => ConnectionState.Pack(LoginResponse.Ok, RunResponse.Hidden, App.Potatox64, VmVersion.MaxValue, RunResponse.NotRunning)
        );

    [Fact]
    public void PackThrowsArgumentOutOfRangeExceptionWhenButtonsStatusOutOfRange()
        => Assert.Throws<ArgumentOutOfRangeException>(
            () => ConnectionState.Pack(LoginResponse.VoicemeeterNotRunning, RunResponse.NotRunning, App.None, default, RunResponse.NotInstalled)
        );

    [Theory]
    [InlineData(LoginResponse.Ok, RunResponse.NotRunning, App.None, 0, RunResponse.Ok)]
    [InlineData(LoginResponse.Ok, RunResponse.NotResponding, App.None, 0, RunResponse.Hidden)]
    [InlineData(LoginResponse.VoicemeeterNotRunning, RunResponse.Ok, App.Standardx64, 0x0102_0304, RunResponse.NotResponding)]
    [InlineData(LoginResponse.VoicemeeterNotRunning, RunResponse.Hidden, App.Potato, 0x0304_0506, RunResponse.NotRunning)]
    public void PackThrowsArgumentExceptionWhenLoginStatusDoesNotMatchVoicemeeterState(LoginResponse loginStatus, RunResponse vmState, App vmApp, int vmVersion, RunResponse buttonsState)
        => Assert.Throws<ArgumentException>(() => ConnectionState.Pack(loginStatus, vmState, vmApp, (VmVersion)vmVersion, buttonsState));

    [Theory]
    [InlineData(LoginResponse.Ok, RunResponse.Ok, App.Standard, 0x0203_0405, RunResponse.Hidden)]
    [InlineData(LoginResponse.Ok, RunResponse.Hidden, App.Bananax64, 0x0304_0506, RunResponse.NotRunning)]
    public void PackThrowsArgumentExceptionWhenVoicemeeterAppDoesNotMatchVoicemeeterVersion(LoginResponse loginStatus, RunResponse vmState, App vmApp, int vmVersion, RunResponse buttonsState)
        => Assert.Throws<ArgumentException>(() => ConnectionState.Pack(loginStatus, vmState, vmApp, (VmVersion)vmVersion, buttonsState));
}
