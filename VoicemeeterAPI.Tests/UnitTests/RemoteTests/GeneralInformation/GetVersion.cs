namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.GeneralInformation;

using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;

public class GetVersion : MockRemote
{
    [Fact]
    public void UpdatesLoginStatusOkWhenVoicemeeterLaunched()
    {
        var version = 0x0201_0202;
        var expected = LoginResponse.Ok;

        this.MockLogin();

        this.MockWrapper.Setup(w => w.GetVoicemeeterVersion()).Returns((InfoResponse.Ok, version));

        var result = this.Remote.GetVersion();

        Assert.Multiple(
            () => Assert.Equal((VmVersion)version, result),
            () => Assert.Equal(expected, this.Remote.LoginStatus),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(), Times.Exactly(2))
        );
    }

    [Fact]
    public void UpdatesLoginStatusVoicemeeterNotRunningWhenVoicemeeterClosed()
    {
        var kind = (int)Kind.Banana;
        var version = 0x0201_0202;
        var noVersion = 0x0000_0000;
        var expected = LoginResponse.VoicemeeterNotRunning;

        this.MockLogin(kind, version);

        this.MockWrapper.Setup(w => w.GetVoicemeeterVersion()).Returns((InfoResponse.NoServer, noVersion));

        var result = this.Remote.GetVersion();

        Assert.Multiple(
            () => Assert.Equal(default, result),
            () => Assert.Equal(expected, this.Remote.LoginStatus),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(), Times.Exactly(2))
        );
    }

    [Fact]
    public void ThrowsExceptionGetInfoWhenUnexpectedResponse()
    {
        var kind = (int)Kind.Banana;
        var version = 0x0201_0202;

        this.MockLogin(kind, version);

        var noVersion = 0;
        this.MockWrapper.Setup(w => w.GetVoicemeeterVersion()).Returns((InfoResponse.NoClient, noVersion));

        var ex = Assert.Throws<GetInfoException>(() => this.Remote.GetVersion());

        Assert.Multiple(
            () => Assert.Equal(InfoResponse.NoClient, ex.Response),
            () => Assert.Equal(noVersion, ex.ReturnedValue),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(), Times.Exactly(2))
        );
    }

    [Fact]
    public void ThrowsExceptionObjectDisposedWhenRemoteDisposed()
    {
        this.Remote.Dispose();

        Assert.Multiple(
            () => Assert.Throws<ObjectDisposedException>(() => this.Remote.GetVersion()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(), Times.Never())
        );
    }
}
