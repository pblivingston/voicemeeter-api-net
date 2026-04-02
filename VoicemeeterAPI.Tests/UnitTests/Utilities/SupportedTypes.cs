using PBLivingston.VoicemeeterAPI.Types;
using PBLivingston.VoicemeeterAPI.Utilities;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Utilities;

public class SupportedTypesTests
{
    [Theory]
    [InlineData(new Type[] { typeof(Type), typeof(string) }, "Type, String")]
    [InlineData(new Type[] { typeof(int), typeof(VmVersion), typeof(object) }, "Int32, VmVersion, Object")]
    public void ListString_ReturnsExpected_String(Type[] types, string expected)
    {
        Assert.Equal(expected, SupportedTypes.ListString(types));
    }

    [Theory]
    [InlineData(typeof(VmVersion), true)]
    [InlineData(typeof(SemVersion), true)]
    [InlineData(typeof(Type), false)]
    public void IsVersionType_ReturnsExpected_Bool(Type type, bool expected)
    {
        Assert.Equal(expected, SupportedTypes.IsVersionType(type));
    }

    [Theory]
    [InlineData(typeof(int), true)]
    [InlineData(typeof(Kind), true)]
    [InlineData(typeof(float), false)]
    public void IsKindType_ReturnsExpected_Bool(Type t, bool valid)
    {
        Assert.Equal(valid, SupportedTypes.IsKindType(t));
    }

    [Theory]
    [InlineData(typeof(int), true)]
    [InlineData(typeof(App), true)]
    [InlineData(typeof(Kind), true)]
    [InlineData(typeof(string), true)]
    [InlineData(typeof(float), false)]
    public void IsRunType_ReturnsExpected_Bool(Type t, bool valid)
    {
        Assert.Equal(valid, SupportedTypes.IsRunType(t));
    }

    [Theory]
    [InlineData(typeof(float), true)]
    [InlineData(typeof(int), true)]
    [InlineData(typeof(string), true)]
    [InlineData(typeof(bool), true)]
    [InlineData(typeof(byte), false)]
    public void IsParamType_ReturnsExpected_Bool(Type t, bool valid)
    {
        Assert.Equal(valid, SupportedTypes.IsParamType(t));
    }
}