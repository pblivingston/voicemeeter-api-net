namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests;

public class General
{
    internal Mock<Remote.IWrapper> MockWrapper { get; } = new Mock<Remote.IWrapper>();

    [Fact]
    public void UsingNewSessionLogsInAndLogsOut()
    {
        var loginResponse = LoginResponse.Ok;
        var vmState = RunResponse.Hidden;
        var vmApp = App.Standard;
        var versionResponse = Response.Ok;
        var vmVersion = VmVersion.MinValid;
        var buttonsState = RunResponse.NotRunning;
        var expectedState = new ConnectionState(loginResponse, vmState, vmApp, vmVersion, buttonsState);

        this.MockWrapper.Setup(w => w.Login()).Returns(loginResponse);
        this.MockWrapper.Setup(w => w.GetVoicemeeterState()).Returns((vmApp, vmState));
        this.MockWrapper.Setup(w => w.GetVoicemeeterVersion()).Returns((versionResponse, vmVersion));
        this.MockWrapper.Setup(w => w.GetApplicationState(App.MacroButtons)).Returns(buttonsState);
        this.MockWrapper.Setup(w => w.Logout()).Returns(loginResponse);

        ConnectionState state;
        using (var remote = Remote.NewSession(this.MockWrapper.Object))
        {
            state = remote.LastConnectionState;
        }

        Assert.Multiple(
            () => Assert.Equal(expectedState, state),
            () => this.MockWrapper.Verify(w => w.Login(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterState(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(App.MacroButtons), Times.Once()),
            () => this.MockWrapper.Verify(w => w.Logout(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.Dispose(), Times.Once())
        );
    }

    [Fact]
    public void BeginCallScopeThrowsObjectDisposedExceptionWhenObjectDisposed()
    {
        var remote = new Remote(this.MockWrapper.Object);

        remote.Dispose();

        Assert.Multiple(
            () => Assert.Throws<ObjectDisposedException>(() => remote.Login()),
            () => this.MockWrapper.Verify(w => w.Login(), Times.Never())
        );
    }
}
