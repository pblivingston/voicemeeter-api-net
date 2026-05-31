namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types;

using System.Text.Json.Serialization;
using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.ConnectionStateData;

public class ConnectionStateTests
{
    [Theory]
    [ClassData(typeof(ConnectionStateData))]
    public void ContsructorReturnsExpectedParts(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        ConnectionState state = new(data.LoginStatus, data.ButtonsState, data.RunningKind, data.RunningVersion);

        Assert.Multiple(
            () => Assert.Equal(data.HashCode, state.HashCode),
            () => Assert.Equal(data.LoginStatus, state.LoginStatus),
            () => Assert.Equal(data.ButtonsState, state.ButtonsState),
            () => Assert.Equal(data.RunningKind, state.RunningKind),
            () => Assert.Equal(data.RunningVersion, state.RunningVersion)
        );
    }

    [Theory]
    [ClassData(typeof(ConnectionStateData))]
    public void LoggedInReturnsExpectedBool(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        ConnectionState state = new(data.LoginStatus, data.ButtonsState, data.RunningKind, data.RunningVersion);

        Assert.Equal(data.LoggedIn, state.LoggedIn);
    }

    [Theory]
    [ClassData(typeof(ConnectionStateData))]
    public void ConnectedReturnsExpectedBool(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        ConnectionState state = new(data.LoginStatus, data.ButtonsState, data.RunningKind, data.RunningVersion);

        Assert.Equal(data.Connected, state.Connected);
    }

    [Theory]
    [ClassData(typeof(ConnectionStateData))]
    public void ButtonsRunningReturnsExpectedBool(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        ConnectionState state = new(data.LoginStatus, data.ButtonsState, data.RunningKind, data.RunningVersion);

        Assert.Equal(data.ButtonsRunning, state.ButtonsRunning);
    }

    [Theory]
    [ClassData(typeof(ConnectionStateData))]
    public void DeconstructorReturnsExpectedParts(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        ConnectionState state = new(data.LoginStatus, data.ButtonsState, data.RunningKind, data.RunningVersion);
        state.Deconstruct(out var login, out var buttons, out var kind, out var version);

        Assert.Multiple(
            () => Assert.Equal(data.LoginStatus, login),
            () => Assert.Equal(data.ButtonsState, buttons),
            () => Assert.Equal(data.RunningKind, kind),
            () => Assert.Equal(data.RunningVersion, version)
        );
    }

    [Theory]
    [ClassData(typeof(ConnectionStateData))]
    public void CastToTupleReturnsExpectedParts(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        ConnectionState state = new(data.LoginStatus, data.ButtonsState, data.RunningKind, data.RunningVersion);

        var (login, buttons, kind, version) = ((LoginResponse, RunResponse, Kind, VmVersion))state;

        Assert.Multiple(
            () => Assert.Equal(data.LoginStatus, login),
            () => Assert.Equal(data.ButtonsState, buttons),
            () => Assert.Equal(data.RunningKind, kind),
            () => Assert.Equal(data.RunningVersion, version)
        );
    }

    [Theory]
    [ClassData(typeof(ConnectionStateData))]
    public void CastFromTupleReturnsExpectedHashCode(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        var state = (ConnectionState)(data.LoginStatus, data.ButtonsState, data.RunningKind, data.RunningVersion);

        Assert.Equal(data.HashCode, state.HashCode);
    }

    [Theory]
    [ClassData(typeof(ConnectionStateData))]
    public void EqualsStateReturnsExpectedTrue(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        ConnectionState stateA = new(data.LoginStatus, data.ButtonsState, data.RunningKind, data.RunningVersion);
        var stateB = (ConnectionState)(data.LoginStatus, data.ButtonsState, data.RunningKind, data.RunningVersion);

        Assert.True(stateA.Equals(stateB));
    }

    [Theory]
    [ClassData(typeof(ConnectionStateData))]
    public void EqualsStateReturnsExpectedFalse(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        var testVersion = (VmVersion)0x0106_0102;
        if (data.RunningVersion == testVersion)
        {
            testVersion = (VmVersion)0x0109_0901;
        }

        ConnectionState stateA = new(data.LoginStatus, data.ButtonsState, data.RunningKind, data.RunningVersion);
        ConnectionState stateB = new(LoginResponse.Unknown, RunResponse.NotResponding, Kind.Standard, testVersion);

        Assert.False(stateA.Equals(stateB));
    }

    [Theory]
    [ClassData(typeof(ConnectionStateData))]
    public void EqualsObjectReturnsExpectedTrue(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        ConnectionState stateA = new(data.LoginStatus, data.ButtonsState, data.RunningKind, data.RunningVersion);
        var stateB = (object)new ConnectionState(data.LoginStatus, data.ButtonsState, data.RunningKind, data.RunningVersion);

        Assert.True(stateA.Equals(stateB));
    }

    [Theory]
    [ClassData(typeof(ConnectionStateData))]
    public void EqualsObjectReturnsExpectedFalseVal(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        var testVersion = (VmVersion)0x0106_0102;
        if (data.RunningVersion == testVersion)
        {
            testVersion = (VmVersion)0x0109_0901;
        }

        ConnectionState stateA = new(data.LoginStatus, data.ButtonsState, data.RunningKind, data.RunningVersion);
        var stateB = (object)new ConnectionState(LoginResponse.Unknown, RunResponse.NotResponding, Kind.Standard, testVersion);

        Assert.False(stateA.Equals(stateB));
    }

