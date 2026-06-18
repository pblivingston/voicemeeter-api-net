namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.Login;

using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;
using PBLivingston.VoicemeeterAPI.Utilities;

public class RunAsync : MockRemote
{
    [Fact]
    public async Task LaunchesAppWhenAllConditionsMet()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var app = App.VBAN2MIDI;

        this.MockWrapper.Setup(w => w.RunVoicemeeter((int)app)).Returns(RunResponse.Ok);

        this.MockWrapper.SetupSequence(w => w.GetApplicationState(app))
            .Returns(RunResponse.NotRunning)
            .Returns(RunResponse.Ok);

        this.MockWrapper.SetupSequence(w => w.IsApplicationInputIdle(app))
            .Returns(Response.NoServer)
            .Returns(Response.Dirty)
            .Returns(Response.Ok);

        this.MockLogin(kind, version);

        var result = await this.Remote.RunAsync(app, TestContext.Current.CancellationToken);

        Assert.Multiple(
            () => Assert.Equal(RunResponse.Ok, result),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter((int)app), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(app), Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.IsApplicationInputIdle(app), Times.Exactly(3))
        );
    }

    [Theory]
    [InlineData(App.Standard, true, App.Standardx64)]
    [InlineData(App.Standardx64, false, App.Standard)]
    public async Task UpdatesLastConnectionStateOkWhenAppIsVoicemeeter(App requested, bool is64Bit, App launched)
    {
        var loginStatus = LoginResponse.Ok;
        var buttonsState = RunResponse.Ok;
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var expectedState = new ConnectionState(loginStatus, buttonsState, (Kind)kind, (VmVersion)version);

        this.MockLogin();

        this.MockWrapper.Setup(w => w.Is64Bit).Returns(is64Bit);
        this.MockWrapper.Setup(w => w.RunVoicemeeter((int)launched)).Returns(RunResponse.Ok);
        this.MockWrapper.Setup(w => w.GetApplicationState(launched)).Returns(RunResponse.Ok);

        this.MockWrapper.SetupSequence(w => w.GetVoicemeeterType())
            .Returns((InfoResponse.NoServer, 0))
            .Returns((InfoResponse.NoServer, 0))
            .Returns((InfoResponse.Ok, kind))
            .Returns((InfoResponse.Ok, kind));

        this.MockWrapper.SetupSequence(w => w.GetVoicemeeterVersion())
            .Returns((InfoResponse.NoServer, 0))
            .Returns((InfoResponse.Ok, version))
            .Returns((InfoResponse.Ok, version))
            .Returns((InfoResponse.Ok, version));

        this.MockWrapper.SetupSequence(w => w.IsParametersDirty())
            .Returns(Response.Dirty)
            .Returns(Response.Dirty)
            .Returns(Response.Ok);

        this.MockWrapper.SetupSequence(w => w.MacroButtonIsDirty())
            .Returns(Response.Dirty)
            .Returns(Response.Ok)
            .Returns(Response.Ok);

        var result = await this.Remote.RunAsync(requested, TestContext.Current.CancellationToken);

        Assert.Multiple(
            () => Assert.Equal(RunResponse.Ok, result),
            () => Assert.Equal(expectedState, this.Remote.LastConnectionState),
            () => this.MockWrapper.Verify(w => w.Is64Bit, Times.Exactly(3)),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter((int)launched), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(launched), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterType(), Times.Exactly(5)),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(), Times.Exactly(5)),
            () => this.MockWrapper.Verify(w => w.IsParametersDirty(), Times.Exactly(3)),
            () => this.MockWrapper.Verify(w => w.MacroButtonIsDirty(), Times.Exactly(3)),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(App.MacroButtons), Times.Exactly(2))
        );
    }

    [Fact]
    public async Task UpdatesLastButtonsStateWhenAppIsMacroButtons()
    {
        var app = App.MacroButtons;
        var loginStatus = LoginResponse.Ok;
        var buttonsState = RunResponse.Ok;
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var expectedState = new ConnectionState(loginStatus, buttonsState, (Kind)kind, (VmVersion)version);

        this.MockLogin(RunResponse.NotRunning, kind, version);

        this.MockWrapper.Setup(w => w.RunVoicemeeter((int)app)).Returns(RunResponse.Ok);

        this.MockWrapper.SetupSequence(w => w.GetApplicationState(app))
            .Returns(RunResponse.NotRunning)
            .Returns(RunResponse.Ok)
            .Returns(RunResponse.Ok);

        this.MockWrapper.SetupSequence(w => w.IsApplicationInputIdle(app))
            .Returns(Response.NoServer)
            .Returns(Response.Dirty)
            .Returns(Response.Ok);

        this.MockWrapper.SetupSequence(w => w.MacroButtonIsDirty())
            .Returns(Response.Dirty)
            .Returns(Response.Ok);

        var result = await this.Remote.RunAsync(app, TestContext.Current.CancellationToken);

        Assert.Multiple(
            () => Assert.Equal(RunResponse.Ok, result),
            () => Assert.Equal(expectedState, this.Remote.LastConnectionState),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter((int)app), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(app), Times.Exactly(4)),
            () => this.MockWrapper.Verify(w => w.IsApplicationInputIdle(app), Times.Exactly(3)),
            () => this.MockWrapper.Verify(w => w.MacroButtonIsDirty(), Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterType(), Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(), Times.Exactly(2))
        );
    }

    [Fact]
    public async Task ThrowsExceptionRunWhenWaitForRunningTimesOut()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var app = App.MacroButtons;

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        this.MockWrapper.Setup(w => w.GetApplicationState(app)).Returns(RunResponse.NotRunning);
        this.MockWrapper.Setup(w => w.RunVoicemeeter((int)app)).Returns(RunResponse.Ok);

        this.MockLogin(RunResponse.NotRunning, kind, version);

        var ex = await Assert.ThrowsAsync<RunException>(async () => await this.Remote.RunAsync(app, cts.Token));

        Assert.Multiple(
            () => Assert.Equal(app, ex.App),
            () => Assert.Equal(RunResponse.Timeout, ex.Response),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(app), Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter((int)app), Times.Once()),
            () => this.MockWrapper.Verify(w => w.IsApplicationInputIdle(app), Times.Never()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterType(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.MacroButtonIsDirty(), Times.Never())
        );
    }

    [Fact]
    public async Task ThrowsExceptionRunWhenWaitForVoicemeeterTimesOut()
    {
        var app = App.Standardx64;

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        this.MockWrapper.Setup(w => w.Is64Bit).Returns(true);
        this.MockWrapper.Setup(w => w.RunVoicemeeter((int)app)).Returns(RunResponse.Ok);

        this.MockLogin();

        var ex = await Assert.ThrowsAsync<RunException>(async () => await this.Remote.RunAsync(app, cts.Token));

        Assert.Multiple(
            () => Assert.Equal(app, ex.App),
            () => Assert.Equal(RunResponse.Timeout, ex.Response),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(app), Times.Never()),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter((int)app), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterType(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.IsParametersDirty(), Times.Never()),
            () => this.MockWrapper.Verify(w => w.MacroButtonIsDirty(), Times.Never()),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(App.MacroButtons), Times.Once())
        );
    }

    [Fact]
    public async Task ThrowsExceptionRunWhenIsAppInputIdleError()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var app = App.VAIO3ControlPanel;

        this.MockWrapper.Setup(w => w.GetApplicationState(app)).Returns(RunResponse.NotRunning);
        this.MockWrapper.Setup(w => w.RunVoicemeeter((int)app)).Returns(RunResponse.Ok);
        this.MockWrapper.Setup(w => w.IsApplicationInputIdle(app)).Returns(Response.Error);

        this.MockLogin(kind, version);

        var ex = await Assert.ThrowsAsync<RunException>(async () => await this.Remote.RunAsync(app, TestContext.Current.CancellationToken));

        Assert.Multiple(
            () => Assert.Equal(app, ex.App),
            () => Assert.Equal(RunResponse.Error, ex.Response),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(app), Times.Once()),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter((int)app), Times.Once()),
            () => this.MockWrapper.Verify(w => w.IsApplicationInputIdle(app), Times.Once())
        );
    }

    [Fact]
    public async Task ThrowsExceptionRunWhenVoicemeeterNotRunning()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var app = App.Standardx64;

        this.MockLogin();

        this.MockWrapper.Setup(w => w.Is64Bit).Returns(true);
        this.MockWrapper.Setup(w => w.RunVoicemeeter((int)app)).Returns(RunResponse.Ok);
        this.MockWrapper.Setup(w => w.GetApplicationState(app)).Returns(RunResponse.Ok);

        this.MockWrapper.SetupSequence(w => w.GetVoicemeeterType())
            .Returns((InfoResponse.NoServer, 0))
            .Returns((InfoResponse.NoServer, 0))
            .Returns((InfoResponse.Ok, kind))
            .Returns((InfoResponse.NoServer, 0));

        this.MockWrapper.SetupSequence(w => w.GetVoicemeeterVersion())
            .Returns((InfoResponse.NoServer, 0))
            .Returns((InfoResponse.Ok, version))
            .Returns((InfoResponse.Ok, version))
            .Returns((InfoResponse.NoServer, 0));

        this.MockWrapper.SetupSequence(w => w.IsParametersDirty())
            .Returns(Response.Dirty)
            .Returns(Response.Dirty)
            .Returns(Response.Ok);

        this.MockWrapper.SetupSequence(w => w.MacroButtonIsDirty())
            .Returns(Response.Dirty)
            .Returns(Response.Ok)
            .Returns(Response.Ok);

        var ex = await Assert.ThrowsAsync<RunException>(async () => await this.Remote.RunAsync(app, TestContext.Current.CancellationToken));

        Assert.Multiple(
            () => Assert.Equal(app, ex.App),
            () => Assert.Equal(RunResponse.Error, ex.Response),
            () => this.MockWrapper.Verify(w => w.Is64Bit, Times.Exactly(3)),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter((int)app), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(app), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterType(), Times.Exactly(5)),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(), Times.Exactly(5)),
            () => this.MockWrapper.Verify(w => w.IsParametersDirty(), Times.Exactly(3)),
            () => this.MockWrapper.Verify(w => w.MacroButtonIsDirty(), Times.Exactly(3)),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(App.MacroButtons), Times.Exactly(2))
        );
    }

    [Fact]
    public async Task ThrowsExceptionRunWhenMacroButtonsNotRunning()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var app = App.MacroButtons;

        this.MockLogin(RunResponse.NotRunning, kind, version);

        this.MockWrapper.Setup(w => w.RunVoicemeeter((int)app)).Returns(RunResponse.Ok);

        this.MockWrapper.SetupSequence(w => w.GetApplicationState(app))
            .Returns(RunResponse.NotRunning)
            .Returns(RunResponse.Ok)
            .Returns(RunResponse.NotRunning);

        this.MockWrapper.SetupSequence(w => w.IsApplicationInputIdle(app))
            .Returns(Response.NoServer)
            .Returns(Response.Dirty)
            .Returns(Response.Ok);

        this.MockWrapper.SetupSequence(w => w.MacroButtonIsDirty())
            .Returns(Response.Dirty)
            .Returns(Response.Ok);

        var ex = await Assert.ThrowsAsync<RunException>(async () => await this.Remote.RunAsync(app, TestContext.Current.CancellationToken));

        Assert.Multiple(
            () => Assert.Equal(app, ex.App),
            () => Assert.Equal(RunResponse.Error, ex.Response),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter((int)app), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(app), Times.Exactly(4)),
            () => this.MockWrapper.Verify(w => w.IsApplicationInputIdle(app), Times.Exactly(3)),
            () => this.MockWrapper.Verify(w => w.MacroButtonIsDirty(), Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterType(), Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(), Times.Exactly(2))
        );
    }

    [Fact]
    public async Task ThrowsExceptionRunWhenUnknownApp()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var app = App.Unknown;

        this.MockLogin(kind, version);

        var ex = await Assert.ThrowsAsync<RunException>(async () => await this.Remote.RunAsync(app, TestContext.Current.CancellationToken));

        Assert.Multiple(
            () => Assert.Equal(app, ex.App),
            () => Assert.Equal(RunResponse.UnknownApp, ex.Response),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(app), Times.Never()),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter((int)app), Times.Never()),
            () => this.MockWrapper.Verify(w => w.IsApplicationInputIdle(app), Times.Never())
        );
    }

    [Fact]
    public async Task ThrowsExceptionRunWhenAppStateNotResponding()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var app = App.VAIOControlPanel;

        this.MockWrapper.Setup(w => w.GetApplicationState(app)).Returns(RunResponse.NotResponding);

        this.MockLogin(kind, version);

        var ex = await Assert.ThrowsAsync<RunException>(async () => await this.Remote.RunAsync(app, TestContext.Current.CancellationToken));

        Assert.Multiple(
            () => Assert.Equal(app, ex.App),
            () => Assert.Equal(RunResponse.NotResponding, ex.Response),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(app), Times.Once()),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter((int)app), Times.Never()),
            () => this.MockWrapper.Verify(w => w.IsApplicationInputIdle(app), Times.Never())
        );
    }

    [Fact]
    public async Task ThrowsExceptionRunWhenUnexpectedResponse()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var app = App.StreamerView;

        this.MockWrapper.Setup(w => w.GetApplicationState(app)).Returns(RunResponse.NotRunning);
        this.MockWrapper.Setup(w => w.RunVoicemeeter((int)app)).Returns(RunResponse.NotInstalled);

        this.MockLogin(kind, version);

        var ex = await Assert.ThrowsAsync<RunException>(async () => await this.Remote.RunAsync(app, TestContext.Current.CancellationToken));

        Assert.Multiple(
            () => Assert.Equal(app, ex.App),
            () => Assert.Equal(RunResponse.NotInstalled, ex.Response),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(app), Times.Once()),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter((int)app), Times.Once()),
            () => this.MockWrapper.Verify(w => w.IsApplicationInputIdle(app), Times.Never())
        );
    }

    [Fact]
    public async Task ThrowsExceptionObjectDisposedWhenRemoteDisposed()
    {
        var app = App.BUSGEQ15;

        this.Remote.Dispose();

        await Assert.ThrowsAsync<ObjectDisposedException>(async () => await this.Remote.RunAsync(app, TestContext.Current.CancellationToken));

        Assert.Multiple(
            () => this.MockWrapper.Verify(w => w.GetApplicationState(app), Times.Never()),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter((int)app), Times.Never()),
            () => this.MockWrapper.Verify(w => w.IsApplicationInputIdle(app), Times.Never())
        );
    }

    #region String

    [Fact]
    public async Task StringThrowsExceptionCannotParseAsTypeWhenInvalidString()
    {
        var ex = await Assert.ThrowsAsync<CannotParseAsTypeException>(async () => await this.Remote.RunAsync("InvalidApp", TestContext.Current.CancellationToken));

        Assert.Multiple(
            () => Assert.Equal("InvalidApp", ex.ActualValue),
            () => Assert.Equal(typeof(App), ex.Type),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(It.IsAny<App>()), Times.Never()),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter(It.IsAny<int>()), Times.Never()),
            () => this.MockWrapper.Verify(w => w.IsApplicationInputIdle(It.IsAny<App>()), Times.Never())
        );
    }

    #endregion

    #region Generic

    [Fact]
    public async Task GenericAppCallsCorrectOverload()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var app = App.BUSMatrix8;

        this.MockWrapper.Setup(w => w.RunVoicemeeter((int)app)).Returns(RunResponse.Ok);

        this.MockWrapper.SetupSequence(w => w.GetApplicationState(app))
            .Returns(RunResponse.NotRunning)
            .Returns(RunResponse.Ok);

        this.MockWrapper.SetupSequence(w => w.IsApplicationInputIdle(app))
            .Returns(Response.NoServer)
            .Returns(Response.Dirty)
            .Returns(Response.Ok);

        this.MockLogin(kind, version);

        var result = await ((IRemote)this.Remote).RunAsync(app, TestContext.Current.CancellationToken);

        Assert.Multiple(
            () => Assert.Equal(RunResponse.Ok, result),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter((int)app), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(app), Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.IsApplicationInputIdle(app), Times.Exactly(3))
        );
    }

    [Fact]
    public async Task GenericIntCallsCorrectOverload()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var app = App.VBAN2MIDI;

        this.MockWrapper.Setup(w => w.RunVoicemeeter((int)app)).Returns(RunResponse.Ok);

        this.MockWrapper.SetupSequence(w => w.GetApplicationState(app))
            .Returns(RunResponse.NotRunning)
            .Returns(RunResponse.Ok);

        this.MockWrapper.SetupSequence(w => w.IsApplicationInputIdle(app))
            .Returns(Response.NoServer)
            .Returns(Response.Dirty)
            .Returns(Response.Ok);

        this.MockLogin(kind, version);

        var result = await ((IRemote)this.Remote).RunAsync((int)app, TestContext.Current.CancellationToken);

        Assert.Multiple(
            () => Assert.Equal(RunResponse.Ok, result),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter((int)app), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(app), Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.IsApplicationInputIdle(app), Times.Exactly(3))
        );
    }

    [Fact]
    public async Task GenericKindCallsCorrectOverload()
    {
        var kind = Kind.Standard;
        var app = App.Standardx64;
        var loginStatus = LoginResponse.Ok;
        var buttonsState = RunResponse.Ok;
        var version = 0x0101_0202;
        var expectedState = new ConnectionState(loginStatus, buttonsState, kind, (VmVersion)version);

        this.MockLogin();

        this.MockWrapper.Setup(w => w.Is64Bit).Returns(true);
        this.MockWrapper.Setup(w => w.RunVoicemeeter((int)app)).Returns(RunResponse.Ok);
        this.MockWrapper.Setup(w => w.GetApplicationState(app)).Returns(RunResponse.Ok);

        this.MockWrapper.SetupSequence(w => w.GetVoicemeeterType())
            .Returns((InfoResponse.NoServer, 0))
            .Returns((InfoResponse.NoServer, 0))
            .Returns((InfoResponse.Ok, (int)kind))
            .Returns((InfoResponse.Ok, (int)kind));

        this.MockWrapper.SetupSequence(w => w.GetVoicemeeterVersion())
            .Returns((InfoResponse.NoServer, 0))
            .Returns((InfoResponse.Ok, version))
            .Returns((InfoResponse.Ok, version))
            .Returns((InfoResponse.Ok, version));

        this.MockWrapper.SetupSequence(w => w.IsParametersDirty())
            .Returns(Response.Dirty)
            .Returns(Response.Dirty)
            .Returns(Response.Ok);

        this.MockWrapper.SetupSequence(w => w.MacroButtonIsDirty())
            .Returns(Response.Dirty)
            .Returns(Response.Ok)
            .Returns(Response.Ok);

        var result = await ((IRemote)this.Remote).RunAsync(kind, TestContext.Current.CancellationToken);

        Assert.Multiple(
            () => Assert.Equal(RunResponse.Ok, result),
            () => Assert.Equal(expectedState, this.Remote.LastConnectionState),
            () => this.MockWrapper.Verify(w => w.Is64Bit, Times.Exactly(4)),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter((int)app), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(app), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterType(), Times.Exactly(5)),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(), Times.Exactly(5)),
            () => this.MockWrapper.Verify(w => w.IsParametersDirty(), Times.Exactly(3)),
            () => this.MockWrapper.Verify(w => w.MacroButtonIsDirty(), Times.Exactly(3)),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(App.MacroButtons), Times.Exactly(2))
        );
    }

    [Fact]
    public async Task GenericStringCallsCorrectOverload()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var app = App.CABLEControlPanel;

        this.MockWrapper.Setup(w => w.RunVoicemeeter((int)app)).Returns(RunResponse.Ok);

        this.MockWrapper.SetupSequence(w => w.GetApplicationState(app))
            .Returns(RunResponse.NotRunning)
            .Returns(RunResponse.Ok);

        this.MockWrapper.SetupSequence(w => w.IsApplicationInputIdle(app))
            .Returns(Response.NoServer)
            .Returns(Response.Dirty)
            .Returns(Response.Ok);

        this.MockLogin(kind, version);

        var result = await ((IRemote)this.Remote).RunAsync(app.ToString(), TestContext.Current.CancellationToken);

        Assert.Multiple(
            () => Assert.Equal(RunResponse.Ok, result),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter((int)app), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(app), Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.IsApplicationInputIdle(app), Times.Exactly(3))
        );
    }

    [Fact]
    public async Task GenericThrowsExceptionTypeNotSupportedWhenInvalidType()
    {
        var ex = await Assert.ThrowsAsync<TypeNotSupportedException>(async () => await ((IRemote)this.Remote).RunAsync(10.0f, TestContext.Current.CancellationToken));

        Assert.Multiple(
            () => Assert.Equal(typeof(float), ex.Type),
            () => Assert.Equal(SupportedTypes.RunTypes, ex.SupportedTypes),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter(It.IsAny<int>()), Times.Never())
        );
    }

    #endregion
}
