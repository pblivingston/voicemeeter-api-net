using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.GeneralInformation;

public class GetVersion : MockRemote
{
    [Fact]
    public void UpdatesConnectionState_Ok_WhenVoicemeeterRunning()
    {
        var kind = (int)Kind.Banana;
        var version = 0x0201_0202;
        var expectedState = new ConnectionStateRecord(LoginResponse.Ok, (Kind)kind, (VmVersion)version);

        MockWrapper.Setup(w => w.GetVoicemeeterType(out kind)).Returns(InfoResponse.Ok);
        MockWrapper.Setup(w => w.GetVoicemeeterVersion(out version)).Returns(InfoResponse.Ok);

        MockLogin_VoicemeeterNotRunning();

        var result = Remote.GetVersion();

        Assert.Multiple(
            () => Assert.Equal((VmVersion)version, result),
            () => Assert.Equal(expectedState, Remote.ConnectionState),
            () => MockWrapper.Verify(w => w.GetVoicemeeterVersion(out version), Times.Once)
        );
    }

    [Fact]
    public void UpdatesConnectionState_VoicemeeterNotRunning_WhenVoicemeeterNotRunning()
    {
        var kind = (int)Kind.Banana;
        var version = 0x0201_0202;
        var expectedState = new ConnectionStateRecord(LoginResponse.VoicemeeterNotRunning, Kind.None, default);

        MockLogin_Ok(kind, version);

        MockWrapper.Setup(w => w.GetVoicemeeterType(out It.Ref<int>.IsAny)).Returns(InfoResponse.NoServer);
        MockWrapper.Setup(w => w.GetVoicemeeterVersion(out It.Ref<int>.IsAny)).Returns(InfoResponse.NoServer);

        var result = Remote.GetVersion();

        Assert.Multiple(
            () => Assert.Equal(default, result),
            () => Assert.Equal(expectedState, Remote.ConnectionState),
            () => MockWrapper.Verify(w => w.GetVoicemeeterVersion(out It.Ref<int>.IsAny), Times.Exactly(2))
        );
    }

    [Fact]
    public void ThrowsException_Remote_WhenUnexpectedResponse()
    {
        var kind = (int)Kind.Banana;
        var version = 0x0201_0202;

        MockLogin_Ok(kind, version);

        var noVersion = 0;
        MockWrapper.Setup(w => w.GetVoicemeeterVersion(out noVersion)).Returns(InfoResponse.NoClient);

        var ex = Assert.Throws<RemoteException>(() => Remote.GetVersion());

        Assert.Multiple(
            () => Assert.Equal("[VoicemeeterAPI] Remote Error: GetVersion failed - NoClient; returned version: 0.0.0.0", ex.Message),
            () => MockWrapper.Verify(w => w.GetVoicemeeterVersion(out It.Ref<int>.IsAny), Times.Exactly(2))
        );
    }

    [Fact]
    public void ThrowsException_Remote_WhenKindMismatch()
    {
        var version = 0x0201_0202;

        MockWrapper.Setup(w => w.GetVoicemeeterType(out It.Ref<int>.IsAny)).Returns(InfoResponse.NoServer);
        MockWrapper.Setup(w => w.GetVoicemeeterVersion(out version)).Returns(InfoResponse.Ok);

        MockLogin_VoicemeeterNotRunning();

        var ex = Assert.Throws<RemoteException>(() => Remote.GetVersion());

        Assert.Multiple(
            () => Assert.Equal("[VoicemeeterAPI] Remote Error: RunningKind 'None' and RunningVersion '2.1.2.2' do not match", ex.Message),
            () => MockWrapper.Verify(w => w.GetVoicemeeterType(out It.Ref<int>.IsAny), Times.Once)
        );
    }

    [Fact]
    public void ThrowsException_RemoteAccess_WhenLoginStatusLoggedOut()
    {
        var ex = Assert.Throws<RemoteAccessException>(() => Remote.GetVersion());

        Assert.Multiple(
            () => Assert.Equal("[VoicemeeterAPI] Remote Error: Access to GetVersion denied - LoginStatus: LoggedOut", ex.Message),
            () => MockWrapper.Verify(w => w.GetVoicemeeterVersion(out It.Ref<int>.IsAny), Times.Never)
        );
    }

    [Fact]
    public void ThrowsException_ObjectDisposed_WhenRemoteDisposed()
    {
        Remote.Dispose();

        var ex = Assert.Throws<ObjectDisposedException>(() => Remote.GetVersion());

        Assert.Multiple(
            () => Assert.Equal(nameof(Remote), ex.ObjectName),
            () => MockWrapper.Verify(w => w.GetVoicemeeterVersion(out It.Ref<int>.IsAny), Times.Never)
        );
    }
}