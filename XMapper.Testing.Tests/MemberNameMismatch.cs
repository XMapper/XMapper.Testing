using System;
using Xunit;

namespace XMapper.Testing.Tests;
public class MemberNameMismatch
{
    public class DummyA
    {
        public string XString1 { get; set; } = "";
        public string XStringA2 { get; set; } = "";
    }

    public class DummyB
    {
        public string XString1 { get; set; } = "";
        public string XStringB2 { get; set; } = "";
    }

    [Fact]
    public void PropertyList_Source()
    {
        var mapper = new XMapper<DummyA, DummyB>(PropertyList.Source);
        var ex = Assert.Throws<Exception>(() => mapper.IsValid(TestCases.AppDefaults));
        Assert.Contains("Property 'XStringA2' was not found on target 'DummyB'.", ex.Message);
    }

    [Fact]
    public void PropertyList_Target()
    {
        var mapper = new XMapper<DummyA, DummyB>(PropertyList.Target);
        var ex = Assert.Throws<Exception>(() => mapper.IsValid(TestCases.AppDefaults));
        Assert.Contains("Property 'XStringB2' was not found on source 'DummyA'.", ex.Message);
    }

    [Fact]
    public void IsValidSource()
    {
        Does.NotThrow(() =>
            new XMapper<DummyA, DummyB>(PropertyList.Source)
            .IgnoreSourceProperty(x => x.XStringA2)
            .IsValid(TestCases.AppDefaults));
    }

    [Fact]
    public void IsValidTarget()
    {
        Does.NotThrow(() =>
            new XMapper<DummyA, DummyB>(PropertyList.Target)
            .IgnoreTargetProperty(x => x.XStringB2)
            .IsValid(TestCases.AppDefaults));
    }
}
