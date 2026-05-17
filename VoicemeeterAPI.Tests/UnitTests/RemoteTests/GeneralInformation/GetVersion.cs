namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.GeneralInformation;

using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;

public class GetVersion : MockRemote
{
    [Fact]
    public void UpdatesLoginStatusOkWhenVoicemeeterRunning()
    {
        var kind = (int)Kind.Banana;
        var version = 0x0201_0202;
        var expectedState = new ConnectionState(LoginResponse.Ok, true, (Kind)kind, (VmVersion)version);

        this.MockLoginVoicemeeterNotRunning();

        this.MockWrapper.Setup(w => w.GetVoicemeeterType(out kind)).Returns(InfoResponse.Ok);
        this.MockWrapper.Setup(w => w.GetVoicemeeterVersion(out version)).Returns(InfoResponse.Ok);

        var result = this.Remote.GetVersion();

        Assert.Multiple(
            () => Assert.Equal((VmVersion)version, result),
            () => Assert.Equal(expectedState, this.Remote.GetConnectionState()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(out version), Times.Exactly(3))
        );
    }

    [Fact]
    public void UpdatesLoginStatusVoicemeeterNotRunningWhenVoicemeeterNotRunning()
    {
        var kind = (int)Kind.Banana;
        var version = 0x0201_0202;
        var noKind = (int)Kind.None;
        var noVersion = 0x0000_0000;
        var expectedState = new ConnectionState(LoginResponse.VoicemeeterNotRunning, true, Kind.None, default);

        this.MockLoginOk(kind, version);

        this.MockWrapper.Setup(w => w.GetVoicemeeterType(out noKind)).Returns(InfoResponse.NoServer);
        this.MockWrapper.Setup(w => w.GetVoicemeeterVersion(out noVersion)).Returns(InfoResponse.NoServer);

        var result = this.Remote.GetVersion();

        Assert.Multiple(
            () => Assert.Equal(default, result),
            () => Assert.Equal(expectedState, this.Remote.GetConnectionState()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(out It.Ref<int>.IsAny), Times.Exactly(4))
        );
    }

    [Fact]
    public void ThrowsExceptionGetInfoWhenUnexpectedResponse()
    {
        var kind = (int)Kind.Banana;
        var version = 0x0201_0202;

        this.MockLoginOk(kind, version);

        var noVersion = 0;
        this.MockWrapper.Setup(w => w.GetVoicemeeterVersion(out noVersion)).Returns(InfoResponse.NoClient);

        var ex = Assert.Throws<GetInfoException>(() => this.Remote.GetVersion());

        Assert.Multiple(
            () => Assert.Equal(InfoResponse.NoClient, ex.Response),
            () => Assert.Equal(noVersion, ex.ReturnedValue),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(out It.Ref<int>.IsAny), Times.Exactly(3))
        );
    }

    [Fact]
    public void ThrowsExceptionAccessDeniedWhenLoginStatusLoggedOut()
    {
        var ex = Assert.Throws<AccessDeniedException>(() => this.Remote.GetVersion());

        Assert.Multiple(
            () => Assert.Equal(LoginResponse.LoggedOut, ex.LoginStatus),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(out It.Ref<int>.IsAny), Times.Never)
        );
    }

    [Fact]
    public void ThrowsExceptionObjectDisposedWhenRemoteDisposed()
    {
        this.Remote.Dispose();

        Assert.Multiple(
            () => Assert.Throws<ObjectDisposedException>(() => this.Remote.GetVersion()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(out It.Ref<int>.IsAny), Times.Never)
        );
    }
}