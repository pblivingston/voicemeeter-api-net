namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.Login;

using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;

public class Login : MockRemote
{
    [Fact]
    public void UpdatesLastConnectionStateOkWhenAllConditionsMet()
    {
        var loginStatus = LoginResponse.Ok;
        var buttonsState = RunResponse.Ok;
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var expectedState = new ConnectionState(loginStatus, buttonsState, (Kind)kind, (VmVersion)version);

        this.MockWrapper.Setup(w => w.Login()).Returns(loginStatus);
        this.MockWrapper.Setup(w => w.GetVoicemeeterType()).Returns((InfoResponse.Ok, kind));
        this.MockWrapper.Setup(w => w.GetVoicemeeterVersion()).Returns((InfoResponse.Ok, version));
        this.MockWrapper.Setup(w => w.GetApplicationState(App.MacroButtons)).Returns(buttonsState);

        var result = this.Remote.Login();

        Assert.Multiple(
            () => Assert.Equal(loginStatus, result),
            () => Assert.Equal(expectedState, this.Remote.LastConnectionState),
            () => this.MockWrapper.Verify(w => w.Login(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterType(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(App.MacroButtons), Times.Once())
        );
    }

    [Fact]
    public void UpdatesLastLoginStatusVoicemeeterNotRunningWhenVoicemeeterNotRunning()
    {
        var loginStatus = LoginResponse.VoicemeeterNotRunning;
        var buttonsState = RunResponse.Ok;
        var kind = (int)Kind.None;
        var version = 0x0000_0000;
        var expectedState = new ConnectionState(loginStatus, buttonsState, Kind.None, default);

        this.MockWrapper.Setup(w => w.Login()).Returns(loginStatus);
        this.MockWrapper.Setup(w => w.GetVoicemeeterType()).Returns((InfoResponse.NoServer, kind));
        this.MockWrapper.Setup(w => w.GetVoicemeeterVersion()).Returns((InfoResponse.NoServer, version));
        this.MockWrapper.Setup(w => w.GetApplicationState(App.MacroButtons)).Returns(buttonsState);

        var result = this.Remote.Login();

        Assert.Multiple(
            () => Assert.Equal(loginStatus, result),
            () => Assert.Equal(expectedState, this.Remote.LastConnectionState),
            () => this.MockWrapper.Verify(w => w.Login(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterType(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(App.MacroButtons), Times.Once())
        );
    }

    [Theory]
    [InlineData(RunResponse.Hidden)]
    [InlineData(RunResponse.NotRunning)]
    [InlineData(RunResponse.NotResponding)]
    public void UpdatesLastButtonsStateWhenButtonsIsState(RunResponse buttonsState)
    {
        var loginStatus = LoginResponse.Ok;
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var expectedState = new ConnectionState(loginStatus, buttonsState, (Kind)kind, (VmVersion)version);

        this.MockWrapper.Setup(w => w.Login()).Returns(loginStatus);
        this.MockWrapper.Setup(w => w.GetVoicemeeterType()).Returns((InfoResponse.Ok, kind));
        this.MockWrapper.Setup(w => w.GetVoicemeeterVersion()).Returns((InfoResponse.Ok, version));
        this.MockWrapper.Setup(w => w.GetApplicationState(App.MacroButtons)).Returns(buttonsState);

        var result = this.Remote.Login();

        Assert.Multiple(
            () => Assert.Equal(loginStatus, result),
            () => Assert.Equal(expectedState, this.Remote.LastConnectionState),
            () => this.MockWrapper.Verify(w => w.Login(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterType(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(App.MacroButtons), Times.Once())
        );
    }

    [Theory]
    [InlineData(LoginResponse.AlreadyLoggedIn)]
    [InlineData(LoginResponse.NoClient)]
    public void ThrowsExceptionRemoteWhenLoginFails(LoginResponse expectedResponse)
    {
        this.MockWrapper.Setup(w => w.Login()).Returns(expectedResponse);

        var ex = Assert.Throws<RemoteException<LoginResponse>>(() => this.Remote.Login());

        Assert.Multiple(
            () => Assert.Equal(expectedResponse, ex.Response),
            () => this.MockWrapper.Verify(w => w.Login(), Times.Once())
        );
    }

    [Fact]
    public void ThrowsExceptionAccessDeniedWhenAlreadyLoggedIn()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;

        this.MockLogin(kind, version);

        var ex = Assert.Throws<AccessDeniedException>(() => this.Remote.Login());

        Assert.Multiple(
            () => Assert.Equal(LoginResponse.Ok, ex.LoginStatus),
            () => this.MockWrapper.Verify(w => w.Login(), Times.Once())
        );
    }

    [Fact]
    public void ThrowsExceptionAccessDeniedWhenLoginStatusUnknown()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;

        this.MockWrapper.Setup(w => w.Logout()).Returns(LoginResponse.NoClient);

        this.MockLogin(kind, version);

        this.Remote.Logout();

        var ex = Assert.Throws<AccessDeniedException>(() => this.Remote.Login());

        Assert.Multiple(
            () => Assert.Equal(LoginResponse.Unknown, ex.LoginStatus),
            () => this.MockWrapper.Verify(w => w.Login(), Times.Once())
        );
    }

    [Fact]
    public void ThrowsExceptionObjectDisposedWhenRemoteDisposed()
    {
        this.Remote.Dispose();

        Assert.Multiple(
            () => Assert.Throws<ObjectDisposedException>(() => this.Remote.Login()),
            () => this.MockWrapper.Verify(w => w.Login(), Times.Never())
        );
    }
}
