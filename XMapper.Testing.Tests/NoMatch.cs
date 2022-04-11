using System;
using Xunit;

namespace XMapper.Testing.Tests;
public class NoMatch
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
        Assert.Throws<Exception>(mapper.IsValid);
    }

    [Fact]
    public void PropertyList_Target()
    {
        var mapper = new XMapper<DummyA, DummyB>(PropertyList.Target);
        Assert.Throws<Exception>(mapper.IsValid);
    }

    [Fact]
    public void IsValidSource()
    {
        Does.NotThrow(() =>
            new XMapper<DummyA, DummyB>(PropertyList.Source)
            .IgnoreSourceProperty(x => x.XStringA2)
            .IsValid());
    }

    [Fact]
    public void IsValidTarget()
    {
        Does.NotThrow(() =>
            new XMapper<DummyA, DummyB>(PropertyList.Target)
            .IgnoreTargetProperty(x => x.XStringB2)
            .IsValid());
    }
}
