namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests;

using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;

public class GetConnectionState : MockRemote
{
    [Fact]
    public void ReturnsExpectedConnectionStateWhenAllConditionsMet()
    {
        var kind = (int)Kind.Banana;
        var version = 0x0201_0202;
        var expectedState = new ConnectionState(LoginResponse.Ok, RunResponse.Ok, (Kind)kind, (VmVersion)version);

        this.MockLogin(kind, version);

        Assert.Multiple(
            () => Assert.Equal(expectedState, this.Remote.GetConnectionState()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterType(), Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(), Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(App.MacroButtons), Times.Exactly(2))
        );
    }

    [Fact]
    public void ReturnsLastConnectionStateWhenLoggedOut()
    {
        var kind = (int)Kind.Banana;
        var version = 0x0201_0202;
        var expectedState = new ConnectionState(LoginResponse.LoggedOut, RunResponse.Ok, (Kind)kind, (VmVersion)version);

        this.MockWrapper.Setup(w => w.Logout()).Returns(LoginResponse.Ok);

        this.MockLogin(kind, version);

        this.Remote.Logout();

        Assert.Multiple(
            () => Assert.Equal(expectedState, this.Remote.GetConnectionState()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterType(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(App.MacroButtons), Times.Once())
        );
    }

    [Fact]
    public void ThrowsExceptionKindMismatchWhenKindAndVersionDoNotMatch()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0301_0202;

        this.MockLogin();

        this.MockWrapper.Setup(w => w.GetVoicemeeterType()).Returns((InfoResponse.Ok, kind));
        this.MockWrapper.Setup(w => w.GetVoicemeeterVersion()).Returns((InfoResponse.Ok, version));

        var ex = Assert.Throws<KindMismatchException>(() => this.Remote.GetConnectionState());

        Assert.Multiple(
            () => Assert.Equal((Kind)kind, ex.ReturnedKind),
            () => Assert.Equal((VmVersion)version, ex.ReturnedVersion),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterType(), Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(), Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(App.MacroButtons), Times.Exactly(2))
        );
    }

    [Fact]
    public void ThrowsExceptionObjectDisposedWhenRemoteDisposed()
    {
        this.Remote.Dispose();

        Assert.Multiple(
            () => Assert.Throws<ObjectDisposedException>(() => this.Remote.GetConnectionState()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterType(), Times.Never()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(), Times.Never()),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(App.MacroButtons), Times.Never())
        );
    }
}
