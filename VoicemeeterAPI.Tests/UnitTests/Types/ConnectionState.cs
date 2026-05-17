namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types;

using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.ConnectionStateData;

public class ConnectionStateTests
{
    [Theory]
    [ClassData(typeof(ConnectionStateData))]
    public void PropertiesReturnExpectedValues(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        ConnectionStateEventArgs state = new(data.LoginStatus, data.MacroButtonsIsRunning, data.RunningKind, data.RunningVersion);

        Assert.Multiple(
            () => Assert.Equal(data.LoginStatus, state.LoginStatus),
            () => Assert.Equal(data.MacroButtonsIsRunning, state.MacroButtonsIsRunning),
            () => Assert.Equal(data.RunningKind, state.RunningKind),
            () => Assert.Equal(data.RunningVersion, state.RunningVersion)
        );
    }

    [Theory]
    [ClassData(typeof(ConnectionStateData))]
    public void LoggedInReturnsExpectedBool(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        ConnectionStateEventArgs state = new(data.LoginStatus, data.MacroButtonsIsRunning, data.RunningKind, data.RunningVersion);

        Assert.Equal(data.LoggedIn, state.LoggedIn);
    }

    [Theory]
    [ClassData(typeof(ConnectionStateData))]
    public void ConnectedReturnsExpectedBool(CaseName scenario, CaseRecord data)
    {
        _ = scenario;

        ConnectionStateEventArgs state = new(data.LoginStatus, data.MacroButtonsIsRunning, data.RunningKind, data.RunningVersion);

        Assert.Equal(data.Connected, state.Connected);
    }
}

public class ConnectionStateData : TheoryData<CaseName, CaseRecord>
{
    public ConnectionStateData()
    {
        this.Add(CaseName.Initial, new(
            LoginResponse.LoggedOut, false, Kind.None, default, false, false
        ));
        this.Add(CaseName.Connected, new(
            LoginResponse.Ok, true, Kind.Standard, (VmVersion)0x0101_0202, true, true
        ));
        this.Add(CaseName.VoicemeeterNotRunning, new(
            LoginResponse.VoicemeeterNotRunning, true, Kind.None, default, true, false
        ));
        this.Add(CaseName.LoggedOut, new(
            LoginResponse.LoggedOut, true, Kind.Banana, (VmVersion)0x0201_0202, false, false
        ));
        this.Add(CaseName.Unknown, new(
            LoginResponse.Unknown, true, Kind.Potato, (VmVersion)0x0301_0202, false, false
        ));
    }

    public record CaseRecord(
        LoginResponse LoginStatus,
        bool MacroButtonsIsRunning,
        Kind RunningKind,
        VmVersion RunningVersion,
        bool LoggedIn,
        bool Connected,
        CaseTag Tags = CaseTag.None
    ) : SerializableRecord
    {
        public CaseRecord() : this(LoginResponse.LoggedOut, false, Kind.None, default, false, false) { }
        public override string ToString() => $"Tags = {this.Tags}";
    }

    public enum CaseName
    {
        Initial, Connected, VoicemeeterNotRunning, LoggedOut, Unknown
    }

    public enum CaseTag
    {
        None = 0
    }
}
