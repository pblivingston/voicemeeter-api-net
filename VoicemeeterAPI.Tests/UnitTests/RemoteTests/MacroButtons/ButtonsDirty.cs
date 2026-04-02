using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.MacroButtons;

public class ButtonsDirty : MockRemote
{
    [Fact]
    public void ReturnsFalse_WhenResponseIsOk()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;

        MockLogin_Ok(kind, version);

        MockWrapper.Setup(w => w.MacroButtonIsDirty()).Returns(Response.Ok);

        var result = Remote.ButtonsDirty();

        Assert.Multiple(
            () => Assert.False(result),
            () => MockWrapper.Verify(w => w.MacroButtonIsDirty(), Times.Exactly(2))
        );
    }

    [Fact]
    public void ReturnsTrue_WhenResponseIsDirty()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;

        MockLogin_Ok(kind, version);

        MockWrapper.Setup(w => w.MacroButtonIsDirty()).Returns(Response.Dirty);

        var result = Remote.ButtonsDirty();

        Assert.Multiple(
            () => Assert.True(result),
            () => MockWrapper.Verify(w => w.MacroButtonIsDirty(), Times.Exactly(2))
        );
    }

    [Fact]
    public void ThrowsException_Remote_WhenUnexpectedResponse()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;

        MockLogin_Ok(kind, version);

        MockWrapper.Setup(w => w.MacroButtonIsDirty()).Returns(Response.Error);

        var ex = Assert.Throws<RemoteException>(() => Remote.ButtonsDirty());

        Assert.Multiple(
            () => Assert.Equal("[VoicemeeterAPI] Remote Error: ButtonsDirty failed - Error", ex.Message),
            () => MockWrapper.Verify(w => w.MacroButtonIsDirty(), Times.Exactly(2))
        );
    }

    [Fact]
    public void ThrowsException_RemoteAccess_WhenLoginStatusNotOk()
    {
        MockLogin_VoicemeeterNotRunning();

        var ex = Assert.Throws<RemoteAccessException>(() => Remote.ButtonsDirty());

        Assert.Multiple(
            () => Assert.Equal("ButtonsDirty", ex.Method),
            () => Assert.Equal(LoginResponse.VoicemeeterNotRunning, ex.LoginStatus),
            () => MockWrapper.Verify(w => w.MacroButtonIsDirty(), Times.Never)
        );
    }

    [Fact]
    public void ThrowsException_ObjectDisposed_WhenRemoteDisposed()
    {
        Remote.Dispose();

        var ex = Assert.Throws<ObjectDisposedException>(() => Remote.ButtonsDirty());

        Assert.Multiple(
            () => Assert.Equal("Remote", ex.ObjectName),
            () => MockWrapper.Verify(w => w.MacroButtonIsDirty(), Times.Never)
        );
    }
}