    [Theory]
    [ClassData(typeof(ConnectionStateData))]
    public void EqualsObjectReturnsExpectedFalseObj(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        ConnectionState state = new(data.LoginStatus, data.ButtonsState, data.RunningKind, data.RunningVersion);

        Assert.False(state.Equals(data.RunningVersion));
    }

    [Theory]
    [ClassData(typeof(ConnectionStateData))]
    public void GetHashCodeReturnsExpectedHashCode(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        ConnectionState state = new(data.LoginStatus, data.ButtonsState, data.RunningKind, data.RunningVersion);

        Assert.Equal(data.HashCode, state.GetHashCode());
    }

    [Theory]
    [ClassData(typeof(ConnectionStateData))]
    public void EqualToReturnsExpectedBool(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        var expected = data.HashCode == 0x0501_0202;

        ConnectionState stateA = new(data.LoginStatus, data.ButtonsState, data.RunningKind, data.RunningVersion);
        ConnectionState stateB = new(LoginResponse.Ok, RunResponse.Ok, Kind.Standard, (VmVersion)0x0101_0202);

        Assert.Equal(expected, stateA == stateB);
    }

    [Theory]
    [ClassData(typeof(ConnectionStateData))]
    public void NotEqualToReturnsExpectedBool(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        var expected = data.HashCode != 0x0501_0202;

        ConnectionState stateA = new(data.LoginStatus, data.ButtonsState, data.RunningKind, data.RunningVersion);
        ConnectionState stateB = new(LoginResponse.Ok, RunResponse.Ok, Kind.Standard, (VmVersion)0x0101_0202);

        Assert.Equal(expected, stateA != stateB);
    }
}

#region ConnectionStateData
public class ConnectionStateData : TheoryData<CaseName, CaseRecord>
{
    public ConnectionStateData()
    {
        this.Add(CaseName.Initial, new(
            unchecked((int)0xA000_0000), LoginResponse.LoggedOut, RunResponse.NotRunning, Kind.None, default, false, false, false
        ));
        this.Add(CaseName.Connected, new(
            0x0501_0202, LoginResponse.Ok, RunResponse.Ok, Kind.Standard, (VmVersion)0x0101_0202, true, true, true
        ));
        this.Add(CaseName.NothingRunning, new(
            0x6000_0000, LoginResponse.VoicemeeterNotRunning, RunResponse.NotRunning, Kind.None, default, true, false, false
        ));
        this.Add(CaseName.VoicemeeterNotRunning, new(
            0x4000_0000, LoginResponse.VoicemeeterNotRunning, RunResponse.Ok, Kind.None, default, true, false, true
        ));
        this.Add(CaseName.ButtonsNotRunning, new(
            0x2501_0202, LoginResponse.Ok, RunResponse.NotRunning, Kind.Standard, (VmVersion)0x0101_0202, true, true, false
        ));
        this.Add(CaseName.ButtonsNotResponding, new(
            0x3A01_0202, LoginResponse.Ok, RunResponse.NotResponding, Kind.Banana, (VmVersion)0x0201_0202, true, true, false
        ));
        this.Add(CaseName.ButtonsHidden, new(
            0x1F01_0202, LoginResponse.Ok, RunResponse.Hidden, Kind.Potato, (VmVersion)0x0301_0202, true, true, true
        ));
        this.Add(CaseName.LoggedOut, new(
            unchecked((int)0x8A01_0202), LoginResponse.LoggedOut, RunResponse.Ok, Kind.Banana, (VmVersion)0x0201_0202, false, false, true
        ));
        this.Add(CaseName.Unknown, new(
            unchecked((int)0xCF01_0202), LoginResponse.Unknown, RunResponse.Ok, Kind.Potato, (VmVersion)0x0301_0202, false, false, true
        ));
    }

    public record CaseRecord(
        [property: JsonPropertyName("hc")] int HashCode,
        [property: JsonPropertyName("ls")] LoginResponse LoginStatus,
        [property: JsonPropertyName("bs")] RunResponse ButtonsState,
        [property: JsonPropertyName("rk")] Kind RunningKind,
        [property: JsonPropertyName("rv")] VmVersion RunningVersion,
        [property: JsonPropertyName("li")] bool LoggedIn,
        [property: JsonPropertyName("c")] bool Connected,
        [property: JsonPropertyName("br")] bool ButtonsRunning,
        [property: JsonPropertyName("t")] CaseTag Tags = CaseTag.None
    ) : SerializableRecord
    {
        public CaseRecord() : this(0xA000_000, LoginResponse.LoggedOut, RunResponse.NotRunning, Kind.None, default, false, false, false) { }
        public override string ToString() => $"Tags = {this.Tags}";
    }

    public enum CaseName
    {
        Initial,
        Connected,
        NothingRunning,
        VoicemeeterNotRunning,
        ButtonsNotRunning,
        ButtonsNotResponding,
        ButtonsHidden,
        LoggedOut,
        Unknown
    }

    public enum CaseTag
    {
        None = 0
    }
}
#endregion
