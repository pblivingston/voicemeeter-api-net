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
        var app = (int)App.AUXControlPanel;

        this.MockWrapper.Setup(w => w.RunVoicemeeter(app)).Returns(RunResponse.Ok);

        this.MockLoginOk(kind, version);

        this.Remote.Run(app);

        this.MockWrapper.Verify(w => w.RunVoicemeeter(app), Times.Once());
    }

    [Theory]
    [InlineData(App.Standard, true, App.Standardx64)]
    [InlineData(App.Standardx64, false, App.Standard)]
    public void LaunchesAdjustedAppWhenAppIsVoicemeeter(App requested, bool is64Bit, App launched)
    {
        var k = (int)Kind.Standard;
        var version = 0x0101_0202;
        var expectedState = new ConnectionStateEventArgs(LoginResponse.Ok, true, (Kind)k, (VmVersion)version);

        this.MockLoginVoicemeeterNotRunning();

        this.MockWrapper.Setup(w => w.Is64Bit).Returns(is64Bit);
        this.MockWrapper.Setup(w => w.RunVoicemeeter((int)launched)).Returns(RunResponse.Ok);
        this.MockWrapper.Setup(w => w.GetVoicemeeterType(out k)).Returns(InfoResponse.Ok);
        this.MockWrapper.Setup(w => w.GetVoicemeeterVersion(out version)).Returns(InfoResponse.Ok);

        this.MockWrapper.SetupSequence(w => w.IsParametersDirty())
            .Returns(Response.Dirty)
            .Returns(Response.Ok);
        this.MockWrapper.SetupSequence(w => w.MacroButtonIsDirty())
            .Returns(Response.Dirty)
            .Returns(Response.Ok);

        this.Remote.Run(requested);

        Assert.Multiple(
            () => Assert.Equal(expectedState, this.Remote.GetConnectionState()),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter((int)launched), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterType(out k), Times.Exactly(3)),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(out version), Times.Exactly(3)),
            () => this.MockWrapper.Verify(w => w.IsParametersDirty(), Times.Exactly(3)),
            () => this.MockWrapper.Verify(w => w.MacroButtonIsDirty(), Times.Exactly(2))
        );
    }

    [Fact]
    public void UpdatesConnectionStateWhenAppIsMacroButtons()
    {
        var kind = Kind.Standard;
        var version = 0x0101_0202;
        var expectedState = new ConnectionStateEventArgs(LoginResponse.Ok, true, kind, (VmVersion)version);
        var app = App.MacroButtons;

        this.MockWrapper.Setup(w => w.RunVoicemeeter((int)app)).Returns(RunResponse.Ok);

        this.MockLoginMacroButtonsNotRunning((int)kind, version);

        this.MockWrapper.Setup(w => w.MacroButtonIsRunning()).Returns(RunResponse.Ok);

        this.MockWrapper.SetupSequence(w => w.MacroButtonIsDirty())
            .Returns(Response.Dirty)
            .Returns(Response.Ok);

        this.Remote.Run(app);

        Assert.Multiple(
            () => Assert.Equal(expectedState, this.Remote.GetConnectionState()),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter((int)app), Times.Once()),
            () => this.MockWrapper.Verify(w => w.MacroButtonIsDirty(), Times.Exactly(3))
        );
    }

    [Fact]
    public void ThrowsExceptionRunWhenUnknownApp()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var app = (int)App.Unknown;

        this.MockWrapper.Setup(w => w.RunVoicemeeter(app)).Returns(RunResponse.UnknownApp);

        this.MockLoginOk(kind, version);

        var ex = Assert.Throws<RunException>(() => this.Remote.Run(app));

        Assert.Multiple(
            () => Assert.Equal(app, (int)ex.App),
            () => Assert.Equal(RunResponse.UnknownApp, ex.Response),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter(app), Times.Once())
        );
    }

    [Fact]
    public void ThrowsExceptionRunWhenUnexpectedResponse()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var app = (int)App.StreamerView;

        this.MockWrapper.Setup(w => w.RunVoicemeeter(app)).Returns(RunResponse.NotInstalled);

        this.MockLoginOk(kind, version);

        var ex = Assert.Throws<RunException>(() => this.Remote.Run(app));

        Assert.Multiple(
            () => Assert.Equal(app, (int)ex.App),
            () => Assert.Equal(RunResponse.NotInstalled, ex.Response),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter(app), Times.Once())
        );
    }

    [Fact]
    public void ThrowsExceptionAccessDeniedWhenLoginStatusLoggedOut()
    {
        var app = (int)App.DeviceCheck;

        var ex = Assert.Throws<AccessDeniedException>(() => this.Remote.Run(app));

        Assert.Multiple(
            () => Assert.Equal(LoginResponse.LoggedOut, ex.LoginStatus),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter(app), Times.Never())
        );
    }

    [Fact]
    public void ThrowsExceptionObjectDisposedWhenRemoteDisposed()
    {
        var app = (int)App.BUSGEQ15;

        this.Remote.Dispose();

        Assert.Multiple(
            () => Assert.Throws<ObjectDisposedException>(() => this.Remote.Run(app)),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter(app), Times.Never())
        );
    }

    [Fact]
    public void KindUpdatesConnectionStateOkWhenAllConditionsMet()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var expectedState = new ConnectionStateEventArgs(LoginResponse.Ok, true, (Kind)kind, (VmVersion)version);
        var app = App.Standardx64;

        this.MockLoginVoicemeeterNotRunning();

        this.MockWrapper.Setup(w => w.Is64Bit).Returns(true);
        this.MockWrapper.Setup(w => w.RunVoicemeeter((int)app)).Returns(RunResponse.Ok);
        this.MockWrapper.Setup(w => w.GetVoicemeeterType(out kind)).Returns(InfoResponse.Ok);
        this.MockWrapper.Setup(w => w.GetVoicemeeterVersion(out version)).Returns(InfoResponse.Ok);

        this.MockWrapper.SetupSequence(w => w.IsParametersDirty())
            .Returns(Response.Dirty)
            .Returns(Response.Ok);
        this.MockWrapper.SetupSequence(w => w.MacroButtonIsDirty())
            .Returns(Response.Dirty)
            .Returns(Response.Ok);

        this.Remote.Run(Kind.Standard);

        Assert.Multiple(
            () => Assert.Equal(expectedState, this.Remote.GetConnectionState()),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter((int)app), Times.Once())
        );
    }

    [Fact]
    public void KindThrowsExceptionRunWhenWaitForRunningTimesOut()
    {
        var kind = (int)Kind.None;
        var version = 0;
        var app = App.Standardx64;

        this.MockWrapper.Setup(w => w.Is64Bit).Returns(true);
        this.MockWrapper.Setup(w => w.RunVoicemeeter((int)app)).Returns(RunResponse.Ok);
        this.MockWrapper.Setup(w => w.GetVoicemeeterType(out kind)).Returns(InfoResponse.NoServer);
        this.MockWrapper.Setup(w => w.GetVoicemeeterVersion(out version)).Returns(InfoResponse.NoServer);

        this.MockLoginVoicemeeterNotRunning();

        var ex = Assert.Throws<RunException>(() => this.Remote.Run(Kind.Standard, timeoutMs: 10));

        Assert.Multiple(
            () => Assert.Equal(app, ex.App),
            () => Assert.Equal(RunResponse.Timeout, ex.Response),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter((int)app), Times.Once())
        );
    }

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

    [Fact]
    public void GenericAppCallsCorrectOverload()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var app = App.BUSMatrix8;

        this.MockWrapper.Setup(w => w.RunVoicemeeter((int)app)).Returns(RunResponse.Ok);

        this.MockLoginOk(kind, version);

        ((IRemote)this.Remote).Run(app);

        this.MockWrapper.Verify(w => w.RunVoicemeeter((int)app), Times.Once());
    }

    [Fact]
    public void GenericIntCallsCorrectOverload()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var app = (int)App.VBAN2MIDI;

        this.MockWrapper.Setup(w => w.RunVoicemeeter(app)).Returns(RunResponse.Ok);

        this.MockLoginOk(kind, version);

        ((IRemote)this.Remote).Run(app);

        this.MockWrapper.Verify(w => w.RunVoicemeeter(app), Times.Once());
    }

    [Fact]
    public void GenericKindCallsCorrectOverload()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var app = App.Standardx64;

        this.MockLoginVoicemeeterNotRunning();

        this.MockWrapper.Setup(w => w.Is64Bit).Returns(true);
        this.MockWrapper.Setup(w => w.RunVoicemeeter((int)app)).Returns(RunResponse.Ok);
        this.MockWrapper.Setup(w => w.GetVoicemeeterType(out kind)).Returns(InfoResponse.Ok);
        this.MockWrapper.Setup(w => w.GetVoicemeeterVersion(out version)).Returns(InfoResponse.Ok);
        this.MockWrapper.Setup(w => w.IsParametersDirty()).Returns(Response.Ok);
        this.MockWrapper.Setup(w => w.MacroButtonIsDirty()).Returns(Response.Ok);

        ((IRemote)this.Remote).Run(Kind.Standard);

        this.MockWrapper.Verify(w => w.RunVoicemeeter((int)app), Times.Once());
    }

    [Fact]
    public void GenericStringCallsCorrectOverload()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var app = App.CABLEControlPanel;

        this.MockWrapper.Setup(w => w.RunVoicemeeter((int)app)).Returns(RunResponse.Ok);

        this.MockLoginOk(kind, version);

        ((IRemote)this.Remote).Run("CABLEControlPanel");

        this.MockWrapper.Verify(w => w.RunVoicemeeter((int)app), Times.Once());
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
}