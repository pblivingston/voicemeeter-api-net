namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.Login;

using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;

public class LoginAsync : MockRemote
{
    [Fact]
    public async Task UpdatesLastConnectionStateOkWhenAllConditionsMet()
    {
        var loginStatus = LoginResponse.Ok;
        var buttonsState = RunResponse.Ok;
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var expectedState = new ConnectionState(loginStatus, buttonsState, (Kind)kind, (VmVersion)version);

        this.MockWrapper.Setup(w => w.Login()).Returns(loginStatus);
        this.MockWrapper.Setup(w => w.GetApplicationState(App.MacroButtons)).Returns(buttonsState);

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

        var result = await this.Remote.LoginAsync(TestContext.Current.CancellationToken);

        Assert.Multiple(
            () => Assert.Equal(loginStatus, result),
            () => Assert.Equal(expectedState, this.Remote.LastConnectionState),
            () => this.MockWrapper.Verify(w => w.Login(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterType(), Times.Exactly(4)),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(), Times.Exactly(4)),
            () => this.MockWrapper.Verify(w => w.IsParametersDirty(), Times.Exactly(3)),
            () => this.MockWrapper.Verify(w => w.MacroButtonIsDirty(), Times.Exactly(3)),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(App.MacroButtons), Times.Once())
        );
    }

    [Fact]
    public async Task UpdatesLastLoginStatusVoicemeeterNotRunningWhenVoicemeeterNotRunning()
    {
        var loginStatus = LoginResponse.VoicemeeterNotRunning;
        var buttonsState = RunResponse.Ok;
        var kind = (int)Kind.None;
        var version = 0x0000_0000;
        var expectedState = new ConnectionState(loginStatus, buttonsState, Kind.None, default);

        this.MockWrapper.Setup(w => w.Login()).Returns(loginStatus);
        this.MockWrapper.Setup(w => w.GetVoicemeeterType()).Returns((InfoResponse.NoServer, kind));
        this.MockWrapper.Setup(w => w.GetVoicemeeterVersion()).Returns((InfoResponse.NoServer, version));
        this.MockWrapper.Setup(w => w.GetApplicationState(App.MacroButtons)).Returns(buttonsState);

        var result = await this.Remote.LoginAsync(TestContext.Current.CancellationToken);

        Assert.Multiple(
            () => Assert.Equal(loginStatus, result),
            () => Assert.Equal(expectedState, this.Remote.LastConnectionState),
            () => this.MockWrapper.Verify(w => w.Login(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterType(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(App.MacroButtons), Times.Once())
        );
    }

    [Theory]
    [InlineData(RunResponse.Hidden)]
    [InlineData(RunResponse.NotRunning)]
    [InlineData(RunResponse.NotResponding)]
    public async Task UpdatesLastButtonsStateWhenButtonsIsState(RunResponse buttonsState)
    {
        var loginStatus = LoginResponse.Ok;
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var expectedState = new ConnectionState(loginStatus, buttonsState, (Kind)kind, (VmVersion)version);

        this.MockWrapper.Setup(w => w.Login()).Returns(loginStatus);
        this.MockWrapper.Setup(w => w.GetVoicemeeterType()).Returns((InfoResponse.Ok, kind));
        this.MockWrapper.Setup(w => w.GetVoicemeeterVersion()).Returns((InfoResponse.Ok, version));
        this.MockWrapper.Setup(w => w.IsParametersDirty()).Returns(Response.Ok);
        this.MockWrapper.Setup(w => w.MacroButtonIsDirty()).Returns(Response.Ok);
        this.MockWrapper.Setup(w => w.GetApplicationState(App.MacroButtons)).Returns(buttonsState);

        var result = await this.Remote.LoginAsync(TestContext.Current.CancellationToken);

        Assert.Multiple(
            () => Assert.Equal(loginStatus, result),
            () => Assert.Equal(expectedState, this.Remote.LastConnectionState),
            () => this.MockWrapper.Verify(w => w.Login(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterType(), Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(), Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.IsParametersDirty(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.MacroButtonIsDirty(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(App.MacroButtons), Times.Once())
        );
    }

    [Fact]
    public async Task ThrowsExceptionRemoteWhenWaitForVoicemeeterTimesOut()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        this.MockWrapper.Setup(w => w.Login()).Returns(LoginResponse.Ok);

        var ex = await Assert.ThrowsAsync<RemoteException<LoginResponse>>(async () => await this.Remote.LoginAsync(cts.Token));

        Assert.Multiple(
            () => Assert.Equal(LoginResponse.Timeout, ex.Response),
            () => this.MockWrapper.Verify(w => w.Login(), Times.Once())
        );
    }

    [Theory]
    [InlineData(LoginResponse.AlreadyLoggedIn)]
    [InlineData(LoginResponse.NoClient)]
    public async Task ThrowsExceptionRemoteWhenLoginFails(LoginResponse expectedResponse)
    {
        this.MockWrapper.Setup(w => w.Login()).Returns(expectedResponse);

        var ex = await Assert.ThrowsAsync<RemoteException<LoginResponse>>(async () => await this.Remote.LoginAsync(TestContext.Current.CancellationToken));

        Assert.Multiple(
            () => Assert.Equal(expectedResponse, ex.Response),
            () => this.MockWrapper.Verify(w => w.Login(), Times.Once())
        );
    }

    [Fact]
    public async Task ThrowsExceptionAccessDeniedWhenAlreadyLoggedIn()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;

        this.MockLogin(kind, version);

        var ex = await Assert.ThrowsAsync<AccessDeniedException>(async () => await this.Remote.LoginAsync(TestContext.Current.CancellationToken));

        Assert.Multiple(
            () => Assert.Equal(LoginResponse.Ok, ex.LoginStatus),
            () => this.MockWrapper.Verify(w => w.Login(), Times.Once())
        );
    }

    [Fact]
    public async Task ThrowsExceptionAccessDeniedWhenLoginStatusUnknown()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;

        this.MockWrapper.Setup(w => w.Logout()).Returns(LoginResponse.NoClient);

        this.MockLogin(kind, version);

        this.Remote.Logout();

        var ex = await Assert.ThrowsAsync<AccessDeniedException>(async () => await this.Remote.LoginAsync(TestContext.Current.CancellationToken));

        Assert.Multiple(
            () => Assert.Equal(LoginResponse.Unknown, ex.LoginStatus),
            () => this.MockWrapper.Verify(w => w.Login(), Times.Once())
        );
    }

    [Fact]
    public async Task ThrowsExceptionObjectDisposedWhenRemoteDisposed()
    {
        this.Remote.Dispose();

        await Assert.ThrowsAsync<ObjectDisposedException>(async () => await this.Remote.LoginAsync(TestContext.Current.CancellationToken));
        this.MockWrapper.Verify(w => w.Login(), Times.Never());
    }
}
