namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.GeneralInformation;

using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;

public class GetAppState : MockRemote
{
    [Fact]
    public void UpdatesPrivateLoginStatusOkWhenAppIsVoicemeeterAndLaunched()
    {
        var app = App.Bananax64;
        var kind = (int)Kind.Banana;
        var version = 0x0201_0202;
        var expectedState = new ConnectionState(LoginResponse.Ok, RunResponse.Ok, (Kind)kind, (VmVersion)version);

        this.MockLogin();

        this.MockWrapper.Setup(w => w.Is64Bit).Returns(true);
        this.MockWrapper.Setup(w => w.GetApplicationState(app)).Returns(RunResponse.Ok);
        this.MockWrapper.Setup(w => w.GetVoicemeeterType()).Returns((InfoResponse.Ok, kind));
        this.MockWrapper.Setup(w => w.GetVoicemeeterVersion()).Returns((InfoResponse.Ok, version));

        var result = this.Remote.GetAppState(app);

        Assert.Multiple(
            () => Assert.Equal(RunResponse.Ok, result),
            () => Assert.Equal(expectedState, this.Remote.GetConnectionState()),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(app), Times.Once())
        );
    }

    [Fact]
    public void UpdatesPrivateLoginStatusVoicemeeterNotRunningWhenAppIsVoicemeeterAndClosed()
    {
        var app = App.Bananax64;
        var kind = (int)Kind.Banana;
        var version = 0x0201_0202;
        var noKind = (int)Kind.None;
        var noVersion = 0x0000_0000;
        var expectedState = new ConnectionState(LoginResponse.VoicemeeterNotRunning, RunResponse.Ok, Kind.None, default);

        this.MockLogin(kind, version);

        this.MockWrapper.Setup(w => w.Is64Bit).Returns(true);
        this.MockWrapper.Setup(w => w.GetApplicationState(app)).Returns(RunResponse.NotRunning);
        this.MockWrapper.Setup(w => w.GetVoicemeeterType()).Returns((InfoResponse.NoServer, noKind));
        this.MockWrapper.Setup(w => w.GetVoicemeeterVersion()).Returns((InfoResponse.NoServer, noVersion));

        var result = this.Remote.GetAppState(app);

        Assert.Multiple(
            () => Assert.Equal(RunResponse.NotRunning, result),
            () => Assert.Equal(expectedState, this.Remote.GetConnectionState()),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(app), Times.Once())
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
    public void ThrowsExceptionObjectDisposedWhenRemoteDisposed()
    {
        var app = App.VAIOControlPanel;

        this.Remote.Dispose();

        Assert.Multiple(
            () => Assert.Throws<ObjectDisposedException>(() => this.Remote.GetAppState(app)),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(app), Times.Never)
        );
    }
}
