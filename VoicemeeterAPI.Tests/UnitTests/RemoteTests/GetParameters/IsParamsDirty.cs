namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.GetParameters;

using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;

public class IsParamsDirty : MockRemote
{
    [Fact]
    public void ReturnsFalseWhenResponseIsOk()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;

        this.MockLoginOk(kind, version);

        this.MockWrapper.Setup(w => w.IsParametersDirty()).Returns(Response.Ok);

        var result = this.Remote.IsParamsDirty();

        Assert.Multiple(
            () => Assert.False(result),
            () => this.MockWrapper.Verify(w => w.IsParametersDirty(), Times.Exactly(2))
        );
    }

    [Fact]
    public void ReturnsTrueWhenResponseIsDirty()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;

        this.MockLoginOk(kind, version);

        this.MockWrapper.Setup(w => w.IsParametersDirty()).Returns(Response.Dirty);

        var result = this.Remote.IsParamsDirty();

        Assert.Multiple(
            () => Assert.True(result),
            () => this.MockWrapper.Verify(w => w.IsParametersDirty(), Times.Exactly(2))
        );
    }

    [Fact]
    public void ThrowsExceptionRemoteWhenUnexpectedResponse()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;

        this.MockLoginOk(kind, version);

        this.MockWrapper.Setup(w => w.IsParametersDirty()).Returns(Response.Error);

        var ex = Assert.Throws<RemoteException<Response>>(() => this.Remote.IsParamsDirty());

        Assert.Multiple(
            () => Assert.Equal(Response.Error, ex.Response),
            () => this.MockWrapper.Verify(w => w.IsParametersDirty(), Times.Exactly(2))
        );
    }

    [Fact]
    public void ThrowsExceptionAccessDeniedWhenLoginStatusNotOk()
    {
        this.MockLoginVoicemeeterNotRunning();

        var ex = Assert.Throws<AccessDeniedException>(() => this.Remote.IsParamsDirty());

        Assert.Multiple(
            () => Assert.Equal(LoginResponse.VoicemeeterNotRunning, ex.LoginStatus),
            () => this.MockWrapper.Verify(w => w.IsParametersDirty(), Times.Never())
        );
    }

    [Fact]
    public void ThrowsExceptionObjectDisposedWhenRemoteDisposed()
    {
        this.Remote.Dispose();

        Assert.Multiple(
            () => Assert.Throws<ObjectDisposedException>(() => this.Remote.IsParamsDirty()),
            () => this.MockWrapper.Verify(w => w.IsParametersDirty(), Times.Never())
        );
    }
}