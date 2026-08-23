namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.Login;

public class LoginAsync : MockRemote
{
    [Fact]
    public async Task WaitsForEngineToSettleWhenAllConditionsMet()
    {
        var loginStatus = LoginResponse.Ok;
        var vmState = RunResponse.Ok;
        var vmApp = App.Potato;
        var response = Response.Ok;
        var vmVersion = VmVersion.MaxValid;
        var buttonsState = RunResponse.Hidden;
        var expectedState = new ConnectionState(loginStatus, vmState, vmApp, vmVersion, buttonsState);

        this.MockWrapper.Setup(w => w.Login()).Returns(loginStatus);
        this.MockWrapper.Setup(w => w.GetVoicemeeterKind()).Returns((response, vmVersion.K));
        this.MockWrapper.Setup(w => w.GetVoicemeeterState()).Returns((vmApp, vmState));
        this.MockWrapper.Setup(w => w.GetVoicemeeterVersion()).Returns((response, vmVersion));
        this.MockWrapper.Setup(w => w.GetApplicationState(App.MacroButtons)).Returns(buttonsState);

        this.MockWrapper.SetupSequence(w => w.IsParametersDirty())
            .Returns(Response.Dirty)
            .Returns(Response.Dirty)
            .Returns(Response.Ok);

        this.MockWrapper.SetupSequence(w => w.MacroButtonIsDirty())
            .Returns(Response.Dirty)
            .Returns(Response.Dirty)
            .Returns(Response.Ok);

        var result = await this.Remote.LoginAsync(TestContext.Current.CancellationToken);

        Assert.Multiple(
            () => Assert.Equal(expectedState, result),
            () => Assert.Equal(expectedState, this.Remote.ConnectionState),
            () => this.MockWrapper.Verify(w => w.Login(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterKind(), Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterState(), Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(), Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(App.MacroButtons), Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.IsParametersDirty(), Times.Exactly(3)),
            () => this.MockWrapper.Verify(w => w.MacroButtonIsDirty(), Times.Exactly(3))
        );
    }
}
