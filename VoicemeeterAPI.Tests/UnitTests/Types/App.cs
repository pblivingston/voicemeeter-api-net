using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types;

public class AppTests
{
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

    public record CaseRecord(App App, string S, Kind Kind, App App32, App App64, CaseTag Tags = CaseTag.None);

#pragma warning disable xUnit1047
    public static TheoryDataRow<Case, CaseRecord>[] GetCaseData =>
    [
        new(Case.Standard,     new(App.Standard, "Standard", Kind.Standard, App.Standard, App.Standardx64)),
        new(Case.Bananax64,    new(App.Bananax64, "Bananax64", Kind.Banana, App.Banana, App.Bananax64)),
        new(Case.None,         new(App.None, "None", Kind.None, App.None, App.None)),
        new(Case.Unknown,      new(App.Unknown, "Test String", Kind.Unknown, App.Unknown, App.Unknown, CaseTag.Invalid_String)),
        new(Case.MacroButtons, new(App.MacroButtons, "MacroButtons", Kind.Unknown, App.MacroButtons, App.MacroButtons))
    ];
#pragma warning restore xUnit1047

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void ToKind_ReturnsExpected_Kind(Case scenario, CaseRecord data)
    {
        _ = scenario;

        Assert.Equal(data.Kind, data.App.ToKind());
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void BitAdjust_ReturnsExpected_App(Case scenario, CaseRecord data)
    {
        _ = scenario;

        var expected = Environment.Is64BitOperatingSystem ? data.App64 : data.App32;

        Assert.Equal(expected, data.App.BitAdjust());
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
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
    [MemberData(nameof(GetCaseData))]
    public void ParseBit_ReturnsExpected_App(Case scenario, CaseRecord data)
    {
        var skipTags = CaseTag.Invalid_String;
        Assert.SkipWhen(data.Tags.HasAny(skipTags), $"Skipping case: {scenario} with any tags: {skipTags}");

        var expected = Environment.Is64BitOperatingSystem ? data.App64 : data.App32;

        Assert.Equal(expected, AppUtils.ParseBit(data.S));
    }

    [Theory]
    [MemberData(nameof(GetCaseData))]
    public void ParseBit_ThrowsException_Argument(Case scenario, CaseRecord data)
    {
        var runTags = CaseTag.Invalid_String;
        Assert.SkipUnless(data.Tags.HasAny(runTags), $"Skipping case: {scenario} without any tags: {runTags}");

        Assert.Throws<ArgumentException>(() => AppUtils.ParseBit(data.S));
    }
}