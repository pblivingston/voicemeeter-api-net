using System.Text.Json.Serialization;
using PBLivingston.VoicemeeterAPI.EventManagement.Exceptions;
using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.AppData;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types;

public class AppTests
{
    [Theory]
    [ClassData(typeof(AppData))]
    public void ToKind_ReturnsExpected_Kind(Case scenario, CaseRecord data)
    {
        _ = scenario;

        Assert.Equal(data.Kind, data.App.ToKind());
    }

    [Theory]
    [ClassData(typeof(AppData))]
    public void BitAdjust_ReturnsExpected_App32Bit(Case scenario, CaseRecord data)
    {
        _ = scenario;

        Assert.Equal((int)data.App32, AppUtils.BitAdjust((int)data.App, false));
    }

    [Theory]
    [ClassData(typeof(AppData))]
    public void BitAdjust_ReturnsExpected_App64Bit(Case scenario, CaseRecord data)
    {
        _ = scenario;

        Assert.Equal((int)data.App64, AppUtils.BitAdjust((int)data.App, true));
    }

    [Theory]
    [ClassData(typeof(AppData))]
    public void BitAdjust_ReturnsExpected_App(Case scenario, CaseRecord data)
    {
        _ = scenario;

        var expected = Environment.Is64BitOperatingSystem ? data.App64 : data.App32;

        Assert.Equal(expected, data.App.BitAdjust());
    }

    [Theory]
    [ClassData(typeof(AppData))]
    public void TryParseBit_ReturnsExpected_App(Case scenario, CaseRecord data)
    {
        _ = scenario;

        var badTags = CaseTag.Invalid_String;
        var shouldSucceed = !data.Tags.HasAny(badTags);

        var expected = App.None;
        if (shouldSucceed) { expected = Environment.Is64BitOperatingSystem ? data.App64 : data.App32; }

        var success = AppUtils.TryParseBit(data.S, out App result);

        Assert.Multiple(
            () => Assert.Equal(shouldSucceed, success),
            () => Assert.Equal(expected, result)
        );
    }

    [Theory]
    [ClassData(typeof(AppData))]
    public void ParseBit_ReturnsExpected_App(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_String;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var expected = Environment.Is64BitOperatingSystem ? data.App64 : data.App32;

        Assert.Equal(expected, AppUtils.ParseBit(data.S));
    }

    [Theory]
    [ClassData(typeof(AppData))]
    public void ParseBit_ThrowsException_CannotParseAsType(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_String;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        var ex = Assert.Throws<CannotParseAsTypeException>(() => AppUtils.ParseBit(data.S));

        Assert.Multiple(
            () => Assert.Equal(data.S, ex.ActualValue),
            () => Assert.Equal(typeof(App), ex.Type)
        );
    }
}

#region AppData
public class AppData : TheoryData<Case, CaseRecord>
{
    public AppData()
    {
        Add(Case.Standard, new(
            App.Standard, "Standard", Kind.Standard, App.Standard, App.Standardx64
        ));
        Add(Case.Bananax64, new(
            App.Bananax64, "Bananax64", Kind.Banana, App.Banana, App.Bananax64
        ));
        Add(Case.None, new(
            App.None, "None", Kind.None, App.None, App.None
        ));
        Add(Case.Unknown, new(
            App.Unknown, "Test String", Kind.Unknown, App.Unknown, App.Unknown, CaseTag.Invalid_String
        ));
        Add(Case.MacroButtons, new(
            App.MacroButtons, "MacroButtons", Kind.Unknown, App.MacroButtons, App.MacroButtons
        ));
    }

    public record CaseRecord(
        [property: JsonPropertyName("a")] App App,
        [property: JsonPropertyName("s")] string S,
        [property: JsonPropertyName("k")] Kind Kind,
        [property: JsonPropertyName("32")] App App32,
        [property: JsonPropertyName("64")] App App64,
        [property: JsonPropertyName("t")] CaseTag Tags = CaseTag.None
    ) : SerializableRecord
    {
        public CaseRecord() : this(default, "", default, default, default) { }
        public override string ToString() => $"Tags = {Tags}";
    }

    public enum Case
    {
        Standard, Bananax64,
        None, Unknown,
        MacroButtons
    }

    [Flags]
    public enum CaseTag
    {
        None = 0,
        Invalid_String = 1 << 0
    }
}
#endregion