using PBLivingston.VoicemeeterAPI.EventManagement.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.GeneralInformation;

public class GetVersion : MockRemote
{
    [Fact]
    public void UpdatesLoginStatus_Ok_WhenVoicemeeterRunning()
    {
        var kind = (int)Kind.Banana;
        var version = 0x0201_0202;
        var expectedState = new ConnectionStateEventArgs(LoginResponse.Ok, (Kind)kind, (VmVersion)version);

        MockLogin_VoicemeeterNotRunning();

        MockWrapper.Setup(w => w.GetVoicemeeterType(out kind)).Returns(InfoResponse.Ok);
        MockWrapper.Setup(w => w.GetVoicemeeterVersion(out version)).Returns(InfoResponse.Ok);

        var result = Remote.GetVersion();

        Assert.Multiple(
            () => Assert.Equal((VmVersion)version, result),
            () => Assert.Equal(expectedState, Remote.GetConnectionState()),
            () => MockWrapper.Verify(w => w.GetVoicemeeterVersion(out version), Times.Exactly(3))
        );
    }

    [Fact]
    public void UpdatesLoginStatus_VoicemeeterNotRunning_WhenVoicemeeterNotRunning()
    {
        var kind = (int)Kind.Banana;
        var version = 0x0201_0202;
        var noKind = (int)Kind.None;
        var noVersion = 0x0000_0000;
        var expectedState = new ConnectionStateEventArgs(LoginResponse.VoicemeeterNotRunning, Kind.None, default);

        MockLogin_Ok(kind, version);

        MockWrapper.Setup(w => w.GetVoicemeeterType(out noKind)).Returns(InfoResponse.NoServer);
        MockWrapper.Setup(w => w.GetVoicemeeterVersion(out noVersion)).Returns(InfoResponse.NoServer);

        var result = Remote.GetVersion();

        Assert.Multiple(
            () => Assert.Equal(default, result),
            () => Assert.Equal(expectedState, Remote.GetConnectionState()),
            () => MockWrapper.Verify(w => w.GetVoicemeeterVersion(out It.Ref<int>.IsAny), Times.Exactly(4))
        );
    }

    [Fact]
    public void ThrowsException_GetInfo_WhenUnexpectedResponse()
    {
        var kind = (int)Kind.Banana;
        var version = 0x0201_0202;

        MockLogin_Ok(kind, version);

        var noVersion = 0;
        MockWrapper.Setup(w => w.GetVoicemeeterVersion(out noVersion)).Returns(InfoResponse.NoClient);

        var ex = Assert.Throws<GetInfoException>(() => Remote.GetVersion());

        Assert.Multiple(
            () => Assert.Equal(InfoResponse.NoClient, ex.Response),
            () => Assert.Equal(noVersion, ex.ReturnedValue),
            () => MockWrapper.Verify(w => w.GetVoicemeeterVersion(out It.Ref<int>.IsAny), Times.Exactly(3))
        );
    }

    [Fact]
    public void ThrowsException_AccessDenied_WhenLoginStatusLoggedOut()
    {
        var ex = Assert.Throws<AccessDeniedException>(() => Remote.GetVersion());

        Assert.Multiple(
            () => Assert.Equal(LoginResponse.LoggedOut, ex.LoginStatus),
            () => MockWrapper.Verify(w => w.GetVoicemeeterVersion(out It.Ref<int>.IsAny), Times.Never)
        );
    }

    [Fact]
    public void ThrowsException_ObjectDisposed_WhenRemoteDisposed()
    {
        Remote.Dispose();

        var ex = Assert.Throws<ObjectDisposedException>(() => Remote.GetVersion());

        Assert.Multiple(
            () => Assert.Equal("Remote", ex.ObjectName),
            () => MockWrapper.Verify(w => w.GetVoicemeeterVersion(out It.Ref<int>.IsAny), Times.Never)
        );
    }
}