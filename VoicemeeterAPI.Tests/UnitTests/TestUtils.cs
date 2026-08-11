namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests;

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit.Sdk;

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
    internal Mock<Remote.IWrapper> MockWrapper { get; }
    protected Remote Remote { get; }

    protected MockRemote()
    {
        this.MockWrapper = new Mock<Remote.IWrapper>();
        this.Remote = new Remote(this.MockWrapper.Object);
    }

    public void Dispose()
    {
        this.Remote.Dispose();

        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///   Sets up and performs a mock login sequence where:<br/>
    ///   Voicemeeter is not running<br/>
    ///   MacroButtons is not running<br/>
    /// </summary>
    /// <inheritdoc cref="MockLogin_p"/>
    protected void MockLogin()
        => this.MockLogin(RunResponse.NotRunning);

    /// <summary>
    ///   Sets up and performs a mock login sequence where:<br/>
    ///   Voicemeeter is not running<br/>
    ///   MacroButtons is the given state<br/>
    /// </summary>
    /// <inheritdoc cref="MockLogin_p"/>
    protected void MockLogin(RunResponse buttonsState)
        => this.MockLogin_p(LoginResponse.VoicemeeterNotRunning, RunResponse.NotRunning, App.None, default, buttonsState);

    /// <summary>
    ///   Sets up and performs a mock login sequence where:<br/>
    ///   The given Voicemeeter app/version is the given state<br/>
    ///   MacroButtons is not running<br/>
    /// </summary>
    /// <inheritdoc cref="MockLogin_p"/>
    protected void MockLogin(RunResponse vmState, App vmApp, VmVersion vmVersion)
        => this.MockLogin(vmState, vmApp, vmVersion, RunResponse.NotRunning);

    /// <summary>
    ///   Sets up and performs a mock login sequence where:<br/>
    ///   The given Voicemeeter app/version is the given state<br/>
    ///   MacroButtons is the given state<br/>
    /// </summary>
    /// <inheritdoc cref="MockLogin_p"/>
    protected void MockLogin(RunResponse vmState, App vmApp, VmVersion vmVersion, RunResponse buttonsState)
        => this.MockLogin_p(LoginResponse.Ok, vmState, vmApp, vmVersion, buttonsState);

    /// <summary>
    ///   Sets up and performs a mock login sequence
    /// </summary>
    /// <param name="loginStatus"></param>
    /// <param name="buttonsState"></param>
    /// <param name="kind"></param>
    /// <param name="version"></param>
    /// <remarks>
    ///   Calls:<br/>
    ///   <see cref="Remote.IWrapper.Login()"/> once<br/>
    ///   <see cref="Remote.IWrapper.GetVoicemeeterState()"/> once<br/>
    ///   <see cref="Remote.IWrapper.GetVoicemeeterVersion()"/> once<br/>
    ///   <see cref="Remote.IWrapper.GetApplicationState(App)"/> once with <see cref="App.MacroButtons"/><br/>
    /// </remarks>
    private void MockLogin_p(LoginResponse loginStatus, RunResponse vmState, App vmApp, VmVersion vmVersion, RunResponse buttonsState)
    {
        var expectedState = new ConnectionState(loginStatus, vmState, vmApp, vmVersion, buttonsState);

        var versionResponse = vmApp is App.None
            ? Response.NoServer
            : Response.Ok;

        this.MockWrapper.Setup(w => w.Login()).Returns(loginStatus);
        this.MockWrapper.Setup(w => w.GetVoicemeeterState()).Returns((vmApp, vmState));
        this.MockWrapper.Setup(w => w.GetVoicemeeterVersion()).Returns((versionResponse, vmVersion));
        this.MockWrapper.Setup(w => w.GetApplicationState(App.MacroButtons)).Returns(buttonsState);

        var result = this.Remote.Login();

        Assert.Multiple(
            () => Assert.Equal(expectedState, result),
            () => this.MockWrapper.Verify(w => w.Login(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterState(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetVoicemeeterVersion(), Times.Once()),
            () => this.MockWrapper.Verify(w => w.GetApplicationState(App.MacroButtons), Times.Once())
        );
    }
}
