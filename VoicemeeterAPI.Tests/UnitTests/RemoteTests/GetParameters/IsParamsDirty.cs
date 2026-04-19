using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.GetParameters;

public class IsParamsDirty : MockRemote
{
    [Fact]
    public void ReturnsFalse_WhenResponseIsOk()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;

        MockLogin_Ok(kind, version);

        MockWrapper.Setup(w => w.IsParametersDirty()).Returns(Response.Ok);

        var result = Remote.IsParamsDirty();

        Assert.Multiple(
            () => Assert.False(result),
            () => MockWrapper.Verify(w => w.IsParametersDirty(), Times.Exactly(2))
        );
    }

    [Fact]
    public void ReturnsTrue_WhenResponseIsDirty()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;

        MockLogin_Ok(kind, version);

        MockWrapper.Setup(w => w.IsParametersDirty()).Returns(Response.Dirty);

        var result = Remote.IsParamsDirty();

        Assert.Multiple(
            () => Assert.True(result),
            () => MockWrapper.Verify(w => w.IsParametersDirty(), Times.Exactly(2))
        );
    }

    [Fact]
    public void ThrowsException_Remote_WhenUnexpectedResponse()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;

        MockLogin_Ok(kind, version);

        MockWrapper.Setup(w => w.IsParametersDirty()).Returns(Response.Error);

        var ex = Assert.Throws<RemoteException>(() => Remote.IsParamsDirty());

        Assert.Multiple(
            () => Assert.Equal("[VoicemeeterAPI] Remote Error: IsParamsDirty failed - Error", ex.Message),
            () => MockWrapper.Verify(w => w.IsParametersDirty(), Times.Exactly(2))
        );
    }

    [Fact]
    public void ThrowsException_RemoteAccess_WhenLoginStatusNotOk()
    {
        MockLogin_VoicemeeterNotRunning();

        var ex = Assert.Throws<RemoteAccessException>(() => Remote.IsParamsDirty());

        Assert.Multiple(
            () => Assert.Equal("IsParamsDirty", ex.Method),
            () => Assert.Equal(LoginResponse.VoicemeeterNotRunning, ex.LoginStatus),
            () => MockWrapper.Verify(w => w.IsParametersDirty(), Times.Never)
        );
    }

    [Fact]
    public void ThrowsException_ObjectDisposed_WhenRemoteIsDisposed()
    {
        Remote.Dispose();

        var ex = Assert.Throws<ObjectDisposedException>(() => Remote.IsParamsDirty());

        Assert.Multiple(
            () => Assert.Equal("Remote", ex.ObjectName),
            () => MockWrapper.Verify(w => w.IsParametersDirty(), Times.Never)
        );
    }
}