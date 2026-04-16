using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;
using PBLivingston.VoicemeeterAPI.Utilities;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.RemoteTests.Login;

public class Run : MockRemote
{
    [Fact]
    public void LaunchesApp_WhenValid()
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
    public void ThrowsException_Run_WhenUnknownApp()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var app = (int)App.Unknown;

        MockWrapper.Setup(w => w.RunVoicemeeter(app)).Returns(RunResponse.UnknownApp);

        MockLogin_Ok(kind, version);

        var ex = Assert.Throws<RunException>(() => Remote.Run(app));

        Assert.Multiple(
            () => Assert.Equal(app, (int)ex.App),
            () => Assert.Equal(RunResponse.UnknownApp, ex.Response),
            () => MockWrapper.Verify(w => w.RunVoicemeeter(app), Times.Once)
        );
    }

    [Fact]
    public void ThrowsException_Run_WhenUnexpectedResponse()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var app = (int)App.StreamerView;

        MockWrapper.Setup(w => w.RunVoicemeeter(app)).Returns(RunResponse.NotInstalled);

        MockLogin_Ok(kind, version);

        var ex = Assert.Throws<RunException>(() => Remote.Run(app));

        Assert.Multiple(
            () => Assert.Equal(app, (int)ex.App),
            () => Assert.Equal(RunResponse.NotInstalled, ex.Response),
            () => MockWrapper.Verify(w => w.RunVoicemeeter(app), Times.Once)
        );
    }

    [Fact]
    public void ThrowsException_RemoteAccess_WhenLoginStatusLoggedOut()
    {
        var app = (int)App.DeviceCheck;

        var ex = Assert.Throws<RemoteAccessException>(() => Remote.Run(app));

        Assert.Multiple(
            () => Assert.Equal("Run", ex.Method),
            () => Assert.Equal(LoginResponse.LoggedOut, ex.LoginStatus),
            () => MockWrapper.Verify(w => w.RunVoicemeeter(app), Times.Never)
        );
    }

    [Fact]
    public void ThrowsException_ObjectDisposed_WhenRemoteDisposed()
    {
        var app = (int)App.BUSGEQ15;

        Remote.Dispose();

        var ex = Assert.Throws<ObjectDisposedException>(() => Remote.Run(app));

        Assert.Multiple(
            () => Assert.Equal("Remote", ex.ObjectName),
            () => MockWrapper.Verify(w => w.RunVoicemeeter(app), Times.Never)
        );
    }

    [Fact]
    public void App_Calls_ButtonsDirty_WhenAppIsMacroButtons()
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

        Assert.Multiple(
            () => MockWrapper.Verify(w => w.RunVoicemeeter((int)app), Times.Once),
            () => MockWrapper.Verify(w => w.MacroButtonIsDirty(), Times.Exactly(3))
        );
    }

    [Fact]
    public void Kind_UpdatesConnectionState_Ok_WhenAllConditionsMet()
    {
        var kind = (int)Kind.Standard;
        var version = 0x0101_0202;
        var expectedState = new ConnectionStateRecord(LoginResponse.Ok, (Kind)kind, (VmVersion)version);
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

        Assert.Multiple(
            () => Assert.Equal(expectedState, Remote.ConnectionState),
            () => MockWrapper.Verify(w => w.RunVoicemeeter((int)app), Times.Once)
        );
    }

    [Fact]
    public void Kind_ThrowsException_Run_WhenWaitForRunningTimesOut()
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
            () => Assert.Equal(RunResponse.Timeout, ex.Response),
            () => MockWrapper.Verify(w => w.RunVoicemeeter((int)app), Times.Once)
        );
    }

    [Fact]
    public void String_ThrowsException_Argument_WhenInvalidString()
    {
        var ex = Assert.Throws<ArgumentException>(() => Remote.Run("InvalidApp"));

        Assert.Multiple(
            () => Assert.Equal("app", ex.ParamName),
            () => MockWrapper.Verify(w => w.RunVoicemeeter(It.IsAny<int>()), Times.Never)
        );
    }

    [Fact]
    public void GenericInt_Calls_CorrectOverload()
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
    public void GenericApp_Calls_CorrectOverload()
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
    public void GenericKind_Calls_CorrectOverload()
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
    public void GenericString_Calls_CorrectOverload()
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
    public void Generic_ThrowsException_TypeNotSupported_WhenInvalidType()
    {
        var ex = Assert.Throws<TypeNotSupportedException<float>>(() => ((IRemote)Remote).Run(10.0f));

        Assert.Multiple(
            () => Assert.Equal("Run", ex.Method),
            () => Assert.Equal("app", ex.Param),
            () => Assert.Equal(typeof(float), ex.Type),
            () => Assert.Equal(SupportedTypes.RunTypes, ex.Supported),
            () => MockWrapper.Verify(w => w.RunVoicemeeter(It.IsAny<int>()), Times.Never)
        );
    }
}