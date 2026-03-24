using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.KindData;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types;

public class KindTests
{
    [Theory]
    [InlineData(typeof(int), true)]
    [InlineData(typeof(Kind), true)]
    [InlineData(typeof(float), false)]
    public void IsKindType_ReturnsExpected_Bool(Type t, bool valid)
    {
        Assert.Equal(valid, KindUtils.IsKindType(t));
    }

    [Theory]
    [ClassData(typeof(KindData))]
    public void ToApp_ReturnsExpected_App(Case scenario, CaseRecord data)
    {
        _ = scenario;

        var app = Environment.Is64BitOperatingSystem ? data.App64 : data.App32;

        Assert.Equal(app, data.K.ToApp());
    }

    [Theory]
    [ClassData(typeof(KindData))]
    public void IsValid_Int_ReturnsExpected_Bool(Case scenario, CaseRecord data)
    {
        _ = scenario;

        Assert.Equal(data.Valid, KindUtils.IsValid(data.Kind));
    }

    [Theory]
    [ClassData(typeof(KindData))]
    public void IsValid_Kind_ReturnsExpected_Bool(Case scenario, CaseRecord data)
    {
        _ = scenario;

        Assert.Equal(data.Valid, data.K.IsValid());
    }
}

public class KindData : TheoryData<Case, CaseRecord>
{
    public KindData()
    {
        Add(Case.Potato, new(
            3, Kind.Potato, App.Potato, App.Potatox64, true
        ));
        Add(Case.None, new(
            0, Kind.None, App.None, App.None, false
        ));
        Add(Case.Unknown, new(
            -1, Kind.Unknown, App.Unknown, App.Unknown, false
        ));
    }

    public record CaseRecord(
        int Kind,
        Kind K,
        App App32,
        App App64,
        bool Valid,
        CaseTag Tags = CaseTag.None
    ) : SerializableRecord
    {
        public CaseRecord() : this(0, default, default, default, false) { }
        public override string ToString() => $"Tags = {Tags}";
    }

    public enum Case
    {
        Potato, None, Unknown
    }

    [Flags]
    public enum CaseTag
    {
        None = 0
    }
}