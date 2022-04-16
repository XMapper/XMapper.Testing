using System;
using Xunit;

namespace XMapper.Testing.Tests;

public class DisallowedIgnoreTests
{
    public class DummyA
    {
        public string XStringA { get; set; } = "";
    }

    public class DummyB
    {
        public string XStringA { get; set; } = "";
    }

    [Fact]
    public void IgnoreSourceProperty_while_PropertyList_is_Target()
    {
        var ex = Assert.Throws<Exception>(() =>
            new XMapper<DummyA, DummyB>(PropertyList.Target)
                .IgnoreSourceProperty(x => x.XStringA)
                .IsValid(TestCases.AppDefaults));
        Assert.Contains("Use 'IgnoreTargetProperty' if PropertyList is Target.", ex.Message);
    }

    [Fact]
    public void IgnoreTargetProperty_while_PropertyList_is_Target()
    {
        var ex = Assert.Throws<Exception>(() =>
            new XMapper<DummyA, DummyB>(PropertyList.Source)
                .IgnoreTargetProperty(x => x.XStringA)
                .IsValid(TestCases.AppDefaults));
        Assert.Contains("Use 'IgnoreSourceProperty' if PropertyList is Source.", ex.Message);

    }

    [Theory]
    [InlineData(PropertyList.Target)]
    [InlineData(PropertyList.Source)]
    public void IsValid(PropertyList propertyList)
    {
        Does.NotThrow(() =>
            new XMapper<DummyA, DummyB>(propertyList)
            .IsValid(TestCases.AppDefaults));
    }
}

public static class Does
{
    public static void NotThrow(Action action)
    {
        action();
    }
}
