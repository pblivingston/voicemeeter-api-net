namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.MacroButtons;

using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;

public class IsMacroButtonsRunning : MockRemote
{
    [Fact]
    public void ReturnsTrueWhenResponseIsOk()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;

        this.MockLoginOk(kind, version);

        var result = this.Remote.IsButtonsRunning();

        Assert.Multiple(
            () => Assert.True(result),
            () => this.MockWrapper.Verify(w => w.MacroButtonIsRunning(), Times.Exactly(3))
        );
    }

    [Fact]
    public void ReturnsFalseWhenResponseIsNotRunning()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;

        this.MockLoginMacroButtonsNotRunning(kind, version);

        var result = this.Remote.IsButtonsRunning();

        Assert.Multiple(
            () => Assert.False(result),
            () => this.MockWrapper.Verify(w => w.MacroButtonIsRunning(), Times.Exactly(3))
        );
    }

    [Fact]
    public void ThrowsExceptionRemoteWhenUnexpectedResponse()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;

        this.MockLoginOk(kind, version);

        this.MockWrapper.Setup(w => w.MacroButtonIsRunning()).Returns(RunResponse.NotInstalled);

        var ex = Assert.Throws<RemoteException<RunResponse>>(() => this.Remote.IsButtonsRunning());

        Assert.Multiple(
            () => Assert.Equal(RunResponse.NotInstalled, ex.Response),
            () => this.MockWrapper.Verify(w => w.MacroButtonIsRunning(), Times.Exactly(3))
        );
    }

    [Fact]
    public void ThrowsExceptionAccessDeniedWhenLoginStatusLoggedOut()
    {
        var ex = Assert.Throws<AccessDeniedException>(() => this.Remote.IsButtonsRunning());

        Assert.Multiple(
            () => Assert.Equal(LoginResponse.LoggedOut, ex.LoginStatus),
            () => this.MockWrapper.Verify(w => w.MacroButtonIsRunning(), Times.Never())
        );
    }

    [Fact]
    public void ThrowsExceptionObjectDisposedWhenRemoteDisposed()
    {
        this.Remote.Dispose();

        Assert.Multiple(
            () => Assert.Throws<ObjectDisposedException>(() => this.Remote.IsButtonsRunning()),
            () => this.MockWrapper.Verify(w => w.MacroButtonIsRunning(), Times.Never())
        );
    }
}