using PBLivingston.VoicemeeterAPI.Messages;
using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests;

public class Login : MockRemote
{
    [Fact]
    public void Login_UpdatesLoginStatus_Ok_WhenAllConditionsMet()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;

        MockWrapper.Setup(w => w.Login()).Returns(LoginResponse.Ok);
        MockWrapper.Setup(w => w.GetVoicemeeterType(out kind)).Returns(InfoResponse.Ok);
        MockWrapper.Setup(w => w.GetVoicemeeterVersion(out version)).Returns(InfoResponse.Ok);

        MockWrapper.SetupSequence(w => w.IsParametersDirty())
            .Returns(Response.Dirty)
            .Returns(Response.Ok);
        MockWrapper.SetupSequence(w => w.MacroButtonIsDirty())
            .Returns(Response.Dirty)
            .Returns(Response.Ok);

        Remote.Login();

        Assert.Equal(LoginResponse.Ok, Remote.LoginStatus);
        MockWrapper.Verify(w => w.Login(), Times.Once);
    }

    [Fact]
    public void Login_UpdatesLoginStatus_VoicemeeterNotRunning_WhenVoicemeeterNotRunning()
    {
        MockWrapper.Setup(w => w.Login()).Returns(LoginResponse.VoicemeeterNotRunning);

        Remote.Login();

        Assert.Equal(LoginResponse.VoicemeeterNotRunning, Remote.LoginStatus);
        MockWrapper.Verify(w => w.Login(), Times.Once);
    }

    [Theory]
    [InlineData(LoginResponse.AlreadyLoggedIn)]
    [InlineData(LoginResponse.NoClient)]
    public void Login_ThrowsException_Login_WhenLoginFails(LoginResponse expectedResponse)
    {
        MockWrapper.Setup(w => w.Login()).Returns(expectedResponse);

        var ex = Assert.Throws<LoginException>(() => Remote.Login());
        Assert.Equal(expectedResponse, ex.Response);
        MockWrapper.Verify(w => w.Login(), Times.Once);
    }

    [Fact]
    public void Login_ThrowsException_Login_WhenWaitForRunningTimesOut()
    {
        var kind = (int)Kind.None;
        var version = 0;

        MockWrapper.Setup(w => w.Login()).Returns(LoginResponse.Ok);
        MockWrapper.Setup(w => w.GetVoicemeeterType(out kind)).Returns(InfoResponse.NoServer);
        MockWrapper.Setup(w => w.GetVoicemeeterVersion(out version)).Returns(InfoResponse.NoServer);

        var ex = Assert.Throws<LoginException>(() => Remote.Login(timeoutMs: 10));
        Assert.Equal(LoginResponse.Timeout, ex.Response);
        MockWrapper.Verify(w => w.Login(), Times.Once);
    }

    [Fact]
    public void Login_ThrowsException_Remote_WhenAlreadyLoggedIn()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;

        MockWrapper.Setup(w => w.Login()).Returns(LoginResponse.Ok);
        MockWrapper.Setup(w => w.GetVoicemeeterType(out kind)).Returns(InfoResponse.Ok);
        MockWrapper.Setup(w => w.GetVoicemeeterVersion(out version)).Returns(InfoResponse.Ok);

        Remote.Login();

        var ex = Assert.Throws<RemoteException>(() => Remote.Login());
        Assert.Equal($"[VoicemeeterAPI] Remote Error: Already logged in - LoginStatus: {Remote.LoginStatus}.", ex.Message);
        MockWrapper.Verify(w => w.Login(), Times.Once);
    }

    [Fact]
    public void Login_ThrowsException_RemoteAccess_WhenLoginStatusUnknown()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;

        MockWrapper.Setup(w => w.Login()).Returns(LoginResponse.Ok);
        MockWrapper.Setup(w => w.GetVoicemeeterType(out kind)).Returns(InfoResponse.Ok);
        MockWrapper.Setup(w => w.GetVoicemeeterVersion(out version)).Returns(InfoResponse.Ok);
        MockWrapper.Setup(w => w.Logout()).Returns(LoginResponse.NoClient);

        Remote.Login();
        Remote.Logout(timeoutMs: 10);

        var ex = Assert.Throws<RemoteAccessException>(() => Remote.Login());
        Assert.Multiple(
            () => Assert.Equal("Login", ex.Method),
            () => Assert.Equal(LoginResponse.Unknown, ex.LoginStatus)
        );
        MockWrapper.Verify(w => w.Login(), Times.Once);
    }

    [Fact]
    public void Login_ThrowsException_ObjectDisposed_WhenRemoteDisposed()
    {
        Remote.Dispose();

        var ex = Assert.Throws<ObjectDisposedException>(() => Remote.Login());
        Assert.Equal(nameof(Remote), ex.ObjectName);
        MockWrapper.Verify(w => w.Login(), Times.Never);
    }
}