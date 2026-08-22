namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.Login;

public class Logout : MockRemote
{
    [Fact]
    public void UpdatesConnectionStateAfterLoggingOut()
    {
        var loginStatus = LoginResponse.LoggedOut;
        var vmState = RunResponse.Ok;
        var vmApp = App.Standardx64;
        var vmVersion = VmVersion.MinValid;
        var buttonsState = RunResponse.NotRunning;
        var expectedState = new ConnectionState(loginStatus, vmState, vmApp, vmVersion, buttonsState);

        this.MockLogin(vmState, vmApp, vmVersion);

        this.MockWrapper.Setup(w => w.Logout()).Returns(LoginResponse.Ok);
        this.MockWrapper.Setup(w => w.GetVoicemeeterKind()).Returns((Response.Error, Kind.None));

        this.Remote.Logout();

        Assert.Multiple(
            () => Assert.Equal(expectedState, this.Remote.ConnectionState),
            () => this.MockWrapper.Verify(w => w.Logout(), Times.Once())
        );
    }
}
