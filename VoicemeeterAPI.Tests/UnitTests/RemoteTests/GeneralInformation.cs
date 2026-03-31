using PBLivingston.VoicemeeterAPI.Messages;
using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests;

public class GeneralInformation : MockRemote
{
    #region GetKind

    [Fact]
    public void GetKind_ReturnsExpected_Kind_UpdatesLoginStatus_Ok_WhenVoicemeeterRunning()
    {
        var kind = (int)Kind.Banana;

        MockWrapper.Setup(w => w.GetVoicemeeterType(out kind)).Returns(InfoResponse.Ok);

        MockLogin_VoicemeeterNotRunning();

        var result = Remote.GetKind();

        Assert.Multiple(
            () => Assert.Equal(Kind.Banana, result),
            () => Assert.Equal(LoginResponse.Ok, Remote.LoginStatus)
        );
        MockWrapper.Verify(w => w.GetVoicemeeterType(out It.Ref<int>.IsAny), Times.Once);
    }

    [Fact]
    public void GetKind_ReturnsExpected_Kind_UpdatesLoginStatus_VoicemeeterNotRunning_WhenVoicemeeterNotRunning()
    {
        var kind = (int)Kind.Banana;
        var version = 0x0201_0202;

        MockLogin_Ok(kind, version);

        var noKind = (int)Kind.None;
        MockWrapper.Setup(w => w.GetVoicemeeterType(out noKind)).Returns(InfoResponse.NoServer);

        var result = Remote.GetKind();

        Assert.Multiple(
            () => Assert.Equal(Kind.None, result),
            () => Assert.Equal(LoginResponse.VoicemeeterNotRunning, Remote.LoginStatus)
        );
        MockWrapper.Verify(w => w.GetVoicemeeterType(out It.Ref<int>.IsAny), Times.Exactly(2));
    }

    [Fact]
    public void GetKind_ThrowsException_Remote_WhenUnexpectedResponse()
    {
        var kind = (int)Kind.Banana;
        var version = 0x0201_0202;

        MockLogin_Ok(kind, version);

        var noKind = (int)Kind.None;
        MockWrapper.Setup(w => w.GetVoicemeeterType(out noKind)).Returns(InfoResponse.NoClient);

        var ex = Assert.Throws<RemoteException>(() => Remote.GetKind());
        Assert.Equal("[VoicemeeterAPI] Remote Error: GetKind failed - NoClient; returned kind: None", ex.Message);
        MockWrapper.Verify(w => w.GetVoicemeeterType(out It.Ref<int>.IsAny), Times.Exactly(2));
    }

    [Fact]
    public void GetKind_ThrowsException_RemoteAccess_WhenLoginStatusLoggedOut()
    {
        var ex = Assert.Throws<RemoteAccessException>(() => Remote.GetKind());
        Assert.Equal("[VoicemeeterAPI] Remote Error: Access to GetKind denied - LoginStatus: LoggedOut", ex.Message);
        MockWrapper.Verify(w => w.GetVoicemeeterType(out It.Ref<int>.IsAny), Times.Never);
    }

    [Fact]
    public void GetKind_ThrowsException_ObjectDisposed_WhenRemoteDisposed()
    {
        Remote.Dispose();

        var ex = Assert.Throws<ObjectDisposedException>(() => Remote.GetKind());
        Assert.Equal(nameof(Remote), ex.ObjectName);
        MockWrapper.Verify(w => w.GetVoicemeeterType(out It.Ref<int>.IsAny), Times.Never);
    }

    #endregion

    #region GetVersion

    [Fact]
    public void GetVersion_ReturnsExpected_Version_UpdatesLoginStatus_Ok_WhenVoicemeeterRunning()
    {
        var version = 0x0201_0202;

        MockWrapper.Setup(w => w.GetVoicemeeterVersion(out version)).Returns(InfoResponse.Ok);

        MockLogin_VoicemeeterNotRunning();

        var result = Remote.GetVersion();

        Assert.Multiple(
            () => Assert.Equal((VmVersion)version, result),
            () => Assert.Equal(LoginResponse.Ok, Remote.LoginStatus)
        );
        MockWrapper.Verify(w => w.GetVoicemeeterVersion(out It.Ref<int>.IsAny), Times.Once);
    }

    [Fact]
    public void GetVersion_ReturnsExpected_Version_UpdatesLoginStatus_VoicemeeterNotRunning_WhenVoicemeeterNotRunning()
    {
        var kind = (int)Kind.Banana;
        var version = 0x0201_0202;

        MockLogin_Ok(kind, version);

        var noVersion = 0;
        MockWrapper.Setup(w => w.GetVoicemeeterVersion(out noVersion)).Returns(InfoResponse.NoServer);

        var result = Remote.GetVersion();

        Assert.Multiple(
            () => Assert.Equal(default, result),
            () => Assert.Equal(LoginResponse.VoicemeeterNotRunning, Remote.LoginStatus)
        );
        MockWrapper.Verify(w => w.GetVoicemeeterVersion(out It.Ref<int>.IsAny), Times.Exactly(2));
    }

    [Fact]
    public void GetVersion_ThrowsException_Remote_WhenUnexpectedResponse()
    {
        var kind = (int)Kind.Banana;
        var version = 0x0201_0202;

        MockLogin_Ok(kind, version);

        var noVersion = 0;
        MockWrapper.Setup(w => w.GetVoicemeeterVersion(out noVersion)).Returns(InfoResponse.NoClient);

        var ex = Assert.Throws<RemoteException>(() => Remote.GetVersion());
        Assert.Equal("[VoicemeeterAPI] Remote Error: GetVersion failed - NoClient; returned version: 0.0.0.0", ex.Message);
        MockWrapper.Verify(w => w.GetVoicemeeterVersion(out It.Ref<int>.IsAny), Times.Exactly(2));
    }

    [Fact]
    public void GetVersion_ThrowsException_RemoteAccess_WhenLoginStatusLoggedOut()
    {
        var ex = Assert.Throws<RemoteAccessException>(() => Remote.GetVersion());
        Assert.Equal("[VoicemeeterAPI] Remote Error: Access to GetVersion denied - LoginStatus: LoggedOut", ex.Message);
        MockWrapper.Verify(w => w.GetVoicemeeterVersion(out It.Ref<int>.IsAny), Times.Never);
    }

    [Fact]
    public void GetVersion_ThrowsException_ObjectDisposed_WhenRemoteDisposed()
    {
        Remote.Dispose();

        var ex = Assert.Throws<ObjectDisposedException>(() => Remote.GetVersion());
        Assert.Equal(nameof(Remote), ex.ObjectName);
        MockWrapper.Verify(w => w.GetVoicemeeterVersion(out It.Ref<int>.IsAny), Times.Never);
    }

    #endregion
}