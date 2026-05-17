namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types;

using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.KindData;

public class KindTests
{
    [Theory]
    [ClassData(typeof(KindData))]
    public void ToAppReturnsExpectedApp(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        var app = Environment.Is64BitOperatingSystem ? data.App64 : data.App32;

        Assert.Equal(app, data.K.ToApp());
    }

    [Theory]
    [ClassData(typeof(KindData))]
    public void IsValidIntReturnsExpectedBool(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        Assert.Equal(data.Valid, KindUtils.IsValid(data.Kind));
    }

    [Theory]
    [ClassData(typeof(KindData))]
    public void IsValidKindReturnsExpectedBool(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        Assert.Equal(data.Valid, data.K.IsValid());
    }
}

public class KindData : TheoryData<CaseName, CaseRecord>
{
    public KindData()
    {
        this.Add(CaseName.Potato, new(
            3, Kind.Potato, App.Potato, App.Potatox64, true
        ));
        this.Add(CaseName.None, new(
            0, Kind.None, App.None, App.None, false
        ));
        this.Add(CaseName.Unknown, new(
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
        public override string ToString() => $"Tags = {this.Tags}";
    }

    public enum CaseName
    {
        Potato, None, Unknown
    }

    [Flags]
    public enum CaseTag
    {
        None = 0
    }
}
