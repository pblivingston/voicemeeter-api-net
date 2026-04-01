using PBLivingston.VoicemeeterAPI.Messages;
using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.GetParameters;

public class GetParamString : MockRemote
{
    [Fact]
    public void ReturnsValue_WhenResponse_Ok()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;
        var param = "Mock.Param";
        var value = "Test String";

        MockWrapper.Setup(w => w.GetParameter(param, out value)).Returns(Response.Ok);

        MockLogin_Ok(kind, version);

        Remote.GetParam(param, out string result);

        Assert.Equal(value, result);
        MockWrapper.Verify(w => w.GetParameter(param, out value), Times.Once);
    }

    [Theory]
    [InlineData(Response.Error)]
    [InlineData(Response.UnknownParameter)]
    [InlineData(Response.StructureMismatch)]
    public void ThrowsException_GetParam_WhenResponse_NotOk(Response response)
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;
        var param = "Mock.Param";
        var value = "Test String";

        MockWrapper.Setup(w => w.GetParameter(param, out value)).Returns(response);

        MockLogin_Ok(kind, version);

        var ex = Assert.Throws<GetParamException<string>>(() => Remote.GetParam(param, out string _));
        Assert.Multiple(
            () => Assert.Equal(response, ex.Response),
            () => Assert.Equal(param, ex.Param),
            () => Assert.Equal(value, ex.Value),
            () => Assert.Equal(typeof(string), ex.ExpectedType)
        );
        MockWrapper.Verify(w => w.GetParameter(param, out value), Times.Once);
    }

    [Fact]
    public void ThrowsException_RemoteAccess_WhenLoginStatus_NotOk()
    {
        var param = "Mock.Param";

        MockLogin_VoicemeeterNotRunning();

        var ex = Assert.Throws<RemoteAccessException>(() => Remote.GetParam(param, out string _));
        Assert.Multiple(
            () => Assert.Equal("GetParam", ex.Method),
            () => Assert.Equal(LoginResponse.VoicemeeterNotRunning, ex.LoginStatus)
        );
        MockWrapper.Verify(w => w.GetParameter(param, out It.Ref<string>.IsAny), Times.Never);
    }

    [Fact]
    public void ThrowsException_ObjectDisposed_WhenRemote_Disposed()
    {
        var param = "Mock.Param";

        Remote.Dispose();

        var ex = Assert.Throws<ObjectDisposedException>(() => Remote.GetParam(param, out string _));
        Assert.Equal("Remote", ex.ObjectName);
        MockWrapper.Verify(w => w.GetParameter(param, out It.Ref<string>.IsAny), Times.Never);
    }
}