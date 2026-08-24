namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.GeneralInformation;

public class RefreshConnectionState : MockRemote
{
    [Fact]
    public void UpdatesConnectionStateWhenLoggedIn()
    {
        var loginStatus = LoginResponse.VoicemeeterNotRunning;
        var vmState = RunResponse.NotRunning;
        var vmApp = App.None;
        var response = Response.NoServer;
        VmVersion vmVersion = default;
        var buttonsState = RunResponse.NotRunning;
        var expectedState = new ConnectionState(loginStatus, vmState, vmApp, vmVersion, buttonsState);

        this.MockLogin(RunResponse.Ok, App.Standardx64, VmVersion.MinValid);

        this.MockWrapper.Setup(w => w.GetVoicemeeterKind()).Returns((response, vmVersion.K));
        this.MockWrapper.Setup(w => w.GetVoicemeeterState()).Returns((vmApp, vmState));
        this.MockWrapper.Setup(w => w.GetVoicemeeterVersion()).Returns((response, vmVersion));
        this.MockWrapper.Setup(w => w.GetApplicationState(App.MacroButtons)).Returns(buttonsState);

        var result = this.Remote.RefreshConnectionState();

        Assert.Multiple(
            () => Assert.Equal(expectedState, result),
            () => Assert.Equal(expectedState, this.Remote.ConnectionState),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterKind(), Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterState(), Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(), Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(App.MacroButtons), Times.Exactly(2))
        );
    }

    [Fact]
    public void ThrowsInvalidOperationExceptionWhenUnsupportedVersion()
    {
        var vmState = RunResponse.NotRunning;
        var vmApp = App.None;
        var response = Response.Ok;
        var vmVersion = VmVersion.MaxValue;
        var buttonsState = RunResponse.NotRunning;

        this.MockWrapper.Setup(w => w.GetVoicemeeterKind()).Returns((response, vmVersion.K));
        this.MockWrapper.Setup(w => w.GetVoicemeeterState()).Returns((vmApp, vmState));
        this.MockWrapper.Setup(w => w.GetVoicemeeterVersion()).Returns((response, vmVersion));
        this.MockWrapper.Setup(w => w.GetApplicationState(App.MacroButtons)).Returns(buttonsState);

        Assert.Multiple(
            () => Assert.Throws<InvalidOperationException>(() => this.Remote.RefreshConnectionState()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterKind(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterState(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(App.MacroButtons), Times.Once())
        );
    }
}
