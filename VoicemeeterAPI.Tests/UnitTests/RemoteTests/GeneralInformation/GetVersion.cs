namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.GeneralInformation;

public class GetVersion : MockRemote
{
    [Fact]
    public void ThrowsInvalidOperationExceptionWhenNotLoggedIn()
    {
        this.MockWrapper.Setup(w => w.GetVoicemeeterVersion()).Returns((Response.Error, default));

        Assert.Multiple(
            () => Assert.Throws<InvalidOperationException>(() => this.Remote.GetVersion()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(), Times.Once())
        );
    }

    [Theory]
    [InlineData(Response.Dirty)]
    [InlineData(Response.UnknownParameter)]
    [InlineData(Response.StructureMismatch)]
    public void ThrowsUnhandledResponseExceptionWhenUnhandledResponse(Response response)
    {
        this.MockWrapper.Setup(w => w.GetVoicemeeterVersion()).Returns((response, default));

        var ex = Assert.Throws<UnhandledResponseException>(() => this.Remote.GetVersion());

        Assert.Multiple(
            () => Assert.Equal(response, ex.Response),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(), Times.Once())
        );
    }
}
