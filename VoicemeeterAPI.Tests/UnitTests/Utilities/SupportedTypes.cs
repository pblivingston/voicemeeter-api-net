namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Utilities;

using PBLivingston.VoicemeeterAPI.Types;
using PBLivingston.VoicemeeterAPI.Utilities;

public class SupportedTypesTests
{
    [Theory]
    [InlineData(new Type[] { typeof(Type), typeof(string) }, "Type, String")]
    [InlineData(new Type[] { typeof(int), typeof(VmVersion), typeof(object) }, "Int32, VmVersion, Object")]
    public void ListStringReturnsExpectedString(Type[] types, string expected)
        => Assert.Equal(expected, SupportedTypes.ListString(types));

    [Theory]
    [InlineData(typeof(VmVersion), true)]
    [InlineData(typeof(SemVersion), true)]
    [InlineData(typeof(Type), false)]
    public void IsVersionTypeReturnsExpectedBool(Type type, bool expected)
        => Assert.Equal(expected, SupportedTypes.IsVersionType(type));

    [Theory]
    [InlineData(typeof(int), true)]
    [InlineData(typeof(Kind), true)]
    [InlineData(typeof(float), false)]
    public void IsKindTypeReturnsExpectedBool(Type t, bool valid)
        => Assert.Equal(valid, SupportedTypes.IsKindType(t));

    [Theory]
    [InlineData(typeof(int), true)]
    [InlineData(typeof(App), true)]
    [InlineData(typeof(Kind), true)]
    [InlineData(typeof(string), true)]
    [InlineData(typeof(float), false)]
    public void IsRunTypeReturnsExpectedBool(Type t, bool valid)
        => Assert.Equal(valid, SupportedTypes.IsRunType(t));

    [Theory]
    [InlineData(typeof(float), true)]
    [InlineData(typeof(int), true)]
    [InlineData(typeof(string), true)]
    [InlineData(typeof(bool), true)]
    [InlineData(typeof(byte), false)]
    public void IsParamTypeReturnsExpectedBool(Type t, bool valid)
        => Assert.Equal(valid, SupportedTypes.IsParamType(t));
}
