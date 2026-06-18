namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.GeneralInformation;

using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;

public class GetKind : MockRemote
{
    [Fact]
    public void UpdatesLoginStatusOkWhenVoicemeeterLaunched()
    {
        var kind = (int)Kind.Banana;
        var expected = LoginResponse.Ok;

        this.MockLogin();

        this.MockWrapper.Setup(w => w.GetVoicemeeterType()).Returns((InfoResponse.Ok, kind));

        var result = this.Remote.GetKind();

        Assert.Multiple(
            () => Assert.Equal(Kind.Banana, result),
            () => Assert.Equal(expected, this.Remote.LoginStatus),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterType(), Times.Exactly(2))
        );
    }

    [Fact]
    public void UpdatesLoginStatusVoicemeeterNotRunningWhenVoicemeeterClosed()
    {
        var kind = (int)Kind.Banana;
        var version = 0x0201_0202;
        var noKind = (int)Kind.None;
        var expected = LoginResponse.VoicemeeterNotRunning;

        this.MockLogin(kind, version);

        this.MockWrapper.Setup(w => w.GetVoicemeeterType()).Returns((InfoResponse.NoServer, noKind));

        var result = this.Remote.GetKind();

        Assert.Multiple(
            () => Assert.Equal(Kind.None, result),
            () => Assert.Equal(expected, this.Remote.LoginStatus),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterType(), Times.Exactly(2))
        );
    }

    [Fact]
    public void ThrowsExceptionGetInfoWhenUnexpectedResponse()
    {
        var kind = (int)Kind.Banana;
        var version = 0x0201_0202;

        this.MockLogin(kind, version);

        var noKind = (int)Kind.None;
        this.MockWrapper.Setup(w => w.GetVoicemeeterType()).Returns((InfoResponse.NoClient, noKind));

        var ex = Assert.Throws<GetInfoException>(() => this.Remote.GetKind());

        Assert.Multiple(
            () => Assert.Equal(InfoResponse.NoClient, ex.Response),
            () => Assert.Equal(noKind, ex.ReturnedValue),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterType(), Times.Exactly(2))
        );
    }

    [Fact]
    public void ThrowsExceptionObjectDisposedWhenRemoteDisposed()
    {
        this.Remote.Dispose();

        Assert.Multiple(
            () => Assert.Throws<ObjectDisposedException>(() => this.Remote.GetKind()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterType(), Times.Never())
        );
    }
}
