namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types;

using System.Text.Json.Serialization;
using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.AppData;

public class AppTests
{
    [Theory]
    [ClassData(typeof(AppData))]
    public void ToKindReturnsExpectedKind(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        Assert.Equal(data.Kind, data.App.ToKind());
    }

    [Theory]
    [ClassData(typeof(AppData))]
    public void IsValidIntReturnsExpectedBool(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        Assert.Equal(data.IsValid, AppUtils.IsValid((int)data.App));
    }

    [Theory]
    [ClassData(typeof(AppData))]
    public void IsValidAppReturnsExpectedBool(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        Assert.Equal(data.IsValid, data.App.IsValid());
    }

    [Theory]
    [ClassData(typeof(AppData))]
    public void IsVoicemeeterIntReturnsExpectedBool(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        Assert.Equal(data.IsVoicemeeter, AppUtils.IsVoicemeeter((int)data.App));
    }

    [Theory]
    [ClassData(typeof(AppData))]
    public void IsVoicemeeterAppReturnsExpectedBool(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        Assert.Equal(data.IsVoicemeeter, data.App.IsVoicemeeter());
    }

    [Theory]
    [ClassData(typeof(AppData))]
    public void BitAdjustReturnsExpectedApp32Bit(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        Assert.Equal((int)data.App32, AppUtils.BitAdjust((int)data.App, false));
    }

    [Theory]
    [ClassData(typeof(AppData))]
    public void BitAdjustReturnsExpectedApp64Bit(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        Assert.Equal((int)data.App64, AppUtils.BitAdjust((int)data.App, true));
    }

    [Theory]
    [ClassData(typeof(AppData))]
    public void BitAdjustReturnsExpectedApp(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        var expected = Environment.Is64BitOperatingSystem ? data.App64 : data.App32;

        Assert.Equal(expected, data.App.BitAdjust());
    }

    [Theory]
    [ClassData(typeof(AppData))]
    public void TryParseBitReturnsExpectedApp(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        var badTags = CaseTag.InvalidString;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var expected = App.None;
        if (shouldSucceed)
        {
            expected = Environment.Is64BitOperatingSystem ? data.App64 : data.App32;
        }

        var success = AppUtils.TryParseBit(data.S, out var result);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(expected, result)
        );
    }

    [Theory]
    [ClassData(typeof(AppData))]
    public void ParseBitReturnsExpectedApp(CaseName scenario, CaseRecord data)
    {
        var skipTags = CaseTag.InvalidString;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var expected = Environment.Is64BitOperatingSystem ? data.App64 : data.App32;

        Assert.Equal(expected, AppUtils.ParseBit(data.S));
    }

    [Theory]
    [ClassData(typeof(AppData))]
    public void ParseBitThrowsExceptionCannotParseAsType(CaseName scenario, CaseRecord data)
    {
        var runTags = CaseTag.InvalidString;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<CannotParseAsTypeException>(() => AppUtils.ParseBit(data.S));

        Assert.Multiple(
            () => Assert.Equal(data.S, ex.ActualValue),
            () => Assert.Equal(typeof(App), ex.Type)
        );
    }
}

#region AppData
public class AppData : TheoryData<CaseName, CaseRecord>
{
    public AppData()
    {
        this.Add(CaseName.Standard, new(
            App.Standard, "Standard", Kind.Standard, App.Standard, App.Standardx64, true, true
        ));
        this.Add(CaseName.Bananax64, new(
            App.Bananax64, "Bananax64", Kind.Banana, App.Banana, App.Bananax64, true, true
        ));
        this.Add(CaseName.None, new(
            App.None, "None", Kind.None, App.None, App.None, false, false
        ));
        this.Add(CaseName.Unknown, new(
            App.Unknown, "Test String", Kind.Unknown, App.Unknown, App.Unknown, false, false, CaseTag.InvalidString
        ));
        this.Add(CaseName.MacroButtons, new(
            App.MacroButtons, "MacroButtons", Kind.Unknown, App.MacroButtons, App.MacroButtons, true, false
        ));
    }

    public record CaseRecord(
        [property: JsonPropertyName("a")] App App,
        [property: JsonPropertyName("s")] string S,
        [property: JsonPropertyName("k")] Kind Kind,
        [property: JsonPropertyName("32")] App App32,
        [property: JsonPropertyName("64")] App App64,
        [property: JsonPropertyName("v")] bool IsValid,
        [property: JsonPropertyName("vm")] bool IsVoicemeeter,
        [property: JsonPropertyName("t")] CaseTag Tags = CaseTag.None
    ) : SerializableRecord
    {
        public CaseRecord() : this(default, "", default, default, default, false, false) { }
        public override string ToString() => $"Tags = {this.Tags}";
    }

    public enum CaseName
    {
        Standard, Bananax64,
        None, Unknown,
        MacroButtons
    }

    [Flags]
    public enum CaseTag
    {
        None = 0,
        InvalidString = 1 << 0
    }
}
#endregion
