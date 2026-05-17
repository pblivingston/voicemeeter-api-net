namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.Login;

using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;

public class Login : MockRemote
{
    [Fact]
    public void UpdatesConnectionStateOkWhenAllConditionsMet()
    {
        var loginStatus = LoginResponse.Ok;
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var expectedState = new ConnectionStateEventArgs(loginStatus, true, (Kind)kind, (VmVersion)version);

        this.MockWrapper.Setup(w => w.Login()).Returns(loginStatus);
        this.MockWrapper.Setup(w => w.GetVoicemeeterType(out kind)).Returns(InfoResponse.Ok);
        this.MockWrapper.Setup(w => w.GetVoicemeeterVersion(out version)).Returns(InfoResponse.Ok);
        this.MockWrapper.Setup(w => w.MacroButtonIsRunning()).Returns(RunResponse.Ok);

        this.MockWrapper.SetupSequence(w => w.IsParametersDirty())
            .Returns(Response.Dirty)
            .Returns(Response.Ok);
        this.MockWrapper.SetupSequence(w => w.MacroButtonIsDirty())
            .Returns(Response.Dirty)
            .Returns(Response.Ok);

        var result = this.Remote.Login();

        Assert.Multiple(
            () => Assert.Equal(loginStatus, result),
            () => Assert.Equal(expectedState, this.Remote.GetConnectionState()),
            () => this.MockWrapper.Verify(w => w.Login(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterType(out kind), Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(out version), Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.MacroButtonIsRunning(), Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.IsParametersDirty(), Times.Exactly(3)),
            () => this.MockWrapper.Verify(w => w.MacroButtonIsDirty(), Times.Exactly(2))
        );
    }

    [Fact]
    public void UpdatesConnectionStateVoicemeeterNotRunningWhenVoicemeeterNotRunning()
    {
        var kind = (int)Kind.None;
        var version = 0x0000_0000;
        var loginStatus = LoginResponse.VoicemeeterNotRunning;
        var expectedState = new ConnectionStateEventArgs(loginStatus, true, Kind.None, default);

        this.MockWrapper.Setup(w => w.Login()).Returns(loginStatus);
        this.MockWrapper.Setup(w => w.GetVoicemeeterType(out kind)).Returns(InfoResponse.NoServer);
        this.MockWrapper.Setup(w => w.GetVoicemeeterVersion(out version)).Returns(InfoResponse.NoServer);
        this.MockWrapper.Setup(w => w.MacroButtonIsRunning()).Returns(RunResponse.Ok);

        var result = this.Remote.Login();

        Assert.Multiple(
            () => Assert.Equal(loginStatus, result),
            () => Assert.Equal(expectedState, this.Remote.GetConnectionState()),
            () => this.MockWrapper.Verify(w => w.Login(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterType(out kind), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(out version), Times.Once()),
            () => this.MockWrapper.Verify(w => w.MacroButtonIsRunning(), Times.Exactly(2))
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
    public void ThrowsExceptionRemoteWhenWaitForRunningTimesOut()
    {
        var kind = (int)Kind.None;
        var version = 0;

        this.MockWrapper.Setup(w => w.Login()).Returns(LoginResponse.Ok);
        this.MockWrapper.Setup(w => w.GetVoicemeeterType(out kind)).Returns(InfoResponse.NoServer);
        this.MockWrapper.Setup(w => w.GetVoicemeeterVersion(out version)).Returns(InfoResponse.NoServer);

        var ex = Assert.Throws<RemoteException<LoginResponse>>(() => this.Remote.Login(timeoutMs: 10));

        Assert.Multiple(
            () => Assert.Equal(LoginResponse.Timeout, ex.Response),
            () => this.MockWrapper.Verify(w => w.Login(), Times.Once())
        );
    }

    [Fact]
    public void ThrowsExceptionRemoteWhenAlreadyLoggedIn()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;

        this.MockLoginOk(kind, version);

        var ex = Assert.Throws<RemoteException<LoginResponse>>(() => this.Remote.Login());

        Assert.Multiple(
            () => Assert.Equal(LoginResponse.AlreadyLoggedIn, ex.Response),
            () => this.MockWrapper.Verify(w => w.Login(), Times.Once())
        );
    }

    [Fact]
    public void ThrowsExceptionAccessDeniedWhenLoginStatusUnknown()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;

        this.MockWrapper.Setup(w => w.Logout()).Returns(LoginResponse.NoClient);

        this.MockLoginOk(kind, version);

        this.Remote.Logout(timeoutMs: 10);

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