namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.GetParameters;

public class GetParamFloat : MockRemote
{
    [Fact]
    public void ReturnsFailureWhenVoicemeeterHasShutDownExternally()
    {
        var vmState = RunResponse.Hidden;
        var vmApp = App.Potato;
        var vmVersion = VmVersion.MaxValid;
        var buttonsState = RunResponse.Hidden;
        var response = Response.NoServer;
        var param = "Mock.Param";
        var expected = Result.Failure<Response, float>(response);

        this.MockLogin(vmState, vmApp, vmVersion, buttonsState);

        this.MockWrapper.Setup(w => w.GetParameter_Float(param)).Returns((response, default));

        var result = this.Remote.GetParamFloat(param);

        Assert.Multiple(
            () => Assert.Equal(expected, result),
            () => this.MockWrapper.Verify(w => w.GetParameter_Float(param), Times.Once())
        );
    }

    [Fact]
    public void ThrowsInvalidOperationExceptionWhenNotLoggedIn()
    {
        var param = "Mock.Param";

        this.MockWrapper.Setup(w => w.GetParameter_Float(param)).Returns((Response.Error, default));

        Assert.Multiple(
            () => Assert.Throws<InvalidOperationException>(() => this.Remote.GetParamFloat(param)),
            () => this.MockWrapper.Verify(w => w.GetParameter_Float(param), Times.Once())
        );
    }

    [Fact]
    public void ThrowsRemoteExceptionWhenAmbiguousError()
    {
        var vmState = RunResponse.Ok;
        var vmApp = App.Standardx64;
        var vmVersion = VmVersion.MinValid;
        var buttonsState = RunResponse.Hidden;
        var response = Response.Error;
        var param = "Mock.Param";

        this.MockLogin(vmState, vmApp, vmVersion, buttonsState);

        this.MockWrapper.Setup(w => w.GetParameter_Float(param)).Returns((response, default));

        var ex = Assert.Throws<RemoteException>(() => this.Remote.GetParamFloat(param));

        Assert.Multiple(
            () => Assert.Equal(response, ex.Response),
            () => this.MockWrapper.Verify(w => w.GetParameter_Float(param), Times.Once())
        );
    }

    [Fact]
    public void ThrowsInvalidOperationExceptionWhenVoicemeeterNotRunning()
    {
        var buttonsState = RunResponse.Ok;
        var param = "Mock.Param";

        this.MockLogin(buttonsState);

        this.MockWrapper.Setup(w => w.GetParameter_Float(param)).Returns((Response.NoServer, default));

        Assert.Multiple(
            () => Assert.Throws<InvalidOperationException>(() => this.Remote.GetParamFloat(param)),
            () => this.MockWrapper.Verify(w => w.GetParameter_Float(param), Times.Once())
        );
    }

    [Fact]
    public void ThrowsArgumentExceptionWhenUnknownParameter()
    {
        var param = "Mock.Param";

        this.MockWrapper.Setup(w => w.GetParameter_Float(param)).Returns((Response.UnknownParameter, default));

        Assert.Multiple(
            () => Assert.Throws<ArgumentException>(() => this.Remote.GetParamFloat(param)),
            () => this.MockWrapper.Verify(w => w.GetParameter_Float(param), Times.Once())
        );
    }

    [Theory]
    [InlineData(Response.StructureMismatch)]
    [InlineData(Response.Dirty)]
    public void ThrowsUnhandledResponseExceptionWhenUnhandledResponse(Response response)
    {
        var param = "Mock.Param";

        this.MockWrapper.Setup(w => w.GetParameter_Float(param)).Returns((response, default));

        var ex = Assert.Throws<UnhandledResponseException>(() => this.Remote.GetParamFloat(param));

        Assert.Multiple(
            () => Assert.Equal(response, ex.Response),
            () => this.MockWrapper.Verify(w => w.GetParameter_Float(param), Times.Once())
        );
    }
}
