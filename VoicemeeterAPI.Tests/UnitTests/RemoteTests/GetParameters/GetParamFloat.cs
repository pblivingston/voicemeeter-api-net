namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.GetParameters;

using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;

public class GetParamFloat : MockRemote
{
    [Fact]
    public void ReturnsValueWhenResponseOk()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;
        var param = "Mock.Param";
        var value = 0.75f;

        this.MockWrapper.Setup(w => w.GetParameter_Float(param)).Returns((Response.Ok, value));

        this.MockLogin(kind, version);

        this.Remote.GetParam(param, out float result);

        Assert.Multiple(
            () => Assert.Equal(value, result),
            () => this.MockWrapper.Verify(w => w.GetParameter_Float(param), Times.Once())
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
        var value = 0.75f;

        this.MockWrapper.Setup(w => w.GetParameter_Float(param)).Returns((response, value));

        this.MockLogin(kind, version);

        var ex = Assert.Throws<GetParamException<float>>(() => this.Remote.GetParam(param, out float _));

        Assert.Multiple(
            () => Assert.Equal(response, ex.Response),
            () => Assert.Equal(param, ex.VmParam),
            () => Assert.Equal(value, ex.ReturnedValue),
            () => Assert.Equal(typeof(float), ex.ExpectedType),
            () => this.MockWrapper.Verify(w => w.GetParameter_Float(param), Times.Once())
        );
    }

    [Fact]
    public void ThrowsExceptionAccessDeniedWhenLoginStatusNotOk()
    {
        var param = "Mock.Param";

        this.MockLogin();

        var ex = Assert.Throws<AccessDeniedException>(() => this.Remote.GetParam(param, out float _));

        Assert.Multiple(
            () => Assert.Equal(LoginResponse.VoicemeeterNotRunning, ex.LoginStatus),
            () => this.MockWrapper.Verify(w => w.GetParameter_Float(param), Times.Never())
        );
    }

    [Fact]
    public void ThrowsExceptionObjectDisposedWhenRemoteDisposed()
    {
        var param = "Mock.Param";

        this.Remote.Dispose();

        Assert.Multiple(
            () => Assert.Throws<ObjectDisposedException>(() => this.Remote.GetParam(param, out float _)),
            () => this.MockWrapper.Verify(w => w.GetParameter_Float(param), Times.Never())
        );
    }

    [Theory]
    [InlineData(0.0f, 0)]
    [InlineData(42.0f, 42)]
    [InlineData(100.0f, 100)]
    public void IntReturnsValueWhenValueWholeNumberNotNegative(float value, int expected)
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;
        var param = "Mock.IntParam";

        this.MockWrapper.Setup(w => w.GetParameter_Float(param)).Returns((Response.Ok, value));

        this.MockLogin(kind, version);

        this.Remote.GetParam(param, out int result);

        Assert.Multiple(
            () => Assert.Equal(expected, result),
            () => this.MockWrapper.Verify(w => w.GetParameter_Float(param), Times.Once())
        );
    }

    [Theory]
    [InlineData(42.5f)]
    [InlineData(-42.0f)]
    public void IntThrowsExceptionGetParamWhenValueNotWholeNumberNegative(float value)
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;
        var param = "Mock.IntParam";

        this.MockWrapper.Setup(w => w.GetParameter_Float(param)).Returns((Response.Ok, value));

        this.MockLogin(kind, version);

        var ex = Assert.Throws<GetParamException<float>>(() => this.Remote.GetParam(param, out int _));

        Assert.Multiple(
            () => Assert.Equal(Response.TypeMismatch, ex.Response),
            () => Assert.Equal(param, ex.VmParam),
            () => Assert.Equal(value, ex.ReturnedValue),
            () => Assert.Equal(typeof(int), ex.ExpectedType),
            () => this.MockWrapper.Verify(w => w.GetParameter_Float(param), Times.Once())
        );
    }

    [Theory]
    [InlineData(0.0f, false)]
    [InlineData(1.0f, true)]
    public void BoolReturnsValueWhenValueZeroOrOne(float value, bool expected)
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;
        var param = "Mock.BoolParam";

        this.MockWrapper.Setup(w => w.GetParameter_Float(param)).Returns((Response.Ok, value));

        this.MockLogin(kind, version);

        this.Remote.GetParam(param, out bool result);

        Assert.Multiple(
            () => Assert.Equal(expected, result),
            () => this.MockWrapper.Verify(w => w.GetParameter_Float(param), Times.Once())
        );
    }

    [Theory]
    [InlineData(-1.0f)]
    [InlineData(2.0f)]
    [InlineData(0.5f)]
    public void BoolThrowsExceptionGetParamWhenValueNotZeroOrOne(float value)
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;
        var param = "Mock.BoolParam";

        this.MockWrapper.Setup(w => w.GetParameter_Float(param)).Returns((Response.Ok, value));

        this.MockLogin(kind, version);

        var ex = Assert.Throws<GetParamException<float>>(() => this.Remote.GetParam(param, out bool _));

        Assert.Multiple(
            () => Assert.Equal(Response.TypeMismatch, ex.Response),
            () => Assert.Equal(param, ex.VmParam),
            () => Assert.Equal(value, ex.ReturnedValue),
            () => Assert.Equal(typeof(bool), ex.ExpectedType),
            () => this.MockWrapper.Verify(w => w.GetParameter_Float(param), Times.Once())
        );
    }
}
