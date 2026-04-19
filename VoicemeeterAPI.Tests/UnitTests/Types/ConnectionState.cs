using PBLivingston.VoicemeeterAPI.Types;
using static PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types.ConnectionStateData;

namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types;

public class ConnectionStateTests
{
    [Theory]
    [ClassData(typeof(ConnectionStateData))]
    public void Properties_ReturnExpected_Values(Case scenario, CaseRecord data)
    {
        _ = scenario;

        ConnectionStateEventArgs state = new(data.LoginStatus, data.RunningKind, data.RunningVersion);

        Assert.Multiple(
            () => Assert.Equal(data.LoginStatus, state.LoginStatus),
            () => Assert.Equal(data.RunningKind, state.RunningKind),
            () => Assert.Equal(data.RunningVersion, state.RunningVersion)
        );
    }

    [Theory]
    [ClassData(typeof(ConnectionStateData))]
    public void LoggedIn_ReturnsExpected_Bool(Case scenario, CaseRecord data)
    {
        _ = scenario;

        ConnectionStateEventArgs state = new(data.LoginStatus, data.RunningKind, data.RunningVersion);

        Assert.Equal(data.LoggedIn, state.LoggedIn);
    }

    [Theory]
    [ClassData(typeof(ConnectionStateData))]
    public void Connected_ReturnsExpected_Bool(Case scenario, CaseRecord data)
    {
        _ = scenario;

        ConnectionStateEventArgs state = new(data.LoginStatus, data.RunningKind, data.RunningVersion);

        Assert.Equal(data.Connected, state.Connected);
    }
}

public class ConnectionStateData : TheoryData<Case, CaseRecord>
{
    public ConnectionStateData()
    {
        Add(Case.Initial, new(
            LoginResponse.LoggedOut, Kind.None, default, false, false
        ));
        Add(Case.Connected, new(
            LoginResponse.Ok, Kind.Standard, (VmVersion)0x0101_0202, true, true
        ));
        Add(Case.VoicemeeterNotRunning, new(
            LoginResponse.VoicemeeterNotRunning, Kind.None, default, true, false
        ));
        Add(Case.LoggedOut, new(
            LoginResponse.LoggedOut, Kind.Banana, (VmVersion)0x0201_0202, false, false
        ));
        Add(Case.Unknown, new(
            LoginResponse.Unknown, Kind.Potato, (VmVersion)0x0301_0202, false, false
        ));
    }

    public record CaseRecord(
        LoginResponse LoginStatus,
        Kind RunningKind,
        VmVersion RunningVersion,
        bool LoggedIn,
        bool Connected,
        CaseTag Tags = CaseTag.None
    ) : SerializableRecord
    {
        public CaseRecord() : this(LoginResponse.LoggedOut, Kind.None, default, false, false) { }
        public override string ToString() => $"Tags = {Tags}";
    }

    public enum Case
    {
        Initial, Connected, VoicemeeterNotRunning, LoggedOut, Unknown
    }

    public enum CaseTag
    {
        None = 0
    }
}