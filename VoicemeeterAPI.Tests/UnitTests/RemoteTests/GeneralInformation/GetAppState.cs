namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.GeneralInformation;

using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;

public class GetAppState : MockRemote
{
    [Fact]
    public void UpdatesLoginStatusOkWhenAppIsVoicemeeterAndLaunched()
    {
        var app = App.Bananax64;
        var expected = LoginResponse.Ok;

        this.MockLogin();

        this.MockWrapper.Setup(w => w.GetApplicationState(app)).Returns(RunResponse.Ok);

        var result = this.Remote.GetAppState(app);

        Assert.Multiple(
            () => Assert.Equal(RunResponse.Ok, result),
            () => Assert.Equal(expected, this.Remote.LoginStatus),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(app), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterType(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.Is64Bit, Times.Never())
        );
    }

    [Fact]
    public void UpdatesLoginStatusVoicemeeterNotRunningWhenAppIsVoicemeeterAndClosed()
    {
        var app = App.Bananax64;
        var kind = (int)Kind.Banana;
        var version = 0x0201_0202;
        var noKind = (int)Kind.None;
        var expected = LoginResponse.VoicemeeterNotRunning;

        this.MockLogin(kind, version);

        this.MockWrapper.Setup(w => w.GetApplicationState(app)).Returns(RunResponse.NotRunning);
        this.MockWrapper.Setup(w => w.GetVoicemeeterType()).Returns((InfoResponse.NoServer, noKind));
        this.MockWrapper.Setup(w => w.Is64Bit).Returns(true);

        var result = this.Remote.GetAppState(app);

        Assert.Multiple(
            () => Assert.Equal(RunResponse.NotRunning, result),
            () => Assert.Equal(expected, this.Remote.LoginStatus),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(app), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterType(), Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.Is64Bit, Times.Once())
        );
    }

    [Fact]
    public void DoesNotUpdateLoginStatusWhenLoggedOut()
    {
        var app = App.Bananax64;
        var expected = LoginResponse.LoggedOut;

        this.MockWrapper.Setup(w => w.GetApplicationState(app)).Returns(RunResponse.Ok);

        var result = this.Remote.GetAppState(app);

        Assert.Multiple(
            () => Assert.Equal(RunResponse.Ok, result),
            () => Assert.Equal(expected, this.Remote.LoginStatus),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(app), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterType(), Times.Never()),
            () => this.MockWrapper.Verify(w => w.Is64Bit, Times.Never())
        );
    }

    [Fact]
    public void ThrowsExceptionRunWhenUnexpectedResponse()
    {
        var app = App.AUXControlPanel;
        var kind = (int)Kind.Banana;
        var version = 0x0201_0202;

        this.MockLogin(kind, version);

        this.MockWrapper.Setup(w => w.GetApplicationState(app)).Returns(RunResponse.NotInstalled);

        var ex = Assert.Throws<RunException>(() => this.Remote.GetAppState(app));

        Assert.Multiple(
            () => Assert.Equal(RunResponse.NotInstalled, ex.Response),
            () => Assert.Equal(app, ex.App),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(app), Times.Once())
        );
    }

    [Fact]
    public void ThrowsExceptionRunWhenStateDoesNotMatchGetKindResponse()
    {
        var app = App.Bananax64;
        var kind = (int)Kind.Banana;
        var version = 0x0201_0202;

        this.MockLogin(kind, version);

        this.MockWrapper.Setup(w => w.GetApplicationState(app)).Returns(RunResponse.NotRunning);
        this.MockWrapper.Setup(w => w.Is64Bit).Returns(true);

        var ex = Assert.Throws<RunException>(() => this.Remote.GetAppState(app));

        Assert.Multiple(
            () => Assert.Equal(RunResponse.Error, ex.Response),
            () => Assert.Equal(app, ex.App),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(app), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterType(), Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.Is64Bit, Times.Once())
        );
    }

    [Fact]
    public void ThrowsExceptionObjectDisposedWhenRemoteDisposed()
    {
        var app = App.VAIOControlPanel;

        this.Remote.Dispose();

        Assert.Multiple(
            () => Assert.Throws<ObjectDisposedException>(() => this.Remote.GetAppState(app)),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(app), Times.Never())
        );
    }
}
