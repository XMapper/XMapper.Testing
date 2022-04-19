using System;
using Xunit;
using Xunit.Abstractions;

namespace XMapper.Testing.Tests;
public class MemberNameMismatch
{
    private readonly XMapperValidator _validator;

    public MemberNameMismatch(ITestOutputHelper testOutputHelper)
    {
        _validator = new XMapperValidator(testOutputHelper.WriteLine);
    }

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
        var ex = Assert.Throws<Exception>(() => _validator.IsValid(mapper, TestCases.AppDefaults));
        Assert.Contains("Property 'XStringA2' was not found on target 'DummyB'.", ex.Message);
    }

    [Fact]
    public void PropertyList_Target()
    {
        var mapper = new XMapper<DummyA, DummyB>(PropertyList.Target);
        var ex = Assert.Throws<Exception>(() => _validator.IsValid(mapper, TestCases.AppDefaults));
        Assert.Contains("Property 'XStringB2' was not found on source 'DummyA'.", ex.Message);
    }

    [Fact]
    public void IsValidSource()
    {
        var mapper = new XMapper<DummyA, DummyB>(PropertyList.Source)
            .IgnoreSourceProperty(x => x.XStringA2);

        Does.NotThrow(() => _validator.IsValid(mapper, TestCases.AppDefaults));
    }

    [Fact]
    public void IsValidTarget()
    {
        var mapper = new XMapper<DummyA, DummyB>(PropertyList.Target)
            .IgnoreTargetProperty(x => x.XStringB2);

        Does.NotThrow(() => _validator.IsValid(mapper, TestCases.AppDefaults));
    }
}
