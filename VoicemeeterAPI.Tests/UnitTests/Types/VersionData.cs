using System.Text.Json.Serialization;
using PBLivingston.VoicemeeterAPI.Types;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types;

public class VersionData : TheoryData<VersionData.Case, VersionData.CaseRecord>
{
    public VersionData()
    {
        Add(Case.Standard, new(
            1, 2, 3, 4, Kind.Standard,
            0x0102_0304, 0x0002_0304,
            "1.2.3.4", "2.3.4"
        ));
        Add(Case.Banana, new(
            2, 3, 4, 5, Kind.Banana,
            0x0203_0405, 0x0003_0405,
            "2.3.4.5", "3.4.5"
        ));
        Add(Case.Potato, new(
            3, 4, 5, 6, Kind.Potato,
            0x0304_0506, 0x0004_0506,
            "3.4.5.6", "4.5.6"
        ));
        Add(Case.MinimumValid, new(
            1, 0, 0, 1, Kind.Standard,
            0x0100_0001, 0x0000_0001,
            "1.0.0.1", "0.0.1"
        ));
        Add(Case.MaximumValid, new(
            3, 255, 255, 255, Kind.Potato,
            0x03FF_FFFF, 0x00FF_FFFF,
            "3.255.255.255", "255.255.255"
        ));
        Add(Case.ZeroedBytes, new(
            0, 0, 0, 0, Kind.None,
            0x0000_0000, 0x0000_0000,
            "0.0.0.0", "0.0.0",
            CaseTag.Invalid_Parts |
            CaseTag.Invalid_Packs |
            CaseTag.Invalid_Strings
        ));
        Add(Case.MaxedBytes, new(
            255, 255, 255, 255, (Kind)255,
            unchecked((int)0xFFFF_FFFF), 0x00FF_FFFF,
            "255.255.255.255", "255.255.255",
            CaseTag.Invalid_Kind |
            CaseTag.Invalid_Packed |
            CaseTag.Invalid_String
        ));
        Add(Case.NegativeKind, new(
            -1, 0, 0, 1, Kind.Unknown,
            unchecked((int)0xFF00_0001), 0x0000_0001,
            "255.0.0.1", "0.0.1",
            CaseTag.Invalid_Kind |
            CaseTag.Invalid_Packed |
            CaseTag.Invalid_String |
            CaseTag.KindIn
        ));
        Add(Case.NegativeSems, new(
            2, -1, -1, -1, Kind.Banana,
            unchecked((int)0xFFFF_FFFF), unchecked((int)0xFFFF_FFFF),
            "255.255.255.255", "65535.255.255",
            CaseTag.Invalid_Sems |
            CaseTag.Invalid_Packs |
            CaseTag.Invalid_Strings |
            CaseTag.PartsIn
        ));
        Add(Case.ValidKindSpill, new(
            2, 257, 1, 1, Kind.Banana,
            0x0301_0101, 0x0101_0101,
            "3.1.1.1", "257.1.1",
            CaseTag.Invalid_Sems |
            CaseTag.Invalid_SemPacked |
            CaseTag.Invalid_SemString |
            CaseTag.PartsIn
        ));
        Add(Case.InvalidKindSpill, new(
            0, 256, 0, 1, Kind.None,
            0x0100_0001, 0x0100_0001,
            "1.0.0.1", "256.0.1",
            CaseTag.Invalid_Parts |
            CaseTag.Invalid_SemPacked |
            CaseTag.Invalid_SemString |
            CaseTag.PartsIn
        ));
        Add(Case.BadStrings, new(
            1, 3, 5, 7, Kind.Standard,
            0x0103_0507, 0x0003_0507,
            "1.3.5.7.9", "Invalid",
            CaseTag.Invalid_Strings
        ));
        Add(Case.ZeroedSems, new(
            1, 0, 0, 0, Kind.Standard,
            0x0100_0000, 0x0000_0000,
            "1.0.0.0", "0.0.0",
            CaseTag.Invalid_Sems |
            CaseTag.Invalid_Packs |
            CaseTag.Invalid_Strings
        ));
        Add(Case.MinorSpill, new(
            1, 0, 0, 258, Kind.Standard,
            0x0100_0102, 0x0000_0102,
            "1.0.1.2", "0.1.2",
            CaseTag.Invalid_Sems |
            CaseTag.SemsIn
        ));
    }

    public record CaseRecord(
        [property: JsonPropertyName("k")] int Kind,
        [property: JsonPropertyName("m")] int Major,
        [property: JsonPropertyName("n")] int Minor,
        [property: JsonPropertyName("p")] int Patch,
        [property: JsonPropertyName("K")] Kind K,
        [property: JsonPropertyName("pk")] int Packed,
        [property: JsonPropertyName("spk")] int SemPacked,
        [property: JsonPropertyName("s")] string String,
        [property: JsonPropertyName("ss")] string SemString,
        [property: JsonPropertyName("t")] CaseTag Tags = CaseTag.None
    ) : SerializableRecord
    {
        public CaseRecord() : this(0, 0, 0, 0, default, 0, 0, "", "") { }
        public override string ToString() => $"Tags = {Tags}";
    }

    public enum Case
    {
        Standard, Banana, Potato,
        MinimumValid, MaximumValid,
        ZeroedBytes, MaxedBytes,
        NegativeKind, NegativeSems,
        ValidKindSpill, InvalidKindSpill,
        BadStrings, ZeroedSems, MinorSpill
    }

    [Flags]
    public enum CaseTag
    {
        None = 0,
        Invalid_Kind = 1 << 0,
        Invalid_Sems = 1 << 1,
        Invalid_Packed = 1 << 2,
        Invalid_SemPacked = 1 << 3,
        Invalid_String = 1 << 4,
        Invalid_SemString = 1 << 5,
        KindIn = 1 << 10,
        SemsIn = 1 << 11,
        Invalid_Parts = Invalid_Kind | Invalid_Sems,
        Invalid_Packs = Invalid_Packed | Invalid_SemPacked,
        Invalid_Strings = Invalid_String | Invalid_SemString,
        PartsIn = KindIn | SemsIn
    }
}