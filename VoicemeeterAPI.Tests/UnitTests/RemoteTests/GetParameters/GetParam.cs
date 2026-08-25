namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.GetParameters;

public class GetParam : MockRemote
{
    [Fact]
    public void IntThrowsArgumentExceptionWhenCannotConvertFromFloat()
    {
        var param = "Mock.Param";
        var value = 0.75f;

        this.MockWrapper.Setup(w => w.GetParameter_Float(param)).Returns((Response.Ok, value));

        Assert.Multiple(
            () => Assert.Throws<ArgumentException>(() => this.Remote.GetParamInt(param)),
            () => this.MockWrapper.Verify(w => w.GetParameter_Float(param), Times.Once())
        );
    }

    [Fact]
    public void BoolThrowsArgumentExceptionWhenCannotConvertFromFloat()
    {
        var param = "Mock.Param";
        var value = 2.0f;

        this.MockWrapper.Setup(w => w.GetParameter_Float(param)).Returns((Response.Ok, value));

        Assert.Multiple(
            () => Assert.Throws<ArgumentException>(() => this.Remote.GetParamBool(param)),
            () => this.MockWrapper.Verify(w => w.GetParameter_Float(param), Times.Once())
        );
    }

    [Fact]
    public void GenericThrowsArgumentExceptionWhenTypeNotSupported()
    {
        var param = "Mock.Param";

        Assert.Multiple(
            () => Assert.Throws<ArgumentException>(() => ((IRemote)this.Remote).GetParam<DateTime>(param)),
            () => this.MockWrapper.Verify(w => w.GetParameter_Float(param), Times.Never()),
            () => this.MockWrapper.Verify(w => w.GetParameter_String(param), Times.Never())
        );
    }
}
