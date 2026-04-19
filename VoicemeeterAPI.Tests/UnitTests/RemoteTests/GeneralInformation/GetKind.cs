using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.GeneralInformation;

public class GetKind : MockRemote
{
    [Fact]
    public void UpdatesConnectionState_Ok_WhenVoicemeeterRunning()
    {
        var kind = (int)Kind.Banana;
        var version = 0x0201_0202;
        var expectedState = new ConnectionStateEventArgs(LoginResponse.Ok, (Kind)kind, (VmVersion)version);

        MockWrapper.Setup(w => w.GetVoicemeeterType(out kind)).Returns(InfoResponse.Ok);
        MockWrapper.Setup(w => w.GetVoicemeeterVersion(out version)).Returns(InfoResponse.Ok);

        MockLogin_VoicemeeterNotRunning();

        var result = Remote.GetKind();

        Assert.Multiple(
            () => Assert.Equal(Kind.Banana, result),
            () => Assert.Equal(expectedState, Remote.ConnectionState),
            () => MockWrapper.Verify(w => w.GetVoicemeeterType(out kind), Times.Once)
        );
    }

    [Fact]
    public void UpdatesConnectionState_VoicemeeterNotRunning_WhenVoicemeeterNotRunning()
    {
        var kind = (int)Kind.Banana;
        var version = 0x0201_0202;
        var expectedState = new ConnectionStateEventArgs(LoginResponse.VoicemeeterNotRunning, Kind.None, default);

        MockLogin_Ok(kind, version);

        MockWrapper.Setup(w => w.GetVoicemeeterType(out It.Ref<int>.IsAny)).Returns(InfoResponse.NoServer);
        MockWrapper.Setup(w => w.GetVoicemeeterVersion(out It.Ref<int>.IsAny)).Returns(InfoResponse.NoServer);

        var result = Remote.GetKind();

        Assert.Multiple(
            () => Assert.Equal(Kind.None, result),
            () => Assert.Equal(expectedState, Remote.ConnectionState),
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
    public void ThrowsException_Remote_WhenVersionMismatch()
    {
        var kind = (int)Kind.Banana;

        MockWrapper.Setup(w => w.GetVoicemeeterType(out kind)).Returns(InfoResponse.Ok);
        MockWrapper.Setup(w => w.GetVoicemeeterVersion(out It.Ref<int>.IsAny)).Returns(InfoResponse.NoServer);

        MockLogin_VoicemeeterNotRunning();

        var ex = Assert.Throws<RemoteException>(() => Remote.GetKind());

        Assert.Multiple(
            () => Assert.Equal("[VoicemeeterAPI] Remote Error: RunningVersion '0.0.0.0' and RunningKind 'Banana' do not match", ex.Message),
            () => MockWrapper.Verify(w => w.GetVoicemeeterType(out It.Ref<int>.IsAny), Times.Once)
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