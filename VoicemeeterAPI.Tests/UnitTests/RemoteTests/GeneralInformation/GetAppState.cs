namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.GeneralInformation;

public class GetAppState : MockRemote
{
    [Fact]
    public void ThrowsAppNotInstalledExceptionWhenExecutableNotFound()
    {
        var app = App.CABLEControlPanel;

        this.MockWrapper.Setup(w => w.GetApplicationState(app)).Returns(RunResponse.NotInstalled);

        Assert.Multiple(
            () => Assert.Throws<AppNotInstalledException>(() => this.Remote.GetAppState(app)),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(app), Times.Once())
        );
    }

    [Fact]
    public void ThrowsArgumentExceptionWhenUnknownApp()
    {
        var app = App.Unknown;

        this.MockWrapper.Setup(w => w.GetApplicationState(app)).Returns(RunResponse.UnknownApp);

        Assert.Multiple(
            () => Assert.Throws<ArgumentException>(() => this.Remote.GetAppState(app)),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(app), Times.Once())
        );
    }

    [Theory]
    [InlineData(RunResponse.AlreadyShutDown)]
    [InlineData(RunResponse.Error)]
    public void ThrowsUnhandledResponseExceptionWhenUnhandledResponse(RunResponse response)
    {
        var app = App.DeviceCheck;

        this.MockWrapper.Setup(w => w.GetApplicationState(app)).Returns(response);

        var ex = Assert.Throws<UnhandledResponseException>(() => this.Remote.GetAppState(app));

        Assert.Multiple(
            () => Assert.Equal(response, ex.Response),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(app), Times.Once())
        );
    }
}
