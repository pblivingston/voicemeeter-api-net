namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.GeneralInformation;

using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;

public class GetKind : MockRemote
{
    [Fact]
    public void UpdatesPrivateLoginStatusOkWhenVoicemeeterLaunched()
    {
        var kind = (int)Kind.Banana;
        var version = 0x0201_0202;
        var expectedState = new ConnectionState(LoginResponse.Ok, RunResponse.Ok, (Kind)kind, (VmVersion)version);

        this.MockLogin();

        this.MockWrapper.Setup(w => w.GetVoicemeeterType()).Returns((InfoResponse.Ok, kind));
        this.MockWrapper.Setup(w => w.GetVoicemeeterVersion()).Returns((InfoResponse.Ok, version));

        var result = this.Remote.GetKind();

        Assert.Multiple(
            () => Assert.Equal(Kind.Banana, result),
            () => Assert.Equal(expectedState, this.Remote.GetConnectionState()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterType(), Times.Exactly(3))
        );
    }

    [Fact]
    public void UpdatesPrivateLoginStatusVoicemeeterNotRunningWhenVoicemeeterClosed()
    {
        var kind = (int)Kind.Banana;
        var version = 0x0201_0202;
        var noKind = (int)Kind.None;
        var noVersion = 0x0000_0000;
        var expectedState = new ConnectionState(LoginResponse.VoicemeeterNotRunning, RunResponse.Ok, Kind.None, default);

        this.MockLogin(kind, version);

        this.MockWrapper.Setup(w => w.GetVoicemeeterType()).Returns((InfoResponse.NoServer, noKind));
        this.MockWrapper.Setup(w => w.GetVoicemeeterVersion()).Returns((InfoResponse.NoServer, noVersion));

        var result = this.Remote.GetKind();

        Assert.Multiple(
            () => Assert.Equal(Kind.None, result),
            () => Assert.Equal(expectedState, this.Remote.GetConnectionState()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterType(), Times.Exactly(3))
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
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterType(), Times.Never)
        );
    }
}
