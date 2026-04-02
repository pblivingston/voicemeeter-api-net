using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.Login;

public class Logout : MockRemote
{
    [Fact]
    public void UpdatesLoginStatus_LoggedOut_WhenSuccessful()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;

        MockWrapper.Setup(w => w.Logout()).Returns(LoginResponse.Ok);

        MockLogin_Ok(kind, version);

        Remote.Logout();

        Assert.Multiple(
            () => Assert.Equal(LoginResponse.LoggedOut, Remote.LoginStatus),
            () => MockWrapper.Verify(w => w.Logout(), Times.Once)
        );
    }

    [Fact]
    public void UpdatesLoginStatus_Unknown_WhenTimesOut()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;

        MockWrapper.Setup(w => w.Logout()).Returns(LoginResponse.NoClient);

        MockLogin_Ok(kind, version);

        Remote.Logout(timeoutMs: 10);

        Assert.Multiple(
            () => Assert.Equal(LoginResponse.Unknown, Remote.LoginStatus),
            () => MockWrapper.Verify(w => w.Logout(), Times.Once)
        );
    }

    [Fact]
    public void ThrowsException_Remote_WhenAlreadyLoggedOut()
    {
        var ex = Assert.Throws<RemoteException>(() => Remote.Logout());

        Assert.Multiple(
            () => Assert.Equal("[VoicemeeterAPI] Remote Error: Already logged out", ex.Message),
            () => MockWrapper.Verify(w => w.Logout(), Times.Never)
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
}