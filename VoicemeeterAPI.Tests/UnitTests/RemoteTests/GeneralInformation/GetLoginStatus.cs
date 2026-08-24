namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.GeneralInformation;

public class GetLoginStatus : MockRemote
{
    [Theory]
    [InlineData(Response.Ok, LoginResponse.Ok)]
    [InlineData(Response.Error, LoginResponse.LoggedOut)]
    [InlineData(Response.NoServer, LoginResponse.VoicemeeterNotRunning)]
    public void ReturnsExpectedLoginResponse(Response response, LoginResponse expected)
    {
        this.MockWrapper.Setup(w => w.GetVoicemeeterKind()).Returns((response, It.IsAny<Kind>()));

        var result = this.Remote.GetLoginStatus();

        Assert.Multiple(
            () => Assert.Equal(expected, result),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterKind(), Times.Once())
        );
    }

    [Theory]
    [InlineData(Response.StructureMismatch)]
    [InlineData(Response.UnknownParameter)]
    [InlineData(Response.Dirty)]
    public void ThrowsUnhandledResponseExceptionWhenUnhandledResponse(Response response)
    {
        this.MockWrapper.Setup(w => w.GetVoicemeeterKind()).Returns((response, It.IsAny<Kind>()));

        var ex = Assert.Throws<UnhandledResponseException>(() => this.Remote.GetLoginStatus());

        Assert.Multiple(
            () => Assert.Equal(response, ex.Response),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterKind(), Times.Once())
        );
    }
}
