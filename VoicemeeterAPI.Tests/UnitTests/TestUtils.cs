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
        this.Remote.Dispose();

        GC.SuppressFinalize(this);
    }

    protected void MockLogin()
        => this.MockLogin(RunResponse.Ok);

    protected void MockLogin(int kind, int version)
        => this.MockLogin(RunResponse.Ok, kind, version);

    protected void MockLogin(RunResponse buttonsState)
        => this.MockLogin_p(LoginResponse.VoicemeeterNotRunning, buttonsState, (int)Kind.None, 0x0000_0000);

    protected void MockLogin(RunResponse buttonsState, int kind, int version)
        => this.MockLogin_p(LoginResponse.Ok, buttonsState, kind, version);

    private void MockLogin_p(LoginResponse loginStatus, RunResponse buttonsState, int kind, int version)
    {
        VmVersion runningVersion = default;
        var infoResponse = InfoResponse.NoServer;

        if (version != 0)
        {
            runningVersion = (VmVersion)version;
            infoResponse = InfoResponse.Ok;
        }

        var expectedState = new ConnectionState(loginStatus, buttonsState, (Kind)kind, runningVersion);

        this.MockWrapper.Setup(w => w.Login()).Returns(loginStatus);
        this.MockWrapper.Setup(w => w.GetVoicemeeterType()).Returns((infoResponse, kind));
        this.MockWrapper.Setup(w => w.GetVoicemeeterVersion()).Returns((infoResponse, version));
        this.MockWrapper.Setup(w => w.GetApplicationState(App.MacroButtons)).Returns(buttonsState);

        var result = this.Remote.Login();

        Assert.Multiple(
            () => Assert.Equal(loginStatus, result),
            () => Assert.Equal(expectedState, this.Remote.LastConnectionState),
            () => this.MockWrapper.Verify(w => w.Login(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterType(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(App.MacroButtons), Times.Once())
        );
    }
}
