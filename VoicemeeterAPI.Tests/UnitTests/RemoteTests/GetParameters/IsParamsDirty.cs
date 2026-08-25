namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.GetParameters;

public class IsParamsDirty : MockRemote
{
    [Fact]
    public void ReturnsFailureWhenVoicemeeterHasShutDownExternally()
    {
        var vmState = RunResponse.Hidden;
        var vmApp = App.Potato;
        var vmVersion = VmVersion.MaxValid;
        var buttonsState = RunResponse.Hidden;
        var response = Response.NoServer;
        var expected = Result.Failure<Response, bool>(response);

        this.MockLogin(vmState, vmApp, vmVersion, buttonsState);

        this.MockWrapper.Setup(w => w.IsParametersDirty()).Returns(response);

        var result = this.Remote.IsParamsDirty();

        Assert.Multiple(
            () => Assert.Equal(expected, result),
            () => this.MockWrapper.Verify(w => w.IsParametersDirty(), Times.Once())
        );
    }

    [Fact]
    public void ThrowsInvalidOperationExceptionWhenNotLoggedIn()
    {
        this.MockWrapper.Setup(w => w.IsParametersDirty()).Returns(Response.Error);

        Assert.Multiple(
            () => Assert.Throws<InvalidOperationException>(() => this.Remote.IsParamsDirty()),
            () => this.MockWrapper.Verify(w => w.IsParametersDirty(), Times.Once())
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

        this.MockLogin(vmState, vmApp, vmVersion, buttonsState);

        this.MockWrapper.Setup(w => w.IsParametersDirty()).Returns(response);

        var ex = Assert.Throws<RemoteException>(() => this.Remote.IsParamsDirty());

        Assert.Multiple(
            () => Assert.Equal(response, ex.Response),
            () => this.MockWrapper.Verify(w => w.IsParametersDirty(), Times.Once())
        );
    }

    [Fact]
    public void ThrowsInvalidOperationExceptionWhenVoicemeeterNotRunning()
    {
        var buttonsState = RunResponse.Ok;

        this.MockLogin(buttonsState);

        this.MockWrapper.Setup(w => w.IsParametersDirty()).Returns(Response.NoServer);

        Assert.Multiple(
            () => Assert.Throws<InvalidOperationException>(() => this.Remote.IsParamsDirty()),
            () => this.MockWrapper.Verify(w => w.IsParametersDirty(), Times.Once())
        );
    }

    [Theory]
    [InlineData(Response.StructureMismatch)]
    [InlineData(Response.UnknownParameter)]
    public void ThrowsUnhandledResponseExceptionWhenUnhandledResponse(Response response)
    {
        this.MockWrapper.Setup(w => w.IsParametersDirty()).Returns(response);

        var ex = Assert.Throws<UnhandledResponseException>(() => this.Remote.IsParamsDirty());

        Assert.Multiple(
            () => Assert.Equal(response, ex.Response),
            () => this.MockWrapper.Verify(w => w.IsParametersDirty(), Times.Once())
        );
    }
}
