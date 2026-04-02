using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.GetParameters;

public class GetParamFloat : MockRemote
{
    [Fact]
    public void ReturnsValue_WhenResponse_Ok()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;
        var param = "Mock.Param";
        var value = 0.75f;

        MockWrapper.Setup(w => w.GetParameter(param, out value)).Returns(Response.Ok);

        MockLogin_Ok(kind, version);

        Remote.GetParam(param, out float result);

        Assert.Multiple(
            () => Assert.Equal(value, result),
            () => MockWrapper.Verify(w => w.GetParameter(param, out value), Times.Once)
        );
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
        var value = 0.75f;

        MockWrapper.Setup(w => w.GetParameter(param, out value)).Returns(response);

        MockLogin_Ok(kind, version);

        var ex = Assert.Throws<GetParamException<float>>(() => Remote.GetParam(param, out float _));

        Assert.Multiple(
            () => Assert.Equal(response, ex.Response),
            () => Assert.Equal(param, ex.Param),
            () => Assert.Equal(value, ex.Value),
            () => Assert.Equal(typeof(float), ex.ExpectedType),
            () => MockWrapper.Verify(w => w.GetParameter(param, out value), Times.Once)
        );
    }

    [Fact]
    public void ThrowsException_RemoteAccess_WhenLoginStatus_NotOk()
    {
        var param = "Mock.Param";

        MockLogin_VoicemeeterNotRunning();

        var ex = Assert.Throws<RemoteAccessException>(() => Remote.GetParam(param, out float _));

        Assert.Multiple(
            () => Assert.Equal("GetParam", ex.Method),
            () => Assert.Equal(LoginResponse.VoicemeeterNotRunning, ex.LoginStatus),
            () => MockWrapper.Verify(w => w.GetParameter(param, out It.Ref<float>.IsAny), Times.Never)
        );
    }

    [Fact]
    public void ThrowsException_ObjectDisposed_WhenRemoteDisposed()
    {
        var param = "Mock.Param";

        Remote.Dispose();

        var ex = Assert.Throws<ObjectDisposedException>(() => Remote.GetParam(param, out float _));

        Assert.Multiple(
            () => Assert.Equal("Remote", ex.ObjectName),
            () => MockWrapper.Verify(w => w.GetParameter(param, out It.Ref<float>.IsAny), Times.Never)
        );
    }

    [Theory]
    [InlineData(0.0f, 0)]
    [InlineData(42.0f, 42)]
    [InlineData(100.0f, 100)]
    public void Int_ReturnsValue_WhenValue_WholeNumber_NotNegative(float value, int expected)
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;
        var param = "Mock.IntParam";

        MockWrapper.Setup(w => w.GetParameter(param, out value)).Returns(Response.Ok);

        MockLogin_Ok(kind, version);

        Remote.GetParam(param, out int result);

        Assert.Multiple(
            () => Assert.Equal(expected, result),
            () => MockWrapper.Verify(w => w.GetParameter(param, out value), Times.Once)
        );
    }

    [Theory]
    [InlineData(42.5f)]
    [InlineData(-42.0f)]
    public void Int_ThrowsException_GetParam_WhenValue_NotWholeNumber_Negative(float value)
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;
        var param = "Mock.IntParam";

        MockWrapper.Setup(w => w.GetParameter(param, out value)).Returns(Response.Ok);

        MockLogin_Ok(kind, version);

        var ex = Assert.Throws<GetParamException<float>>(() => Remote.GetParam(param, out int _));

        Assert.Multiple(
            () => Assert.Equal(Response.TypeMismatch, ex.Response),
            () => Assert.Equal(param, ex.Param),
            () => Assert.Equal(value, ex.Value),
            () => Assert.Equal(typeof(int), ex.ExpectedType),
            () => MockWrapper.Verify(w => w.GetParameter(param, out value), Times.Once)
        );
    }

    [Theory]
    [InlineData(0.0f, false)]
    [InlineData(1.0f, true)]
    public void Bool_ReturnsValue_WhenValue_ZeroOrOne(float value, bool expected)
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;
        var param = "Mock.BoolParam";

        MockWrapper.Setup(w => w.GetParameter(param, out value)).Returns(Response.Ok);

        MockLogin_Ok(kind, version);

        Remote.GetParam(param, out bool result);

        Assert.Multiple(
            () => Assert.Equal(expected, result),
            () => MockWrapper.Verify(w => w.GetParameter(param, out value), Times.Once)
        );
    }

    [Theory]
    [InlineData(-1.0f)]
    [InlineData(2.0f)]
    [InlineData(0.5f)]
    public void Bool_ThrowsException_GetParam_WhenValue_NotZeroOrOne(float value)
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;
        var param = "Mock.BoolParam";

        MockWrapper.Setup(w => w.GetParameter(param, out value)).Returns(Response.Ok);

        MockLogin_Ok(kind, version);

        var ex = Assert.Throws<GetParamException<float>>(() => Remote.GetParam(param, out bool _));

        Assert.Multiple(
            () => Assert.Equal(Response.TypeMismatch, ex.Response),
            () => Assert.Equal(param, ex.Param),
            () => Assert.Equal(value, ex.Value),
            () => Assert.Equal(typeof(bool), ex.ExpectedType),
            () => MockWrapper.Verify(w => w.GetParameter(param, out value), Times.Once)
        );
    }
}