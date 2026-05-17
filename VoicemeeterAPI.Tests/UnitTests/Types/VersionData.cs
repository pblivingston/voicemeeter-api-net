namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types;

using System.Text.Json.Serialization;
using PBLivingston.VoicemeeterAPI.Types;

public class VersionData : TheoryData<VersionData.CaseName, VersionData.CaseRecord>
{
    public VersionData()
    {
        this.Add(CaseName.Standard, new(
            1, 2, 3, 4, Kind.Standard,
            0x0102_0304, 0x0002_0304,
            "1.2.3.4", "2.3.4"
        ));
        this.Add(CaseName.Banana, new(
            2, 3, 4, 5, Kind.Banana,
            0x0203_0405, 0x0003_0405,
            "2.3.4.5", "3.4.5"
        ));
        this.Add(CaseName.Potato, new(
            3, 4, 5, 6, Kind.Potato,
            0x0304_0506, 0x0004_0506,
            "3.4.5.6", "4.5.6"
        ));
        this.Add(CaseName.MinimumValid, new(
            1, 0, 0, 1, Kind.Standard,
            0x0100_0001, 0x0000_0001,
            "1.0.0.1", "0.0.1"
        ));
        this.Add(CaseName.MaximumValid, new(
            3, 255, 255, 255, Kind.Potato,
            0x03FF_FFFF, 0x00FF_FFFF,
            "3.255.255.255", "255.255.255"
        ));
        this.Add(CaseName.ZeroedBytes, new(
            0, 0, 0, 0, Kind.None,
            0x0000_0000, 0x0000_0000,
            "0.0.0.0", "0.0.0",
            CaseTag.InvalidParts |
            CaseTag.InvalidPacks |
            CaseTag.InvalidStrings
        ));
        this.Add(CaseName.MaxedBytes, new(
            255, 255, 255, 255, (Kind)255,
            unchecked((int)0xFFFF_FFFF), 0x00FF_FFFF,
            "255.255.255.255", "255.255.255",
            CaseTag.InvalidKind |
            CaseTag.InvalidVmPacked |
            CaseTag.InvalidVmString
        ));
        this.Add(CaseName.NegativeKind, new(
            -1, 0, 0, 1, Kind.Unknown,
            unchecked((int)0xFF00_0001), 0x0000_0001,
            "255.0.0.1", "0.0.1",
            CaseTag.InvalidKind |
            CaseTag.InvalidVmPacked |
            CaseTag.InvalidVmString |
            CaseTag.KindIn
        ));
        this.Add(CaseName.NegativeSems, new(
            2, -1, -1, -1, Kind.Banana,
            unchecked((int)0xFFFF_FFFF), unchecked((int)0xFFFF_FFFF),
            "255.255.255.255", "65535.255.255",
            CaseTag.InvalidSems |
            CaseTag.InvalidPacks |
            CaseTag.InvalidStrings |
            CaseTag.PartsIn |
            CaseTag.IncompatSemString
        ));
        this.Add(CaseName.ZeroedSems, new(
            1, 0, 0, 0, Kind.Standard,
            0x0100_0000, 0x0000_0000,
            "1.0.0.0", "0.0.0",
            CaseTag.InvalidSems |
            CaseTag.InvalidPacks |
            CaseTag.InvalidStrings
        ));
        this.Add(CaseName.ZeroedKind, new(
            0, 1, 2, 3, Kind.None,
            0x0001_0203, 0x0001_0203,
            "0.1.2.3", "1.2.3",
            CaseTag.InvalidKind |
            CaseTag.InvalidVmPacked |
            CaseTag.InvalidVmString
        ));
        this.Add(CaseName.MinorSpill, new(
            1, 0, 0, 258, Kind.Standard,
            0x0100_0102, 0x0000_0102,
            "1.0.1.2", "0.1.2",
            CaseTag.InvalidSems |
            CaseTag.SemsIn
        ));
        this.Add(CaseName.ValidKindSpill, new(
            2, 257, 1, 1, Kind.Banana,
            0x0301_0101, 0x0101_0101,
            "3.1.1.1", "257.1.1",
            CaseTag.InvalidSems |
            CaseTag.InvalidSemPacked |
            CaseTag.InvalidSemString |
            CaseTag.PartsIn |
            CaseTag.IncompatSemString
        ));
        this.Add(CaseName.InvalidKindSpill, new(
            0, 256, 0, 1, Kind.None,
            0x0100_0001, 0x0100_0001,
            "1.0.0.1", "256.0.1",
            CaseTag.InvalidParts |
            CaseTag.InvalidSemPacked |
            CaseTag.InvalidSemString |
            CaseTag.PartsIn |
            CaseTag.IncompatSemString
        ));
        this.Add(CaseName.LongStrings, new(
            1, 3, 5, 7, Kind.Standard,
            0x0103_0507, 0x0003_0507,
            "1.3.5.7.9", "3.5.7.9",
            CaseTag.InvalidStrings |
            CaseTag.FourSemString |
            CaseTag.IncompatVmString
        ));
        this.Add(CaseName.ShortStrings, new(
            2, 4, 6, 8, Kind.Banana,
            0x0204_0608, 0x0004_0608,
            "4.6.8", "4.6",
            CaseTag.InvalidStrings |
            CaseTag.ThreeString |
            CaseTag.IncompatSemString
        ));
        this.Add(CaseName.NonVersionStrings, new(
            3, 255, 255, 255, Kind.Potato,
            0x03FF_FFFF, 0x00FF_FFFF,
            "Not.A.Version.String", "NotAVersionString",
            CaseTag.InvalidStrings |
            CaseTag.IncompatStrings
        ));
        this.Add(CaseName.EmptyStrings, new(
            0, 0, 0, 0, Kind.None,
            0x0000_0000, 0x0000_0000,
            "", "",
            CaseTag.InvalidParts |
            CaseTag.InvalidPacks |
            CaseTag.InvalidStrings |
            CaseTag.IncompatStrings
        ));
        this.Add(CaseName.BadKindString, new(
            3, 9, 27, 81, Kind.Potato,
            0x0309_1B51, 0x0009_1B51,
            "Three.9.27.81", "9.27.81",
            CaseTag.InvalidVmString |
            CaseTag.IncompatVmString
        ));
        this.Add(CaseName.BadMajorString, new(
            1, 1, 2, 3, Kind.Standard,
            0x0101_0203, 0x0001_0203,
            "1.One.2.3", "One.2.3",
            CaseTag.InvalidStrings |
            CaseTag.IncompatStrings
        ));
        this.Add(CaseName.BadMinorString, new(
            2, 5, 8, 10, Kind.Banana,
            0x0205_080A, 0x0005_080A,
            "2.5.Eight.10", "5.Eight.10",
            CaseTag.InvalidStrings |
            CaseTag.IncompatStrings
        ));
        this.Add(CaseName.BadPatchString, new(
            3, 6, 9, 12, Kind.Potato,
            0x0306_090C, 0x0006_090C,
            "3.6.9.Twelve", "6.9.Twelve",
            CaseTag.InvalidStrings |
            CaseTag.IncompatStrings
        ));
    }

