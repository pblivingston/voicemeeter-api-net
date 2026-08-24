namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.Login;

public class Run : MockRemote
{
    [Theory]
    [InlineData(Kind.Standard, true, App.Standardx64)]
    [InlineData(Kind.Potato, false, App.Potato)]
    public void KindOverloadLaunchesCorrectVoicemeeterApp(Kind requested, bool is64Bit, App launched)
    {
        this.MockWrapper.Setup(w => w.Is64Bit).Returns(is64Bit);
        this.MockWrapper.Setup(w => w.RunVoicemeeter(launched)).Returns(RunResponse.Ok);

        this.Remote.Run(requested);

        Assert.Multiple(
            () => this.MockWrapper.Verify(w => w.Is64Bit, Times.Once()),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter(launched), Times.Once())
        );
    }

    [Fact]
    public void ThrowsAppNotInstalledExceptionWhenAppNotInstalled()
    {
        var app = App.VAIOControlPanel;

        this.MockWrapper.Setup(w => w.RunVoicemeeter(app)).Returns(RunResponse.NotInstalled);

        var ex = Assert.Throws<AppNotInstalledException>(() => this.Remote.Run(app));

        Assert.Multiple(
            () => Assert.Equal(app, ex.App),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter(app), Times.Once())
        );
    }

    [Fact]
    public void ThrowsArgumentExceptionWhenUnknownApp()
    {
        var app = App.Unknown;

        this.MockWrapper.Setup(w => w.RunVoicemeeter(app)).Returns(RunResponse.UnknownApp);

        Assert.Multiple(
            () => Assert.Throws<ArgumentException>(() => this.Remote.Run(app)),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter(app), Times.Once())
        );
    }

    [Theory]
    [InlineData(RunResponse.AlreadyShutDown)]
    [InlineData(RunResponse.Error)]
    [InlineData(RunResponse.Hidden)]
    [InlineData(RunResponse.NotRunning)]
    [InlineData(RunResponse.NotResponding)]
    public void ThrowsUnhandledResponseExceptionWhenUnhandledResponse(RunResponse response)
    {
        var app = App.StreamerView;

        this.MockWrapper.Setup(w => w.RunVoicemeeter(app)).Returns(response);

        var ex = Assert.Throws<UnhandledResponseException>(() => this.Remote.Run(app));

        Assert.Multiple(
            () => Assert.Equal(response, ex.Response),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter(app), Times.Once())
        );
    }

    [Fact]
    public void GenericThrowsArgumentExceptionWhenTypeNotSupported()
        => Assert.Multiple(
            () => Assert.Throws<ArgumentException>(() => ((IRemote)this.Remote).Run(RunResponse.Ok)),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter(It.IsAny<App>()), Times.Never())
        );
}
