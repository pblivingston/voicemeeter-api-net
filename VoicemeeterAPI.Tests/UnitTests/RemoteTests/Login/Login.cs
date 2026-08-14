namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.Login;

public class Login : MockRemote
{
    [Theory]
    [InlineData(LoginResponse.Ok, RunResponse.Ok, App.Standard, 0x0102_0304, RunResponse.NotResponding)]
    [InlineData(LoginResponse.Ok, RunResponse.Hidden, App.Potatox64, 0x0304_0506, RunResponse.NotRunning)]
    [InlineData(LoginResponse.VoicemeeterNotRunning, RunResponse.NotRunning, App.None, 0, RunResponse.Hidden)]
    [InlineData(LoginResponse.VoicemeeterNotRunning, RunResponse.NotResponding, App.Banana, 0x0203_0405, RunResponse.Ok)]
    public void UpdatesLastConnectionStateWhenAllConditionsMet(LoginResponse loginStatus, RunResponse vmState, App vmApp, int vmPacked, RunResponse buttonsState)
    {
        var vmVersion = (VmVersion)vmPacked;
        var versionResponse = vmApp is App.None
            ? Response.NoServer
            : Response.Ok;

        this.MockWrapper.Setup(w => w.Login()).Returns(loginStatus);
        this.MockWrapper.Setup(w => w.GetVoicemeeterState()).Returns((vmApp, vmState));
        this.MockWrapper.Setup(w => w.GetVoicemeeterVersion()).Returns((versionResponse, vmVersion));
        this.MockWrapper.Setup(w => w.GetApplicationState(App.MacroButtons)).Returns(buttonsState);

        var expectedState = new ConnectionState(loginStatus, vmState, vmApp, vmVersion, buttonsState);

        var result = this.Remote.Login();

        Assert.Multiple(
            () => Assert.Equal(expectedState, result),
            () => Assert.Equal(loginStatus, this.Remote.LoginStatus),
            () => Assert.Equal(expectedState, this.Remote.LastConnectionState),
            () => this.MockWrapper.Verify(w => w.Login(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterState(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(App.MacroButtons), Times.Once())
        );
    }

    [Fact]
    public void ThrowsCannotGetClientExceptionWhenLoginFails()
    {
        this.MockWrapper.Setup(w => w.Login()).Returns(LoginResponse.NoClient);

        var expectedState = new ConnectionState(LoginResponse.LoggedOut, RunResponse.NotRunning, App.None, default, RunResponse.NotRunning);

        var ex = Assert.Throws<CannotGetClientException>(() => this.Remote.Login());

        Assert.Multiple(
            () => Assert.Equal(expectedState, ex.LastConnectionState),
            () => this.MockWrapper.Verify(w => w.Login(), Times.Once())
        );
    }

    [Fact]
    public void ThrowsInvalidOperationExceptionWhenAlreadyLoggedIn()
    {
        this.MockWrapper.Setup(w => w.Login()).Returns(LoginResponse.AlreadyLoggedIn);

        Assert.Multiple(
            () => Assert.Throws<InvalidOperationException>(() => this.Remote.Login()),
            () => this.MockWrapper.Verify(w => w.Login(), Times.Once())
        );
    }

    [Theory]
    [InlineData(LoginResponse.Unknown)]
    [InlineData(LoginResponse.LoggedOut)]
    public void ThrowsUnhandledResponseExceptionWhenUnhandledResponse(LoginResponse response)
    {
        this.MockWrapper.Setup(w => w.Login()).Returns(response);

        var expectedState = new ConnectionState(LoginResponse.LoggedOut, RunResponse.NotRunning, App.None, default, RunResponse.NotRunning);

        var ex = Assert.Throws<UnhandledResponseException>(() => this.Remote.Login());

        Assert.Multiple(
            () => Assert.Equal(response, ex.Response),
            () => Assert.Equal(expectedState, ex.LastConnectionState),
            () => this.MockWrapper.Verify(w => w.Login(), Times.Once())
        );
    }

    [Fact]
    public void BeginCallScopeThrowsObjectDisposedExceptionWhenObjectDisposed()
    {
        this.Remote.Dispose();

        Assert.Multiple(
            () => Assert.Throws<ObjectDisposedException>(() => this.Remote.Login()),
            () => this.MockWrapper.Verify(w => w.Login(), Times.Never())
        );
    }
}
