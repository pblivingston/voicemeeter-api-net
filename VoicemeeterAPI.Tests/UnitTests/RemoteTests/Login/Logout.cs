using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.Login;

public class Logout : MockRemote
{
    [Fact]
    public void UpdatesConnectionState_LoggedOut_WhenSuccessful()
    {
        var loginStatus = LoginResponse.LoggedOut;
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var expectedState = new ConnectionStateEventArgs(loginStatus, (Kind)kind, (VmVersion)version);

        MockWrapper.Setup(w => w.Logout()).Returns(LoginResponse.Ok);

        MockLogin_Ok(kind, version);

        var result = Remote.Logout();

        Assert.Multiple(
            () => Assert.Equal(loginStatus, result),
            () => Assert.Equal(expectedState, Remote.ConnectionState),
            () => MockWrapper.Verify(w => w.Logout(), Times.Once)
        );
    }

    [Fact]
    public void UpdatesConnectionState_Unknown_WhenTimesOut()
    {
        var loginStatus = LoginResponse.Unknown;
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var expectedState = new ConnectionStateEventArgs(loginStatus, (Kind)kind, (VmVersion)version);

        MockWrapper.Setup(w => w.Logout()).Returns(LoginResponse.NoClient);

        MockLogin_Ok(kind, version);

        var result = Remote.Logout(timeoutMs: 10);

        Assert.Multiple(
            () => Assert.Equal(loginStatus, result),
            () => Assert.Equal(expectedState, Remote.ConnectionState),
            () => MockWrapper.Verify(w => w.Logout(), Times.Once)
        );
    }

    [Fact]
    public void ThrowsException_ObjectDisposed_WhenRemoteDisposed()
    {
        Remote.Dispose();

        var ex = Assert.Throws<ObjectDisposedException>(() => Remote.Logout());

        Assert.Multiple(
            () => Assert.Equal("Remote", ex.ObjectName),
            () => MockWrapper.Verify(w => w.Logout(), Times.Never)
        );
    }

    [Fact]
    public void CalledByDispose_WhenStillLoggedIn()
    {
        var kind = Kind.Standard;
        var version = 0x0101_0202;
        var expectedState = new ConnectionStateEventArgs(LoginResponse.LoggedOut, kind, (VmVersion)version);

        MockWrapper.Setup(w => w.Logout()).Returns(LoginResponse.Ok);

        MockLogin_Ok((int)kind, version);

        Remote.Dispose();

        Assert.Multiple(
            () => Assert.Equal(expectedState, Remote.ConnectionState),
            () => MockWrapper.Verify(w => w.Logout(), Times.Once)
        );
    }
}