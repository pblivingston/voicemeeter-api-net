namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.GetParameters;

using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;
using PBLivingston.VoicemeeterAPI.Utilities;

public class GetParamGeneric : MockRemote
{
    [Fact]
    public void FloatCallsCorrectOverload()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;
        var param = "Mock.Param";
        var value = 0.75f;

        this.MockWrapper.Setup(w => w.GetParameter_Float(param)).Returns((Response.Ok, value));

        this.MockLogin(kind, version);

        var result = ((IRemote)this.Remote).GetParam<float>(param);

        Assert.Multiple(
            () => Assert.Equal(value, result),
            () => this.MockWrapper.Verify(w => w.GetParameter_Float(param), Times.Once())
        );
    }

    [Fact]
    public void IntCallsCorrectOverload()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;
        var param = "Mock.Param";
        var value = 42.0f;
        var expected = 42;

        this.MockWrapper.Setup(w => w.GetParameter_Float(param)).Returns((Response.Ok, value));

        this.MockLogin(kind, version);

        var result = ((IRemote)this.Remote).GetParam<int>(param);

        Assert.Multiple(
            () => Assert.Equal(expected, result),
            () => this.MockWrapper.Verify(w => w.GetParameter_Float(param), Times.Once())
        );
    }

    [Fact]
    public void BoolCallsCorrectOverload()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;
        var param = "Mock.Param";
        var value = 1.0f;
        var expected = true;

        this.MockWrapper.Setup(w => w.GetParameter_Float(param)).Returns((Response.Ok, value));

        this.MockLogin(kind, version);

        var result = ((IRemote)this.Remote).GetParam<bool>(param);

        Assert.Multiple(
            () => Assert.Equal(expected, result),
            () => this.MockWrapper.Verify(w => w.GetParameter_Float(param), Times.Once())
        );
    }

    [Fact]
    public void StringCallsCorrectOverload()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;
        var param = "Mock.Param";
        var value = "Test String";

        this.MockWrapper.Setup(w => w.GetParameter_String(param)).Returns((Response.Ok, value));

        this.MockLogin(kind, version);

        var result = ((IRemote)this.Remote).GetParam<string>(param);

        Assert.Multiple(
            () => Assert.Equal(value, result),
            () => this.MockWrapper.Verify(w => w.GetParameter_String(param), Times.Once())
        );
    }

    [Fact]
    public void ThrowsExceptionTypeNotSupportedWhenTypeNotSupported()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;
        var param = "Mock.Param";

        this.MockLogin(kind, version);

        var ex = Assert.Throws<TypeNotSupportedException>(() => ((IRemote)this.Remote).GetParam<DateTime>(param));

        Assert.Multiple(
            () => Assert.Equal("T", ex.ParamName),
            () => Assert.Equal(typeof(DateTime), ex.Type),
            () => Assert.Equal(SupportedTypes.ParamTypes, ex.SupportedTypes),
            () => this.MockWrapper.Verify(w => w.GetParameter_Float(param), Times.Never()),
            () => this.MockWrapper.Verify(w => w.GetParameter_String(param), Times.Never())
        );
    }
}