    public record CaseRecord(
        [property: JsonPropertyName("k")] int Kind,
        [property: JsonPropertyName("m")] int Major,
        [property: JsonPropertyName("n")] int Minor,
        [property: JsonPropertyName("p")] int Patch,
        [property: JsonPropertyName("K")] Kind K,
        [property: JsonPropertyName("pk")] int VmPacked,
        [property: JsonPropertyName("spk")] int SemPacked,
        [property: JsonPropertyName("s")] string VmString,
        [property: JsonPropertyName("ss")] string SemString,
        [property: JsonPropertyName("t")] CaseTag Tags = CaseTag.None
    ) : SerializableRecord
    {
        public CaseRecord() : this(0, 0, 0, 0, default, 0, 0, "", "") { }
        public override string ToString() => $"Tags = {this.Tags}";
    }

    public enum CaseName
    {
        Standard, Banana, Potato,
        MinimumValid, MaximumValid,
        ZeroedBytes, MaxedBytes,
        NegativeKind, NegativeSems,
        ZeroedSems, ZeroedKind, MinorSpill,
        ValidKindSpill, InvalidKindSpill,
        LongStrings, ShortStrings,
        NonVersionStrings, EmptyStrings,
        BadKindString, BadMajorString,
        BadMinorString, BadPatchString
    }

    [Flags]
    public enum CaseTag
    {
        None = 0,

        InvalidKind = 1 << 0,
        InvalidSems = 1 << 1,
        InvalidParts = InvalidKind | InvalidSems,

        InvalidVmPacked = 1 << 2,
        InvalidSemPacked = 1 << 3,
        InvalidPacks = InvalidVmPacked | InvalidSemPacked,

        InvalidVmString = 1 << 4,
        InvalidSemString = 1 << 5,
        InvalidStrings = InvalidVmString | InvalidSemString,

        KindIn = 1 << 10,
        SemsIn = 1 << 11,
        PartsIn = KindIn | SemsIn,

        ThreeString = 1 << 12,
        FourSemString = 1 << 13,
        MismatchStrings = ThreeString | FourSemString,

        IncompatVmString = 1 << 14,
        IncompatSemString = 1 << 15,
        IncompatStrings = IncompatVmString | IncompatSemString
    }
}
