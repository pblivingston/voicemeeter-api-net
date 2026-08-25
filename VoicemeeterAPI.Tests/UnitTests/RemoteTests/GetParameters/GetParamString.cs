namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.GetParameters;

public class GetParamString : MockRemote
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
        var expected = Result.Failure<Response, string>(response);

        this.MockLogin(vmState, vmApp, vmVersion, buttonsState);

        this.MockWrapper.Setup(w => w.GetParameter_String(param)).Returns((response, string.Empty));

        var result = this.Remote.GetParamString(param);

        Assert.Multiple(
            () => Assert.Equal(expected, result),
            () => this.MockWrapper.Verify(w => w.GetParameter_String(param), Times.Once())
        );
    }

    [Fact]
    public void ThrowsInvalidOperationExceptionWhenNotLoggedIn()
    {
        var param = "Mock.Param";

        this.MockWrapper.Setup(w => w.GetParameter_String(param)).Returns((Response.Error, string.Empty));

        Assert.Multiple(
            () => Assert.Throws<InvalidOperationException>(() => this.Remote.GetParamString(param)),
            () => this.MockWrapper.Verify(w => w.GetParameter_String(param), Times.Once())
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

        this.MockWrapper.Setup(w => w.GetParameter_String(param)).Returns((response, string.Empty));

        var ex = Assert.Throws<RemoteException>(() => this.Remote.GetParamString(param));

        Assert.Multiple(
            () => Assert.Equal(response, ex.Response),
            () => this.MockWrapper.Verify(w => w.GetParameter_String(param), Times.Once())
        );
    }

    [Fact]
    public void ThrowsInvalidOperationExceptionWhenVoicemeeterNotRunning()
    {
        var buttonsState = RunResponse.Ok;
        var param = "Mock.Param";

        this.MockLogin(buttonsState);

        this.MockWrapper.Setup(w => w.GetParameter_String(param)).Returns((Response.NoServer, string.Empty));

        Assert.Multiple(
            () => Assert.Throws<InvalidOperationException>(() => this.Remote.GetParamString(param)),
            () => this.MockWrapper.Verify(w => w.GetParameter_String(param), Times.Once())
        );
    }

    [Fact]
    public void ThrowsArgumentExceptionWhenUnknownParameter()
    {
        var param = "Mock.Param";

        this.MockWrapper.Setup(w => w.GetParameter_String(param)).Returns((Response.UnknownParameter, string.Empty));

        Assert.Multiple(
            () => Assert.Throws<ArgumentException>(() => this.Remote.GetParamString(param)),
            () => this.MockWrapper.Verify(w => w.GetParameter_String(param), Times.Once())
        );
    }

    [Theory]
    [InlineData(Response.StructureMismatch)]
    [InlineData(Response.Dirty)]
    public void ThrowsUnhandledResponseExceptionWhenUnhandledResponse(Response response)
    {
        var param = "Mock.Param";

        this.MockWrapper.Setup(w => w.GetParameter_String(param)).Returns((response, string.Empty));

        var ex = Assert.Throws<UnhandledResponseException>(() => this.Remote.GetParamString(param));

        Assert.Multiple(
            () => Assert.Equal(response, ex.Response),
            () => this.MockWrapper.Verify(w => w.GetParameter_String(param), Times.Once())
        );
    }
}
