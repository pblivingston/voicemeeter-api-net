namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests;

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit.Sdk;
using PBLivingston.VoicemeeterAPI.Types;

internal static class CaseTagExt
{
    public static bool HasAny<T>(this T tags, T mask) where T : struct, Enum
    {
        var size = Unsafe.SizeOf<T>();
        return size switch
        {
            1 => (Unsafe.As<T, byte>(ref tags) & Unsafe.As<T, byte>(ref mask)) != 0,
            2 => (Unsafe.As<T, short>(ref tags) & Unsafe.As<T, short>(ref mask)) != 0,
            4 => (Unsafe.As<T, int>(ref tags) & Unsafe.As<T, int>(ref mask)) != 0,
            8 => (Unsafe.As<T, long>(ref tags) & Unsafe.As<T, long>(ref mask)) != 0,
            _ => throw new NotSupportedException($"Enum size {size} is not supported.")
        };
    }
}

public abstract record SerializableRecord : IXunitSerializable
{
    public SerializableRecord() { }

    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = false,
        Converters = { new JsonStringEnumConverter() }
    };

    public void Serialize(IXunitSerializationInfo info)
        => info.AddValue("json", JsonSerializer.Serialize(this, Options));

    public void Deserialize(IXunitSerializationInfo info)
    {
        var json = info.GetValue<string>("json");
        if (string.IsNullOrEmpty(json))
        {
            return;
        }

        var data = JsonSerializer.Deserialize(json, this.GetType(), Options);
        if (null == data)
        {
            return;
        }

        var props = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in props.Where(p => p.CanWrite))
        {
            prop.SetValue(this, prop.GetValue(data));
        }
    }
}

public abstract class MockRemote : IDisposable
{
    internal Mock<IWrapper> MockWrapper { get; }
    protected Remote Remote { get; }

    protected MockRemote()
    {
        this.MockWrapper = new Mock<IWrapper>();
        this.Remote = new Remote(this.MockWrapper.Object);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        this.Remote.Dispose();
    }

    protected void MockLoginOk(int kind, int version)
    {
        var loginStatus = LoginResponse.Ok;
        var expectedState = new ConnectionStateEventArgs(loginStatus, true, (Kind)kind, (VmVersion)version);

        this.MockWrapper.Setup(w => w.Login()).Returns(loginStatus);
        this.MockWrapper.Setup(w => w.GetVoicemeeterType(out kind)).Returns(InfoResponse.Ok);
        this.MockWrapper.Setup(w => w.GetVoicemeeterVersion(out version)).Returns(InfoResponse.Ok);
        this.MockWrapper.Setup(w => w.MacroButtonIsRunning()).Returns(RunResponse.Ok);
        this.MockWrapper.Setup(w => w.IsParametersDirty()).Returns(Response.Ok);
        this.MockWrapper.Setup(w => w.MacroButtonIsDirty()).Returns(Response.Ok);

        var result = this.Remote.Login();

        Assert.Multiple(
            () => Assert.Equal(loginStatus, result),
            () => Assert.Equal(expectedState, this.Remote.GetConnectionState()),
            () => this.MockWrapper.Verify(w => w.Login(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterType(out kind), Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(out version), Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.MacroButtonIsRunning(), Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.IsParametersDirty(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.MacroButtonIsDirty(), Times.Once())
        );
    }

    protected void MockLoginVoicemeeterNotRunning()
    {
        var kind = (int)Kind.None;
        var version = 0x0000_0000;
        var loginStatus = LoginResponse.VoicemeeterNotRunning;
        var expectedState = new ConnectionStateEventArgs(loginStatus, true, Kind.None, default);

        this.MockWrapper.Setup(w => w.Login()).Returns(loginStatus);
        this.MockWrapper.Setup(w => w.GetVoicemeeterType(out kind)).Returns(InfoResponse.NoServer);
        this.MockWrapper.Setup(w => w.GetVoicemeeterVersion(out version)).Returns(InfoResponse.NoServer);
        this.MockWrapper.Setup(w => w.MacroButtonIsRunning()).Returns(RunResponse.Ok);

        var result = this.Remote.Login();

        Assert.Multiple(
            () => Assert.Equal(loginStatus, result),
            () => Assert.Equal(expectedState, this.Remote.GetConnectionState()),
            () => this.MockWrapper.Verify(w => w.Login(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterType(out kind), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(out version), Times.Once()),
            () => this.MockWrapper.Verify(w => w.MacroButtonIsRunning(), Times.Exactly(2))
        );
    }

    protected void MockLoginMacroButtonsNotRunning(int kind, int version)
    {
        var loginStatus = LoginResponse.Ok;
        var expectedState = new ConnectionStateEventArgs(loginStatus, false, (Kind)kind, (VmVersion)version);

        this.MockWrapper.Setup(w => w.Login()).Returns(loginStatus);
        this.MockWrapper.Setup(w => w.GetVoicemeeterType(out kind)).Returns(InfoResponse.Ok);
        this.MockWrapper.Setup(w => w.GetVoicemeeterVersion(out version)).Returns(InfoResponse.Ok);
        this.MockWrapper.Setup(w => w.MacroButtonIsRunning()).Returns(RunResponse.NotRunning);
        this.MockWrapper.Setup(w => w.IsParametersDirty()).Returns(Response.Ok);
        this.MockWrapper.Setup(w => w.MacroButtonIsDirty()).Returns(Response.Ok);

        var result = this.Remote.Login();

        Assert.Multiple(
            () => Assert.Equal(loginStatus, result),
            () => Assert.Equal(expectedState, this.Remote.GetConnectionState()),
            () => this.MockWrapper.Verify(w => w.Login(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterType(out kind), Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(out version), Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.MacroButtonIsRunning(), Times.Exactly(2)),
            () => this.MockWrapper.Verify(w => w.IsParametersDirty(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.MacroButtonIsDirty(), Times.Once())
        );
    }

    protected void MockLoginNotRunning()
    {
        var kind = (int)Kind.None;
        var version = 0x0000_0000;
        var loginStatus = LoginResponse.VoicemeeterNotRunning;
        var expectedState = new ConnectionStateEventArgs(loginStatus, false, Kind.None, default);

        this.MockWrapper.Setup(w => w.Login()).Returns(loginStatus);
        this.MockWrapper.Setup(w => w.GetVoicemeeterType(out kind)).Returns(InfoResponse.NoServer);
        this.MockWrapper.Setup(w => w.GetVoicemeeterVersion(out version)).Returns(InfoResponse.NoServer);
        this.MockWrapper.Setup(w => w.MacroButtonIsRunning()).Returns(RunResponse.NotRunning);

        var result = this.Remote.Login();

        Assert.Multiple(
            () => Assert.Equal(loginStatus, result),
            () => Assert.Equal(expectedState, this.Remote.GetConnectionState()),
            () => this.MockWrapper.Verify(w => w.Login(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterType(out kind), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(out version), Times.Once()),
            () => this.MockWrapper.Verify(w => w.MacroButtonIsRunning(), Times.Exactly(2))
        );
    }
}
