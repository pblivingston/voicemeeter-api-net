namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.Login;

public class Logout : MockRemote
{
    [Fact]
    public void UpdatesLastLoginStatusLoggedOutWhenSuccessful()
    {
        var loginStatus = LoginResponse.LoggedOut;
        var vmState = RunResponse.Ok;
        var vmApp = App.Standardx64;
        var vmVersion = VmVersion.MinValid;
        var buttonsState = RunResponse.NotRunning;
        var expectedState = new ConnectionState(loginStatus, vmState, vmApp, vmVersion, buttonsState);

        this.MockLogin(vmState, vmApp, vmVersion);

        this.MockWrapper.Setup(w => w.Logout()).Returns(LoginResponse.Ok);

        this.Remote.Logout();

        Assert.Multiple(
            () => Assert.Equal(loginStatus, this.Remote.LoginStatus),
            () => Assert.Equal(expectedState, this.Remote.LastConnectionState),
            () => this.MockWrapper.Verify(w => w.Logout(), Times.Once())
        );
    }

    [Fact]
    public void UpdatesLastLoginStatusUnknownWhenLogoutFails()
    {
        var loginStatus = LoginResponse.Unknown;
        var vmState = RunResponse.Hidden;
        var vmApp = App.Standard;
        var vmVersion = VmVersion.MinValid;
        var buttonsState = RunResponse.NotRunning;
        var expectedState = new ConnectionState(loginStatus, vmState, vmApp, vmVersion, buttonsState);

        this.MockWrapper.Setup(w => w.Logout()).Returns(LoginResponse.NoClient);

        this.MockLogin(vmState, vmApp, vmVersion);

        this.Remote.Logout();

        Assert.Multiple(
            () => Assert.Equal(loginStatus, this.Remote.LoginStatus),
            () => Assert.Equal(expectedState, this.Remote.LastConnectionState),
            () => this.MockWrapper.Verify(w => w.Logout(), Times.Once())
        );
    }
}
