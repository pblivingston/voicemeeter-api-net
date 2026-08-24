namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.GeneralInformation;

public class GetKind : MockRemote
{
    [Fact]
    public void ThrowsInvalidOperationExceptionWhenNotLoggedIn()
    {
        this.MockWrapper.Setup(w => w.GetVoicemeeterKind()).Returns((Response.Error, Kind.None));

        Assert.Multiple(
            () => Assert.Throws<InvalidOperationException>(() => this.Remote.GetKind()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterKind(), Times.Once())
        );
    }

    [Theory]
    [InlineData(Response.Dirty)]
    [InlineData(Response.UnknownParameter)]
    [InlineData(Response.StructureMismatch)]
    public void ThrowsUnhandledResponseExceptionWhenUnhandledResponse(Response response)
    {
        this.MockWrapper.Setup(w => w.GetVoicemeeterKind()).Returns((response, Kind.None));

        var ex = Assert.Throws<UnhandledResponseException>(() => this.Remote.GetKind());

        Assert.Multiple(
            () => Assert.Equal(response, ex.Response),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterKind(), Times.Once())
        );
    }
}
