namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.Login;

using PBLivingston.VoicemeeterAPI.Types;

public class Logout : MockRemote
{
    [Fact]
    public void UpdatesConnectionStateLoggedOutWhenSuccessful()
    {
        var loginStatus = LoginResponse.LoggedOut;
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var expectedState = new ConnectionState(loginStatus, RunResponse.Ok, (Kind)kind, (VmVersion)version);

        this.MockWrapper.Setup(w => w.Logout()).Returns(LoginResponse.Ok);

        this.MockLoginOk(kind, version);

        var result = this.Remote.Logout();

        Assert.Multiple(
            () => Assert.Equal(loginStatus, result),
            () => Assert.Equal(expectedState, this.Remote.GetConnectionState()),
            () => this.MockWrapper.Verify(w => w.Logout(), Times.Once())
        );
    }

    [Fact]
    public void ReturnsExpectedLoginResponseWhenAlreadyLoggedOut()
    {
        var result = this.Remote.Logout();

        Assert.Multiple(
            () => Assert.Equal(LoginResponse.LoggedOut, result),
            () => this.MockWrapper.Verify(w => w.Logout(), Times.Never())
        );
    }

    [Fact]
    public void UpdatesConnectionStateUnknownWhenTimesOut()
    {
        var loginStatus = LoginResponse.Unknown;
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var expectedState = new ConnectionState(loginStatus, RunResponse.Ok, (Kind)kind, (VmVersion)version);

        this.MockWrapper.Setup(w => w.Logout()).Returns(LoginResponse.NoClient);

        this.MockLoginOk(kind, version);

        var result = this.Remote.Logout();

        Assert.Multiple(
            () => Assert.Equal(loginStatus, result),
            () => Assert.Equal(expectedState, this.Remote.GetConnectionState()),
            () => this.MockWrapper.Verify(w => w.Logout(), Times.Once())
        );
    }

    [Fact]
    public void ThrowsExceptionObjectDisposedWhenRemoteDisposed()
    {
        this.Remote.Dispose();

        Assert.Multiple(
            () => Assert.Throws<ObjectDisposedException>(() => this.Remote.Logout()),
            () => this.MockWrapper.Verify(w => w.Logout(), Times.Never())
        );
    }

    [Fact]
    public void CalledByDisposeWhenStillLoggedIn()
    {
        var kind = Kind.Standard;
        var version = 0x0101_0202;

        this.MockWrapper.Setup(w => w.Logout()).Returns(LoginResponse.Ok);

        this.MockLoginOk((int)kind, version);

        this.Remote.Dispose();

        this.MockWrapper.Verify(w => w.Logout(), Times.Once());
    }
}
