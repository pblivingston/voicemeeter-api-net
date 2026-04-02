using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.GeneralInformation;

public class GetKind : MockRemote
{
    [Fact]
    public void ReturnsExpected_Kind_UpdatesLoginStatus_Ok_WhenVoicemeeterRunning()
    {
        var kind = (int)Kind.Banana;

        MockWrapper.Setup(w => w.GetVoicemeeterType(out kind)).Returns(InfoResponse.Ok);

        MockLogin_VoicemeeterNotRunning();

        var result = Remote.GetKind();

        Assert.Multiple(
            () => Assert.Equal(Kind.Banana, result),
            () => Assert.Equal(LoginResponse.Ok, Remote.LoginStatus),
            () => MockWrapper.Verify(w => w.GetVoicemeeterType(out kind), Times.Once)
        );
    }

    [Fact]
    public void ReturnsExpected_Kind_UpdatesLoginStatus_VoicemeeterNotRunning_WhenVoicemeeterNotRunning()
    {
        var kind = (int)Kind.Banana;
        var version = 0x0201_0202;

        MockLogin_Ok(kind, version);

        var noKind = (int)Kind.None;
        MockWrapper.Setup(w => w.GetVoicemeeterType(out noKind)).Returns(InfoResponse.NoServer);

        var result = Remote.GetKind();

        Assert.Multiple(
            () => Assert.Equal(Kind.None, result),
            () => Assert.Equal(LoginResponse.VoicemeeterNotRunning, Remote.LoginStatus),
            () => MockWrapper.Verify(w => w.GetVoicemeeterType(out It.Ref<int>.IsAny), Times.Exactly(2))
        );
    }

    [Fact]
    public void ThrowsException_Remote_WhenUnexpectedResponse()
    {
        var kind = (int)Kind.Banana;
        var version = 0x0201_0202;

        MockLogin_Ok(kind, version);

        var noKind = (int)Kind.None;
        MockWrapper.Setup(w => w.GetVoicemeeterType(out noKind)).Returns(InfoResponse.NoClient);

        var ex = Assert.Throws<RemoteException>(() => Remote.GetKind());

        Assert.Multiple(
            () => Assert.Equal("[VoicemeeterAPI] Remote Error: GetKind failed - NoClient; returned kind: None", ex.Message),
            () => MockWrapper.Verify(w => w.GetVoicemeeterType(out It.Ref<int>.IsAny), Times.Exactly(2))
        );
    }

    [Fact]
    public void ThrowsException_RemoteAccess_WhenLoginStatusLoggedOut()
    {
        var ex = Assert.Throws<RemoteAccessException>(() => Remote.GetKind());

        Assert.Multiple(
            () => Assert.Equal("[VoicemeeterAPI] Remote Error: Access to GetKind denied - LoginStatus: LoggedOut", ex.Message),
            () => MockWrapper.Verify(w => w.GetVoicemeeterType(out It.Ref<int>.IsAny), Times.Never)
        );
    }

    [Fact]
    public void ThrowsException_ObjectDisposed_WhenRemoteDisposed()
    {
        Remote.Dispose();

        var ex = Assert.Throws<ObjectDisposedException>(() => Remote.GetKind());

        Assert.Multiple(
            () => Assert.Equal(nameof(Remote), ex.ObjectName),
            () => MockWrapper.Verify(w => w.GetVoicemeeterType(out It.Ref<int>.IsAny), Times.Never)
        );
    }
}