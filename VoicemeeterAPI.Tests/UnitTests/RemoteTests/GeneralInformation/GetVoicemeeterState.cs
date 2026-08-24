namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.GeneralInformation;

public class GetVoicemeeterState : MockRemote
{
    [Theory]
    [InlineData(RunResponse.AlreadyShutDown)]
    [InlineData(RunResponse.Error)]
    [InlineData(RunResponse.UnknownApp)]
    [InlineData(RunResponse.NotInstalled)]
    public void ThrowsUnhandledResponseExceptionWhenUnhandledResponse(RunResponse response)
    {
        this.MockWrapper.Setup(w => w.GetVoicemeeterState()).Returns((It.IsAny<App>(), response));

        var ex = Assert.Throws<UnhandledResponseException>(() => this.Remote.GetVoicemeeterState());

        Assert.Multiple(
            () => Assert.Equal(response, ex.Response),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterState(), Times.Once())
        );
    }
}
