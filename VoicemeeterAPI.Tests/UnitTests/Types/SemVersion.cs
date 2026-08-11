namespace PBLivingston.VoicemeeterAPI.Tests.UnitTests.Types;

public class SemVersionTests
{
    [Fact]
    public void GenericDeconstructThrowsArgumentExceptionWhenTypeNotSupported()
        => Assert.Throws<ArgumentException>(() => ((IVersion)(SemVersion)0x0002_0304).Deconstruct(out float _, out var _, out var _, out var _));

    [Fact]
    public void ParseThrowsArgumentExceptionWhenStringHasFourParts()
        => Assert.Throws<ArgumentException>(() => SemVersion.Parse("1.2.3.4"));
}
