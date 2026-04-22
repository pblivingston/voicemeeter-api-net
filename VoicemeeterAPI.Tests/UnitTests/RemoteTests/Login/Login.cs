using PBLivingston.VoicemeeterAPI.EventManagement.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.Login;

public class Login : MockRemote
{
    [Fact]
    public void UpdatesConnectionState_Ok_WhenAllConditionsMet()
    {
        var loginStatus = LoginResponse.Ok;
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var expectedState = new ConnectionStateEventArgs(loginStatus, true, (Kind)kind, (VmVersion)version);

        MockWrapper.Setup(w => w.Login()).Returns(loginStatus);
        MockWrapper.Setup(w => w.GetVoicemeeterType(out kind)).Returns(InfoResponse.Ok);
        MockWrapper.Setup(w => w.GetVoicemeeterVersion(out version)).Returns(InfoResponse.Ok);

        MockWrapper.SetupSequence(w => w.IsParametersDirty())
            .Returns(Response.Dirty)
            .Returns(Response.Ok);
        MockWrapper.SetupSequence(w => w.MacroButtonIsDirty())
            .Returns(Response.Dirty)
            .Returns(Response.Ok);

        var result = Remote.Login();

        Assert.Multiple(
            () => Assert.Equal(loginStatus, result),
            () => Assert.Equal(expectedState, Remote.GetConnectionState()),
            () => MockWrapper.Verify(w => w.Login(), Times.Once)
        );
    }

    [Fact]
    public void UpdatesConnectionState_VoicemeeterNotRunning_WhenVoicemeeterNotRunning()
    {
        var kind = (int)Kind.None;
        var version = 0x0000_0000;
        var loginStatus = LoginResponse.VoicemeeterNotRunning;
        var expectedState = new ConnectionStateEventArgs(loginStatus, true, Kind.None, default);

        MockWrapper.Setup(w => w.Login()).Returns(loginStatus);
        MockWrapper.Setup(w => w.GetVoicemeeterType(out kind)).Returns(InfoResponse.NoServer);
        MockWrapper.Setup(w => w.GetVoicemeeterVersion(out version)).Returns(InfoResponse.NoServer);

        var result = Remote.Login();

        Assert.Multiple(
            () => Assert.Equal(loginStatus, result),
            () => Assert.Equal(expectedState, Remote.GetConnectionState()),
            () => MockWrapper.Verify(w => w.Login(), Times.Once)
        );
    }

    [Theory]
    [InlineData(LoginResponse.AlreadyLoggedIn)]
    [InlineData(LoginResponse.NoClient)]
    public void ThrowsException_Remote_WhenLoginFails(LoginResponse expectedResponse)
    {
        MockWrapper.Setup(w => w.Login()).Returns(expectedResponse);

        var ex = Assert.Throws<RemoteException<LoginResponse>>(() => Remote.Login());

        Assert.Multiple(
            () => Assert.Equal(expectedResponse, ex.Response),
            () => MockWrapper.Verify(w => w.Login(), Times.Once)
        );
    }

    [Fact]
    public void ThrowsException_Remote_WhenWaitForRunningTimesOut()
    {
        var kind = (int)Kind.None;
        var version = 0;

        MockWrapper.Setup(w => w.Login()).Returns(LoginResponse.Ok);
        MockWrapper.Setup(w => w.GetVoicemeeterType(out kind)).Returns(InfoResponse.NoServer);
        MockWrapper.Setup(w => w.GetVoicemeeterVersion(out version)).Returns(InfoResponse.NoServer);

        var ex = Assert.Throws<RemoteException<LoginResponse>>(() => Remote.Login(timeoutMs: 10));

        Assert.Multiple(
            () => Assert.Equal(LoginResponse.Timeout, ex.Response),
            () => MockWrapper.Verify(w => w.Login(), Times.Once)
        );
    }

    [Fact]
    public void ThrowsException_Remote_WhenAlreadyLoggedIn()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;

        MockLogin_Ok(kind, version);

        var ex = Assert.Throws<RemoteException<LoginResponse>>(() => Remote.Login());

        Assert.Multiple(
            () => Assert.Equal(LoginResponse.AlreadyLoggedIn, ex.Response),
            () => MockWrapper.Verify(w => w.Login(), Times.Once)
        );
    }

    [Fact]
    public void ThrowsException_AccessDenied_WhenLoginStatusUnknown()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;

        MockWrapper.Setup(w => w.Logout()).Returns(LoginResponse.NoClient);

        MockLogin_Ok(kind, version);

        Remote.Logout(timeoutMs: 10);

        var ex = Assert.Throws<AccessDeniedException>(() => Remote.Login());

        Assert.Multiple(
            () => Assert.Equal(LoginResponse.Unknown, ex.LoginStatus),
            () => MockWrapper.Verify(w => w.Login(), Times.Once)
        );
    }

    [Fact]
    public void ThrowsException_ObjectDisposed_WhenRemoteDisposed()
    {
        Remote.Dispose();

        var ex = Assert.Throws<ObjectDisposedException>(() => Remote.Login());

        Assert.Multiple(
            () => Assert.Equal("Remote", ex.ObjectName),
            () => MockWrapper.Verify(w => w.Login(), Times.Never)
        );
    }
}