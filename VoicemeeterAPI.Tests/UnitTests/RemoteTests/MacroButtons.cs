using PBLivingston.VoicemeeterAPI.Messages;
using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests;

public class MacroButtons : MockRemote
{
    #region ButtonsDirty

    [Fact]
    public void ButtonsDirty_ReturnsFalse_WhenResponseIsOk()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;

        MockOkLogin(kind, version);

        MockWrapper.Setup(w => w.MacroButtonIsDirty()).Returns(Response.Ok);

        var result = Remote.ButtonsDirty();

        Assert.False(result);
        MockWrapper.Verify(w => w.MacroButtonIsDirty(), Times.Exactly(2));
    }

    [Fact]
    public void ButtonsDirty_ReturnsTrue_WhenResponseIsDirty()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;

        MockOkLogin(kind, version);

        MockWrapper.Setup(w => w.MacroButtonIsDirty()).Returns(Response.Dirty);

        var result = Remote.ButtonsDirty();

        Assert.True(result);
        MockWrapper.Verify(w => w.MacroButtonIsDirty(), Times.Exactly(2));
    }

    [Fact]
    public void ButtonsDirty_ThrowsException_Remote_WhenUnexpectedResponse()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;

        MockOkLogin(kind, version);

        MockWrapper.Setup(w => w.MacroButtonIsDirty()).Returns(Response.Error);

        var ex = Assert.Throws<RemoteException>(() => Remote.ButtonsDirty());
        Assert.Equal("[VoicemeeterAPI] Remote Error: ButtonsDirty failed - Error", ex.Message);
        MockWrapper.Verify(w => w.MacroButtonIsDirty(), Times.Exactly(2));
    }

    [Fact]
    public void ButtonsDirty_ThrowsException_RemoteAccess_WhenLoginStatusNotOk()
    {
        var kind = (int)Kind.None;
        var version = 0;

        MockWrapper.Setup(w => w.Login()).Returns(LoginResponse.VoicemeeterNotRunning);
        MockWrapper.Setup(w => w.GetVoicemeeterType(out kind)).Returns(InfoResponse.NoServer);
        MockWrapper.Setup(w => w.GetVoicemeeterVersion(out version)).Returns(InfoResponse.NoServer);

        Remote.Login(timeoutMs: 10);
        Assert.Equal(LoginResponse.VoicemeeterNotRunning, Remote.LoginStatus);

        var ex = Assert.Throws<RemoteAccessException>(() => Remote.ButtonsDirty());
        Assert.Multiple(
            () => Assert.Equal("ButtonsDirty", ex.Method),
            () => Assert.Equal(LoginResponse.VoicemeeterNotRunning, ex.LoginStatus)
        );
        MockWrapper.Verify(w => w.MacroButtonIsDirty(), Times.Never);
    }

    [Fact]
    public void ButtonsDirty_ThrowsException_ObjectDisposed_WhenRemoteDisposed()
    {
        Remote.Dispose();

        var ex = Assert.Throws<ObjectDisposedException>(() => Remote.ButtonsDirty());
        Assert.Equal("Remote", ex.ObjectName);
        MockWrapper.Verify(w => w.MacroButtonIsDirty(), Times.Never);
    }

    #endregion
}