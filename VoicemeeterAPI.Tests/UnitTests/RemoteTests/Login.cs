using PBLivingston.VoicemeeterAPI.Messages;
using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests;

public class Login : MockRemote
{
    #region Login

    [Fact]
    public void Login_UpdatesLoginStatus_Ok_WhenAllConditionsMet()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;

        MockWrapper.Setup(w => w.Login()).Returns(LoginResponse.Ok);
        MockWrapper.Setup(w => w.GetVoicemeeterType(out kind)).Returns(InfoResponse.Ok);
        MockWrapper.Setup(w => w.GetVoicemeeterVersion(out version)).Returns(InfoResponse.Ok);

        MockWrapper.SetupSequence(w => w.IsParametersDirty())
            .Returns(Response.Dirty)
            .Returns(Response.Ok);
        MockWrapper.SetupSequence(w => w.MacroButtonIsDirty())
            .Returns(Response.Dirty)
            .Returns(Response.Ok);

        Remote.Login();

        Assert.Equal(LoginResponse.Ok, Remote.LoginStatus);
        MockWrapper.Verify(w => w.Login(), Times.Once);
    }

    [Fact]
    public void Login_UpdatesLoginStatus_VoicemeeterNotRunning_WhenVoicemeeterNotRunning()
    {
        MockWrapper.Setup(w => w.Login()).Returns(LoginResponse.VoicemeeterNotRunning);

        Remote.Login();

        Assert.Equal(LoginResponse.VoicemeeterNotRunning, Remote.LoginStatus);
        MockWrapper.Verify(w => w.Login(), Times.Once);
    }

    [Theory]
    [InlineData(LoginResponse.AlreadyLoggedIn)]
    [InlineData(LoginResponse.NoClient)]
    public void Login_ThrowsException_Login_WhenLoginFails(LoginResponse expectedResponse)
    {
        MockWrapper.Setup(w => w.Login()).Returns(expectedResponse);

        var ex = Assert.Throws<LoginException>(() => Remote.Login());
        Assert.Equal(expectedResponse, ex.Response);
        MockWrapper.Verify(w => w.Login(), Times.Once);
    }

    [Fact]
    public void Login_ThrowsException_Login_WhenWaitForRunningTimesOut()
    {
        var kind = (int)Kind.None;
        var version = 0;

        MockWrapper.Setup(w => w.Login()).Returns(LoginResponse.Ok);
        MockWrapper.Setup(w => w.GetVoicemeeterType(out kind)).Returns(InfoResponse.NoServer);
        MockWrapper.Setup(w => w.GetVoicemeeterVersion(out version)).Returns(InfoResponse.NoServer);

        var ex = Assert.Throws<LoginException>(() => Remote.Login(timeoutMs: 10));
        Assert.Equal(LoginResponse.Timeout, ex.Response);
        MockWrapper.Verify(w => w.Login(), Times.Once);
    }

    [Fact]
    public void Login_ThrowsException_Remote_WhenAlreadyLoggedIn()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;

        MockLogin_Ok(kind, version);

        var ex = Assert.Throws<RemoteException>(() => Remote.Login());
        Assert.Equal($"[VoicemeeterAPI] Remote Error: Already logged in - LoginStatus: Ok", ex.Message);
        MockWrapper.Verify(w => w.Login(), Times.Once);
    }

    [Fact]
    public void Login_ThrowsException_RemoteAccess_WhenLoginStatusUnknown()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;

        MockWrapper.Setup(w => w.Logout()).Returns(LoginResponse.NoClient);

        MockLogin_Ok(kind, version);

        Remote.Logout(timeoutMs: 10);

        var ex = Assert.Throws<RemoteAccessException>(() => Remote.Login());
        Assert.Multiple(
            () => Assert.Equal("Login", ex.Method),
            () => Assert.Equal(LoginResponse.Unknown, ex.LoginStatus)
        );
        MockWrapper.Verify(w => w.Login(), Times.Once);
    }

    [Fact]
    public void Login_ThrowsException_ObjectDisposed_WhenRemoteDisposed()
    {
        Remote.Dispose();

        var ex = Assert.Throws<ObjectDisposedException>(() => Remote.Login());
        Assert.Equal("Remote", ex.ObjectName);
        MockWrapper.Verify(w => w.Login(), Times.Never);
    }

    #endregion

    #region Logout

    [Fact]
    public void Logout_UpdatesLoginStatus_LoggedOut_WhenSuccessful()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;

        MockWrapper.Setup(w => w.Logout()).Returns(LoginResponse.Ok);

        MockLogin_Ok(kind, version);

        Remote.Logout();

        Assert.Equal(LoginResponse.LoggedOut, Remote.LoginStatus);
        MockWrapper.Verify(w => w.Logout(), Times.Once);
    }

    [Fact]
    public void Logout_UpdatesLoginStatus_Unknown_WhenTimesOut()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;

        MockWrapper.Setup(w => w.Logout()).Returns(LoginResponse.NoClient);

        MockLogin_Ok(kind, version);

        Remote.Logout(timeoutMs: 10);

        Assert.Equal(LoginResponse.Unknown, Remote.LoginStatus);
        MockWrapper.Verify(w => w.Logout(), Times.Once);
    }

    [Fact]
    public void Logout_ThrowsException_Remote_WhenAlreadyLoggedOut()
    {
        var ex = Assert.Throws<RemoteException>(() => Remote.Logout());
        Assert.Equal("[VoicemeeterAPI] Remote Error: Already logged out", ex.Message);
        MockWrapper.Verify(w => w.Logout(), Times.Never);
    }

    [Fact]
    public void Logout_ThrowsException_ObjectDisposed_WhenRemoteDisposed()
    {
        Remote.Dispose();

        var ex = Assert.Throws<ObjectDisposedException>(() => Remote.Logout());
        Assert.Equal("Remote", ex.ObjectName);
        MockWrapper.Verify(w => w.Logout(), Times.Never);
    }

    #endregion

    #region Run

    [Fact]
    public void Run_LaunchesApp_WhenValid()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var app = (int)App.AUXControlPanel;

        MockWrapper.Setup(w => w.RunVoicemeeter(app)).Returns(RunResponse.Ok);

        MockLogin_Ok(kind, version);

        Remote.Run(app);

        MockWrapper.Verify(w => w.RunVoicemeeter(app), Times.Once);
    }

    [Fact]
    public void Run_ThrowsException_Run_WhenUnknownApp()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var app = (int)App.Unknown;

        MockWrapper.Setup(w => w.RunVoicemeeter(app)).Returns(RunResponse.UnknownApp);

        MockLogin_Ok(kind, version);

        var ex = Assert.Throws<RunException>(() => Remote.Run(app));
        Assert.Multiple(
            () => Assert.Equal(app, (int)ex.App),
            () => Assert.Equal(RunResponse.UnknownApp, ex.Response)
        );
        MockWrapper.Verify(w => w.RunVoicemeeter(app), Times.Once);
    }

    [Fact]
    public void Run_ThrowsException_Run_WhenUnexpectedResponse()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var app = (int)App.StreamerView;

        MockWrapper.Setup(w => w.RunVoicemeeter(app)).Returns(RunResponse.NotInstalled);

        MockLogin_Ok(kind, version);

        var ex = Assert.Throws<RunException>(() => Remote.Run(app));
        Assert.Multiple(
            () => Assert.Equal(app, (int)ex.App),
            () => Assert.Equal(RunResponse.NotInstalled, ex.Response)
        );
        MockWrapper.Verify(w => w.RunVoicemeeter(app), Times.Once);
    }

    [Fact]
    public void Run_ThrowsException_RemoteAccess_WhenLoginStatusLoggedOut()
    {
        var app = (int)App.DeviceCheck;

        var ex = Assert.Throws<RemoteAccessException>(() => Remote.Run(app));
        Assert.Multiple(
            () => Assert.Equal("Run", ex.Method),
            () => Assert.Equal(LoginResponse.LoggedOut, ex.LoginStatus)
        );
        MockWrapper.Verify(w => w.RunVoicemeeter(app), Times.Never);
    }

    [Fact]
    public void Run_ThrowsException_ObjectDisposed_WhenRemoteDisposed()
    {
        var app = (int)App.BUSGEQ15;

        Remote.Dispose();

        var ex = Assert.Throws<ObjectDisposedException>(() => Remote.Run(app));
        Assert.Equal("Remote", ex.ObjectName);
        MockWrapper.Verify(w => w.RunVoicemeeter(app), Times.Never);
    }

    [Fact]
    public void Run_App_Calls_ButtonsDirty_WhenAppIsMacroButtons()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var app = App.MacroButtons;

        MockWrapper.Setup(w => w.RunVoicemeeter((int)app)).Returns(RunResponse.Ok);

        MockLogin_Ok(kind, version);

        MockWrapper.SetupSequence(w => w.MacroButtonIsDirty())
            .Returns(Response.Dirty)
            .Returns(Response.Ok);

        Remote.Run(app);

        MockWrapper.Verify(w => w.RunVoicemeeter((int)app), Times.Once);
        MockWrapper.Verify(w => w.MacroButtonIsDirty(), Times.Exactly(3));
    }

    [Fact]
    public void Run_Kind_UpdatesLoginStatus_Ok_WhenAllConditionsMet()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var app = Environment.Is64BitProcess ? App.Standardx64 : App.Standard;

        MockWrapper.Setup(w => w.RunVoicemeeter((int)app)).Returns(RunResponse.Ok);
        MockWrapper.Setup(w => w.GetVoicemeeterType(out kind)).Returns(InfoResponse.Ok);
        MockWrapper.Setup(w => w.GetVoicemeeterVersion(out version)).Returns(InfoResponse.Ok);

        MockWrapper.SetupSequence(w => w.IsParametersDirty())
            .Returns(Response.Dirty)
            .Returns(Response.Ok);
        MockWrapper.SetupSequence(w => w.MacroButtonIsDirty())
            .Returns(Response.Dirty)
            .Returns(Response.Ok);

        MockLogin_VoicemeeterNotRunning();

        Remote.Run(Kind.Standard);

        Assert.Equal(LoginResponse.Ok, Remote.LoginStatus);
        MockWrapper.Verify(w => w.RunVoicemeeter((int)app), Times.Once);
    }

    [Fact]
    public void Run_Kind_ThrowsException_Run_WhenWaitForRunningTimesOut()
    {
        var kind = (int)Kind.None;
        var version = 0;
        var app = Environment.Is64BitProcess ? App.Standardx64 : App.Standard;

        MockWrapper.Setup(w => w.RunVoicemeeter((int)app)).Returns(RunResponse.Ok);
        MockWrapper.Setup(w => w.GetVoicemeeterType(out kind)).Returns(InfoResponse.NoServer);
        MockWrapper.Setup(w => w.GetVoicemeeterVersion(out version)).Returns(InfoResponse.NoServer);

        MockLogin_VoicemeeterNotRunning();

        var ex = Assert.Throws<RunException>(() => Remote.Run(Kind.Standard, timeoutMs: 10));
        Assert.Multiple(
            () => Assert.Equal(app, ex.App),
            () => Assert.Equal(RunResponse.Timeout, ex.Response)
        );
        MockWrapper.Verify(w => w.RunVoicemeeter((int)app), Times.Once);
    }

    [Fact]
    public void Run_String_ThrowsException_Argument_WhenInvalidString()
    {
        var ex = Assert.Throws<ArgumentException>(() => Remote.Run("InvalidApp"));
        Assert.Equal("app", ex.ParamName);
        MockWrapper.Verify(w => w.RunVoicemeeter(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void Run_GenericInt_Calls_CorrectOverload()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var app = (int)App.VBAN2MIDI;

        MockWrapper.Setup(w => w.RunVoicemeeter(app)).Returns(RunResponse.Ok);

        MockLogin_Ok(kind, version);

        ((IRemote)Remote).Run(app);

        MockWrapper.Verify(w => w.RunVoicemeeter(app), Times.Once);
    }

    [Fact]
    public void Run_GenericApp_Calls_CorrectOverload()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var app = App.BUSMatrix8;

        MockWrapper.Setup(w => w.RunVoicemeeter((int)app)).Returns(RunResponse.Ok);

        MockLogin_Ok(kind, version);

        ((IRemote)Remote).Run(app);

        MockWrapper.Verify(w => w.RunVoicemeeter((int)app), Times.Once);
    }

    [Fact]
    public void Run_GenericKind_Calls_CorrectOverload()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var app = Environment.Is64BitProcess ? App.Standardx64 : App.Standard;

        MockWrapper.Setup(w => w.RunVoicemeeter((int)app)).Returns(RunResponse.Ok);
        MockWrapper.Setup(w => w.GetVoicemeeterType(out kind)).Returns(InfoResponse.Ok);
        MockWrapper.Setup(w => w.GetVoicemeeterVersion(out version)).Returns(InfoResponse.Ok);
        MockWrapper.Setup(w => w.IsParametersDirty()).Returns(Response.Ok);
        MockWrapper.Setup(w => w.MacroButtonIsDirty()).Returns(Response.Ok);

        MockLogin_VoicemeeterNotRunning();

        ((IRemote)Remote).Run(Kind.Standard);

        MockWrapper.Verify(w => w.RunVoicemeeter((int)app), Times.Once);
    }

    [Fact]
    public void Run_GenericString_Calls_CorrectOverload()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var app = App.CABLEControlPanel;

        MockWrapper.Setup(w => w.RunVoicemeeter((int)app)).Returns(RunResponse.Ok);

        MockLogin_Ok(kind, version);

        ((IRemote)Remote).Run("CABLEControlPanel");

        MockWrapper.Verify(w => w.RunVoicemeeter((int)app), Times.Once);
    }

    [Fact]
    public void Run_Generic_ThrowsException_Argument_WhenInvalidType()
    {
        var ex = Assert.Throws<ArgumentException>(() => ((IRemote)Remote).Run(10.0f));
        Assert.Equal("app", ex.ParamName);
        MockWrapper.Verify(w => w.RunVoicemeeter(It.IsAny<int>()), Times.Never);
    }

    #endregion
}