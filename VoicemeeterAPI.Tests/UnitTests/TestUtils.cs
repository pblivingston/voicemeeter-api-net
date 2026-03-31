using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit.Sdk;
using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests;

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
        if (string.IsNullOrEmpty(json)) return;
        var data = JsonSerializer.Deserialize(json, GetType(), Options);
        if (null == data) return;

        var props = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in props.Where(p => p.CanWrite))
        {
            prop.SetValue(this, prop.GetValue(data));
        }
    }
}

public abstract class MockRemote
{
    internal readonly Mock<IWrapper> MockWrapper;
    protected readonly Remote Remote;

    protected MockRemote()
    {
        MockWrapper = new Mock<IWrapper>();
        Remote = new Remote(MockWrapper.Object);
    }

    protected void MockOkLogin(int kind, int version)
    {
        MockWrapper.Setup(w => w.Login()).Returns(LoginResponse.Ok);
        MockWrapper.Setup(w => w.GetVoicemeeterType(out kind)).Returns(InfoResponse.Ok);
        MockWrapper.Setup(w => w.GetVoicemeeterVersion(out version)).Returns(InfoResponse.Ok);

        Remote.Login();
    }
}