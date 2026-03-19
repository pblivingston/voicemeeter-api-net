using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types;

public class KindTests
{
    public enum Case
    {
        Potato, None, Unknown
    }

    public record CaseRecord(int Kind, Kind K, App App32, App App64, bool Valid);

#pragma warning disable xUnit1047
    public static TheoryDataRow<Case, CaseRecord>[] GetCaseData =>
    [
        new(Case.Potato, new(3, Kind.Potato, App.Potato, App.Potatox64, true)),
        new(Case.None, new(0, Kind.None, App.None, App.None, false)),
        new(Case.Unknown, new(-1, Kind.Unknown, App.Unknown, App.Unknown, false))
    ];
#pragma warning restore xUnit1047

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void ToApp_ReturnsExpected_App(Case scenario, CaseRecord data)
    {
        _ = scenario;

        var app = Environment.Is64BitOperatingSystem ? data.App64 : data.App32;

        Assert.Equal(app, data.K.ToApp());
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void IsValid_Int_ReturnsExpected_Bool(Case scenario, CaseRecord data)
    {
        _ = scenario;

        Assert.Equal(data.Valid, KindUtils.IsValid(data.Kind));
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void IsValid_Kind_ReturnsExpected_Bool(Case scenario, CaseRecord data)
    {
        _ = scenario;

        Assert.Equal(data.Valid, KindUtils.IsValid(data.K));
    }

    [Fact]
    public void IsValid_ReturnsExpected_False()
    {
        float example = 123.45f;
        Assert.False(KindUtils.IsValid(example));
    }

    [Theory]
    [InlineData(typeof(int), true)]
    [InlineData(typeof(Kind), true)]
    [InlineData(typeof(float), false)]
    public void IsKindType_ReturnsExpected_Bool(Type t, bool valid)
    {
        Assert.Equal(valid, KindUtils.IsKindType(t));
    }

    [Fact]
    public void ValidateKindType_ThrowsException_NotSupported()
    {
        var example = typeof(float);

        Assert.Throws<NotSupportedException>(() => KindUtils.ValidateKindType(example));
    }
}