namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.Login;

using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;
using PBLivingston.VoicemeeterAPI.Utilities;

public class Run : MockRemote
{
    [Fact]
    public void LaunchesAppWhenValid()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var app = App.MacroButtons;

        this.MockWrapper.Setup(w => w.GetApplicationState(app)).Returns(RunResponse.NotRunning);
        this.MockWrapper.Setup(w => w.RunVoicemeeter((int)app)).Returns(RunResponse.Ok);

        this.MockLogin(RunResponse.NotRunning, kind, version);

        this.Remote.Run(app);

        Assert.Multiple(
            () => this.MockWrapper.Verify(w => w.GetApplicationState(app), Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter((int)app), Times.Once())
        );
    }

    [Theory]
    [InlineData(App.Standard, true, App.Standardx64)]
    [InlineData(App.Standardx64, false, App.Standard)]
    public void LaunchesAdjustedAppWhenAppIsVoicemeeter(App requested, bool is64Bit, App launched)
    {
        this.MockLogin();

        this.MockWrapper.Setup(w => w.Is64Bit).Returns(is64Bit);
        this.MockWrapper.Setup(w => w.RunVoicemeeter((int)launched)).Returns(RunResponse.Ok);

        this.Remote.Run(requested);

        Assert.Multiple(
            () => this.MockWrapper.Verify(w => w.Is64Bit, Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(launched), Times.Never()),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter((int)launched), Times.Once())
        );
    }

    [Fact]
    public void ThrowsExceptionRunWhenUnknownApp()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var app = App.Unknown;

        this.MockLogin(kind, version);

        var ex = Assert.Throws<RunException>(() => this.Remote.Run(app));

        Assert.Multiple(
            () => Assert.Equal(app, ex.App),
            () => Assert.Equal(RunResponse.UnknownApp, ex.Response),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter(It.IsAny<int>()), Times.Never())
        );
    }

    [Fact]
    public void ThrowsExceptionRunWhenAppStateNotResponding()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var app = App.VAIOControlPanel;

        this.MockWrapper.Setup(w => w.GetApplicationState(app)).Returns(RunResponse.NotResponding);

        this.MockLogin(kind, version);

        var ex = Assert.Throws<RunException>(() => this.Remote.Run(app));

        Assert.Multiple(
            () => Assert.Equal(app, ex.App),
            () => Assert.Equal(RunResponse.NotResponding, ex.Response),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter(It.IsAny<int>()), Times.Never())
        );
    }

    [Fact]
    public void ThrowsExceptionRunWhenUnexpectedResponse()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var app = App.StreamerView;

        this.MockWrapper.Setup(w => w.GetApplicationState(app)).Returns(RunResponse.NotRunning);
        this.MockWrapper.Setup(w => w.RunVoicemeeter((int)app)).Returns(RunResponse.NotInstalled);

        this.MockLogin(kind, version);

        var ex = Assert.Throws<RunException>(() => this.Remote.Run(app));

        Assert.Multiple(
            () => Assert.Equal(app, ex.App),
            () => Assert.Equal(RunResponse.NotInstalled, ex.Response),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter((int)app), Times.Once())
        );
    }

    [Fact]
    public void ThrowsExceptionObjectDisposedWhenRemoteDisposed()
    {
        var app = App.BUSGEQ15;

        this.Remote.Dispose();

        Assert.Multiple(
            () => Assert.Throws<ObjectDisposedException>(() => this.Remote.Run(app)),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter(It.IsAny<int>()), Times.Never())
        );
    }

    #region String

    [Fact]
    public void StringThrowsExceptionCannotParseAsTypeWhenInvalidString()
    {
        var ex = Assert.Throws<CannotParseAsTypeException>(() => this.Remote.Run("InvalidApp"));

        Assert.Multiple(
            () => Assert.Equal("InvalidApp", ex.ActualValue),
            () => Assert.Equal(typeof(App), ex.Type),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter(It.IsAny<int>()), Times.Never())
        );
    }

    #endregion

    #region Generic

    [Fact]
    public void GenericAppCallsCorrectOverload()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var app = App.BUSMatrix8;

        this.MockWrapper.Setup(w => w.GetApplicationState(app)).Returns(RunResponse.NotRunning);
        this.MockWrapper.Setup(w => w.RunVoicemeeter((int)app)).Returns(RunResponse.Ok);

        this.MockLogin(kind, version);

        ((IRemote)this.Remote).Run(app);

        Assert.Multiple(
            () => this.MockWrapper.Verify(w => w.GetApplicationState(app), Times.Once()),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter((int)app), Times.Once())
        );
    }

    [Fact]
    public void GenericIntCallsCorrectOverload()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var app = App.VBAN2MIDI;

        this.MockWrapper.Setup(w => w.GetApplicationState(app)).Returns(RunResponse.NotRunning);
        this.MockWrapper.Setup(w => w.RunVoicemeeter((int)app)).Returns(RunResponse.Ok);

        this.MockLogin(kind, version);

        ((IRemote)this.Remote).Run((int)app);

        Assert.Multiple(
            () => this.MockWrapper.Verify(w => w.GetApplicationState(app), Times.Once()),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter((int)app), Times.Once())
        );
    }

    [Fact]
    public void GenericKindCallsCorrectOverload()
    {
        var kind = Kind.Standard;
        var app = App.Standardx64;

        this.MockWrapper.Setup(w => w.Is64Bit).Returns(true);
        this.MockWrapper.Setup(w => w.RunVoicemeeter((int)app)).Returns(RunResponse.Ok);

        this.MockLogin();

        ((IRemote)this.Remote).Run(kind);

        Assert.Multiple(
            () => this.MockWrapper.Verify(w => w.Is64Bit, Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter((int)app), Times.Once())
        );
    }

    [Fact]
    public void GenericStringCallsCorrectOverload()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var app = App.CABLEControlPanel;

        this.MockWrapper.Setup(w => w.GetApplicationState(app)).Returns(RunResponse.NotRunning);
        this.MockWrapper.Setup(w => w.RunVoicemeeter((int)app)).Returns(RunResponse.Ok);

        this.MockLogin(kind, version);

        ((IRemote)this.Remote).Run(app.ToString());

        Assert.Multiple(
            () => this.MockWrapper.Verify(w => w.GetApplicationState(app), Times.Once()),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter((int)app), Times.Once())
        );
    }

    [Fact]
    public void GenericThrowsExceptionTypeNotSupportedWhenInvalidType()
    {
        var ex = Assert.Throws<TypeNotSupportedException>(() => ((IRemote)this.Remote).Run(10.0f));

        Assert.Multiple(
            () => Assert.Equal(typeof(float), ex.Type),
            () => Assert.Equal(SupportedTypes.RunTypes, ex.SupportedTypes),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter(It.IsAny<int>()), Times.Never())
        );
    }

    #endregion
}
