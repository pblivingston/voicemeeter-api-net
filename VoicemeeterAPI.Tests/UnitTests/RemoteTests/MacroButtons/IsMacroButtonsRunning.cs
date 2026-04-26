using PBLivingston.VoicemeeterAPI.EventManagement.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.MacroButtons;

public class IsMacroButtonsRunning : MockRemote
{
    [Fact]
    public void ReturnsTrue_WhenResponseIsOk()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;

        MockLogin_Ok(kind, version);

        var result = Remote.IsMacroButtonsRunning();

        Assert.Multiple(
            () => Assert.True(result),
            () => MockWrapper.Verify(w => w.MacroButtonIsRunning(), Times.Exactly(3))
        );
    }

    [Fact]
    public void ReturnsFalse_WhenResponseIsNotRunning()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;

        MockLogin_MacroButtonsNotRunning(kind, version);

        var result = Remote.IsMacroButtonsRunning();

        Assert.Multiple(
            () => Assert.False(result),
            () => MockWrapper.Verify(w => w.MacroButtonIsRunning(), Times.Exactly(3))
        );
    }

    [Fact]
    public void ThrowsException_Remote_WhenUnexpectedResponse()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;

        MockLogin_Ok(kind, version);

        MockWrapper.Setup(w => w.MacroButtonIsRunning()).Returns(RunResponse.NotInstalled);

        var ex = Assert.Throws<RemoteException<RunResponse>>(() => Remote.IsMacroButtonsRunning());

        Assert.Multiple(
            () => Assert.Equal(RunResponse.NotInstalled, ex.Response),
            () => MockWrapper.Verify(w => w.MacroButtonIsRunning(), Times.Exactly(3))
        );
    }

    [Fact]
    public void ThrowsException_AccessDenied_WhenLoginStatusLoggedOut()
    {
        var ex = Assert.Throws<AccessDeniedException>(() => Remote.IsMacroButtonsRunning());

        Assert.Multiple(
            () => Assert.Equal(LoginResponse.LoggedOut, ex.LoginStatus),
            () => MockWrapper.Verify(w => w.MacroButtonIsRunning(), Times.Never())
        );
    }

    [Fact]
    public void ThrowsException_ObjectDisposed_WhenRemoteDisposed()
    {
        Remote.Dispose();

        var ex = Assert.Throws<ObjectDisposedException>(() => Remote.IsMacroButtonsRunning());

        Assert.Multiple(
            () => Assert.Equal("Remote", ex.ObjectName),
            () => MockWrapper.Verify(w => w.MacroButtonIsRunning(), Times.Never())
        );
    }
}