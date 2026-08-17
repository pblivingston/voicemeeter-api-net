namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.Login;

public class RunAsync : MockRemote
{
    [Theory]
    [InlineData(Kind.Standard, true, App.Standardx64, 0x0102_0304)]
    [InlineData(Kind.Potato, false, App.Potato, 0x0304_0506)]
    public async Task UpdatesLastConnectionStateWhenAppIsVoicemeeter(Kind requested, bool is64Bit, App launched, int vmPacked)
    {
        var loginStatus = LoginResponse.Ok;
        var vmState = RunResponse.Ok;
        var vmVersion = (VmVersion)vmPacked;
        var buttonsState = RunResponse.NotRunning;
        var expectedResult = Result.Success(vmState, launched);
        var expectedState = new ConnectionState(loginStatus, vmState, launched, vmVersion, buttonsState);

        this.MockLogin(buttonsState);

        this.MockWrapper.Setup(w => w.Is64Bit).Returns(is64Bit);
        this.MockWrapper.Setup(w => w.RunVoicemeeter(launched)).Returns(RunResponse.Ok);
        this.MockWrapper.Setup(w => w.GetVoicemeeterState()).Returns((launched, vmState));
        this.MockWrapper.Setup(w => w.GetApplicationState(App.MacroButtons)).Returns(buttonsState);

        this.MockWrapper.SetupSequence(w => w.GetVoicemeeterVersion())
            .Returns((Response.NoServer, default))
            .Returns((Response.NoServer, default))
            .Returns((Response.Ok, vmVersion))
            .Returns((Response.Ok, vmVersion));

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
            () => Assert.Equal(expectedResult, result),
            () => Assert.Equal(expectedState, this.Remote.LastConnectionState),
            () => this.MockWrapper.Verify(w => w.Is64Bit, Times.Once()),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter(launched), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterState(), Times.Exactly(3)),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(), Times.Exactly(5)),
            () => this.MockWrapper.Verify(w => w.IsParametersDirty(), Times.Exactly(3)),
            () => this.MockWrapper.Verify(w => w.MacroButtonIsDirty(), Times.Exactly(3)),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(App.MacroButtons), Times.Exactly(2))
        );
    }

    [Fact]
    public async Task UpdatesLastConnectionStateWhenAppIsMacroButtons()
    {
        var app = App.MacroButtons;
        var loginStatus = LoginResponse.Ok;
        var vmState = RunResponse.Ok;
        var vmApp = App.Standard;
        var vmVersion = VmVersion.MinValid;
        var buttonsState = RunResponse.Ok;
        var expectedResult = Result.Success(buttonsState, app);
        var expectedState = new ConnectionState(loginStatus, vmState, vmApp, vmVersion, buttonsState);

        this.MockLogin(vmState, vmApp, vmVersion);

        this.MockWrapper.Setup(w => w.RunVoicemeeter(app)).Returns(RunResponse.Ok);
        this.MockWrapper.Setup(w => w.GetApplicationState(app)).Returns(RunResponse.Ok);

        this.MockWrapper.SetupSequence(w => w.MacroButtonIsDirty())
            .Returns(Response.Dirty)
            .Returns(Response.Dirty)
            .Returns(Response.Ok);

        var result = await this.Remote.RunAsync(app, TestContext.Current.CancellationToken);

        Assert.Multiple(
            () => Assert.Equal(expectedResult, result),
            () => Assert.Equal(expectedState, this.Remote.LastConnectionState),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter(app), Times.Once()),
            () => this.MockWrapper.Verify(w => w.WaitForApplicationInputIdle(app, TestContext.Current.CancellationToken), Times.Once()),
            () => this.MockWrapper.Verify(w => w.MacroButtonIsDirty(), Times.Exactly(3)),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterState(), Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(), Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(app), Times.Exactly(3))
        );
    }

    [Fact]
    public async Task ReturnsTimeoutWhenWaitForEngineTimesOut()
    {
        var app = App.Banana;
        var response = RunResponse.Timeout;
        var state = RunResponse.NotRunning;
        var expectedResult = Result.Failure(response, app);

        this.MockLogin(state);

        this.MockWrapper.Setup(w => w.RunVoicemeeter(app)).Returns(RunResponse.Ok);
        this.MockWrapper.Setup(w => w.GetVoicemeeterVersion()).Returns((Response.NoServer, default));

        var result = await this.Remote.RunAsync(app, TestContext.Current.CancellationToken);

        Assert.Multiple(
            () => Assert.Equal(expectedResult, result),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter(app), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterState(), Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(), Times.AtLeast(3)),
            () => this.MockWrapper.Verify(w => w.IsParametersDirty(), Times.Never()),
            () => this.MockWrapper.Verify(w => w.MacroButtonIsDirty(), Times.Never())
        );
    }

    [Fact]
    public async Task ReturnsTimeoutWhenWaitForRunningTimesOut()
    {
        var app = App.MacroButtons;
        var response = RunResponse.Timeout;
        var vmState = RunResponse.Ok;
        var vmApp = App.Potato;
        var vmVersion = VmVersion.MaxValid;
        var expectedResult = Result.Failure(response, app);

        this.MockLogin(vmState, vmApp, vmVersion);

        this.MockWrapper.Setup(w => w.RunVoicemeeter(app)).Returns(RunResponse.Ok);

        var result = await this.Remote.RunAsync(app, TestContext.Current.CancellationToken);

        Assert.Multiple(
            () => Assert.Equal(expectedResult, result),
            () => this.MockWrapper.Verify(w => w.RunVoicemeeter(app), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(app), Times.Once()),
            () => this.MockWrapper.Verify(w => w.WaitForApplicationInputIdle(app, TestContext.Current.CancellationToken), Times.Once()),
            () => this.MockWrapper.Verify(w => w.MacroButtonIsDirty(), Times.Never())
        );
    }

    [Fact]
    public async Task ThrowsInvalidOperationExceptionWhenAppIsVoicemeeterAndNotLoggedIn()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await this.Remote.RunAsync(App.Bananax64, TestContext.Current.CancellationToken)
        );

        this.MockWrapper.Verify(w => w.RunVoicemeeter(It.IsAny<App>()), Times.Never());
    }

    [Fact]
    public async Task GenericThrowsArgumentExceptionWhenTypeNotSupported()
    {
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await ((IRemote)this.Remote).RunAsync(RunResponse.Ok, TestContext.Current.CancellationToken)
        );

        this.MockWrapper.Verify(w => w.RunVoicemeeter(It.IsAny<App>()), Times.Never());
    }
}
