using PBLivingston.VoicemeeterAPI.Messages;
using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests;

public class GetParameters : MockRemote
{
    #region ParamsDirty

    [Fact]
    public void ParamsDirty_ReturnsFalse_WhenResponseIsOk()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;

        MockLogin_Ok(kind, version);

        MockWrapper.Setup(w => w.IsParametersDirty()).Returns(Response.Ok);

        var result = Remote.ParamsDirty();

        Assert.False(result);
        MockWrapper.Verify(w => w.IsParametersDirty(), Times.Exactly(2));
    }

    [Fact]
    public void ParamsDirty_ReturnsTrue_WhenResponseIsDirty()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;

        MockLogin_Ok(kind, version);

        MockWrapper.Setup(w => w.IsParametersDirty()).Returns(Response.Dirty);

        var result = Remote.ParamsDirty();

        Assert.True(result);
        MockWrapper.Verify(w => w.IsParametersDirty(), Times.Exactly(2));
    }

    [Fact]
    public void ParamsDirty_ThrowsException_Remote_WhenUnexpectedResponse()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;

        MockLogin_Ok(kind, version);

        MockWrapper.Setup(w => w.IsParametersDirty()).Returns(Response.Error);

        var ex = Assert.Throws<RemoteException>(() => Remote.ParamsDirty());
        Assert.Equal("[VoicemeeterAPI] Remote Error: ParamsDirty failed - Error", ex.Message);
        MockWrapper.Verify(w => w.IsParametersDirty(), Times.Exactly(2));
    }

    [Fact]
    public void ParamsDirty_ThrowsException_RemoteAccess_WhenLoginStatusNotOk()
    {
        MockLogin_VoicemeeterNotRunning();

        var ex = Assert.Throws<RemoteAccessException>(() => Remote.ParamsDirty());
        Assert.Multiple(
            () => Assert.Equal("ParamsDirty", ex.Method),
            () => Assert.Equal(LoginResponse.VoicemeeterNotRunning, ex.LoginStatus)
        );
        MockWrapper.Verify(w => w.IsParametersDirty(), Times.Never);
    }

    [Fact]
    public void ParamsDirty_ThrowsException_ObjectDisposed_WhenRemoteIsDisposed()
    {
        Remote.Dispose();

        var ex = Assert.Throws<ObjectDisposedException>(() => Remote.ParamsDirty());
        Assert.Equal("Remote", ex.ObjectName);
        MockWrapper.Verify(w => w.IsParametersDirty(), Times.Never);
    }

    #endregion
}