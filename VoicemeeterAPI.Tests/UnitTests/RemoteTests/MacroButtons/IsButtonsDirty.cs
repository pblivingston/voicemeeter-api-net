namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.MacroButtons;

using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;

public class IsButtonsDirty : MockRemote
{
    [Fact]
    public void ReturnsFalseWhenResponseIsOk()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;

        this.MockLogin(kind, version);

        this.MockWrapper.Setup(w => w.MacroButtonIsDirty()).Returns(Response.Ok);

        var result = this.Remote.IsButtonsDirty();

        Assert.Multiple(
            () => Assert.False(result),
            () => this.MockWrapper.Verify(w => w.MacroButtonIsDirty(), Times.Once())
        );
    }

    [Fact]
    public void ReturnsTrueWhenResponseIsDirty()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;

        this.MockLogin(kind, version);

        this.MockWrapper.Setup(w => w.MacroButtonIsDirty()).Returns(Response.Dirty);

        var result = this.Remote.IsButtonsDirty();

        Assert.Multiple(
            () => Assert.True(result),
            () => this.MockWrapper.Verify(w => w.MacroButtonIsDirty(), Times.Once())
        );
    }

    [Fact]
    public void ThrowsExceptionRemoteWhenUnexpectedResponse()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;

        this.MockLogin(kind, version);

        this.MockWrapper.Setup(w => w.MacroButtonIsDirty()).Returns(Response.Error);

        var ex = Assert.Throws<RemoteException<Response>>(() => this.Remote.IsButtonsDirty());

        Assert.Multiple(
            () => Assert.Equal(Response.Error, ex.Response),
            () => this.MockWrapper.Verify(w => w.MacroButtonIsDirty(), Times.Once())
        );
    }

    [Fact]
    public void ThrowsExceptionAccessDeniedWhenLoginStatusNotOk()
    {
        this.MockLogin();

        var ex = Assert.Throws<AccessDeniedException>(() => this.Remote.IsButtonsDirty());

        Assert.Multiple(
            () => Assert.Equal(LoginResponse.VoicemeeterNotRunning, ex.LoginStatus),
            () => this.MockWrapper.Verify(w => w.MacroButtonIsDirty(), Times.Never())
        );
    }

    [Fact]
    public void ThrowsExceptionObjectDisposedWhenRemoteDisposed()
    {
        this.Remote.Dispose();

        Assert.Multiple(
            () => Assert.Throws<ObjectDisposedException>(() => this.Remote.IsButtonsDirty()),
            () => this.MockWrapper.Verify(w => w.MacroButtonIsDirty(), Times.Never())
        );
    }
}
