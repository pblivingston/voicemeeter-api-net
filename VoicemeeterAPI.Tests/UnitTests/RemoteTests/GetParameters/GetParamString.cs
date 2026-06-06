namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.GetParameters;

using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;

public class GetParamString : MockRemote
{
    [Fact]
    public void ReturnsValueWhenResponseOk()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;
        var param = "Mock.Param";
        var value = "Test String";

        this.MockWrapper.Setup(w => w.GetParameter_String(param)).Returns((Response.Ok, value));

        this.MockLogin(kind, version);

        this.Remote.GetParam(param, out string result);

        Assert.Multiple(
            () => Assert.Equal(value, result),
            () => this.MockWrapper.Verify(w => w.GetParameter_String(param), Times.Once())
        );
    }

    [Theory]
    [InlineData(Response.Error)]
    [InlineData(Response.UnknownParameter)]
    [InlineData(Response.StructureMismatch)]
    public void ThrowsExceptionGetParamWhenResponseNotOk(Response response)
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;
        var param = "Mock.Param";
        var value = "Test String";

        this.MockWrapper.Setup(w => w.GetParameter_String(param)).Returns((response, value));

        this.MockLogin(kind, version);

        var ex = Assert.Throws<GetParamException<string>>(() => this.Remote.GetParam(param, out string _));

        Assert.Multiple(
            () => Assert.Equal(response, ex.Response),
            () => Assert.Equal(param, ex.VmParam),
            () => Assert.Equal(value, ex.ReturnedValue),
            () => Assert.Equal(typeof(string), ex.ExpectedType),
            () => this.MockWrapper.Verify(w => w.GetParameter_String(param), Times.Once())
        );
    }

    [Fact]
    public void ThrowsExceptionAccessDeniedWhenLoginStatusNotOk()
    {
        var param = "Mock.Param";

        this.MockLogin();

        var ex = Assert.Throws<AccessDeniedException>(() => this.Remote.GetParam(param, out string _));

        Assert.Multiple(
            () => Assert.Equal(LoginResponse.VoicemeeterNotRunning, ex.LoginStatus),
            () => this.MockWrapper.Verify(w => w.GetParameter_String(param), Times.Never())
        );
    }

    [Fact]
    public void ThrowsExceptionObjectDisposedWhenRemoteDisposed()
    {
        var param = "Mock.Param";

        this.Remote.Dispose();

        Assert.Multiple(
            () => Assert.Throws<ObjectDisposedException>(() => this.Remote.GetParam(param, out string _)),
            () => this.MockWrapper.Verify(w => w.GetParameter_String(param), Times.Never())
        );
    }
}
