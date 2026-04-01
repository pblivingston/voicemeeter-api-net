using PBLivingston.VoicemeeterAPI.Messages;
using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.GetParameters;

public class GetParamGeneric : MockRemote
{
    [Fact]
    public void Float_Calls_CorrectOverload()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;
        var param = "Mock.Param";
        var value = 0.75f;

        MockWrapper.Setup(w => w.GetParameter(param, out value)).Returns(Response.Ok);

        MockLogin_Ok(kind, version);

        ((IRemote)Remote).GetParam(param, out float result);

        Assert.Multiple(
            () => Assert.Equal(value, result),
            () => MockWrapper.Verify(w => w.GetParameter(param, out value), Times.Once)
        );
    }

    [Fact]
    public void Int_Calls_CorrectOverload()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;
        var param = "Mock.Param";
        var value = 42.0f;
        var expected = 42;

        MockWrapper.Setup(w => w.GetParameter(param, out value)).Returns(Response.Ok);

        MockLogin_Ok(kind, version);

        ((IRemote)Remote).GetParam(param, out int result);

        Assert.Multiple(
            () => Assert.Equal(expected, result),
            () => MockWrapper.Verify(w => w.GetParameter(param, out value), Times.Once)
        );
    }

    [Fact]
    public void Bool_Calls_CorrectOverload()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;
        var param = "Mock.Param";
        var value = 1.0f;
        var expected = true;

        MockWrapper.Setup(w => w.GetParameter(param, out value)).Returns(Response.Ok);

        MockLogin_Ok(kind, version);

        ((IRemote)Remote).GetParam(param, out bool result);

        Assert.Multiple(
            () => Assert.Equal(expected, result),
            () => MockWrapper.Verify(w => w.GetParameter(param, out value), Times.Once)
        );
    }

    [Fact]
    public void String_Calls_CorrectOverload()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;
        var param = "Mock.Param";
        var value = "Test String";

        MockWrapper.Setup(w => w.GetParameter(param, out value)).Returns(Response.Ok);

        MockLogin_Ok(kind, version);

        ((IRemote)Remote).GetParam(param, out string result);

        Assert.Multiple(
            () => Assert.Equal(value, result),
            () => MockWrapper.Verify(w => w.GetParameter(param, out value), Times.Once)
        );
    }

    [Fact]
    public void ThrowsException_NotSupported_WhenTypeNotSupported()
    {
        var kind = (int)Kind.Potato;
        var version = 0x0301_0202;
        var param = "Mock.Param";

        MockLogin_Ok(kind, version);

        var ex = Assert.Throws<NotSupportedException>(() => ((IRemote)Remote).GetParam(param, out DateTime _));

        Assert.Multiple(
            () => Assert.Equal($"'value' type '{typeof(DateTime)}' is not supported for GetParams", ex.Message),
            () => MockWrapper.Verify(w => w.GetParameter(param, out It.Ref<float>.IsAny), Times.Never),
            () => MockWrapper.Verify(w => w.GetParameter(param, out It.Ref<string>.IsAny), Times.Never)
        );
    }
}