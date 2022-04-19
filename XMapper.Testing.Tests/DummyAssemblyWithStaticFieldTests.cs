using System;
using Xunit;
using Xunit.Abstractions;

namespace XMapper.Testing.Tests;
public class DummyAssemblyWithStaticFieldTests
{
    private readonly XMapperValidator _validator;

    public DummyAssemblyWithStaticFieldTests(ITestOutputHelper testOutputHelper)
    {
        _validator = new XMapperValidator(testOutputHelper.WriteLine);
    }

    [Fact]
    public void PropertyNotFound()
    {
        var exception = Assert.ThrowsAny<Exception>(() =>
            _validator.AllAreValidInAssembly("DummyAssembly1", TestCases.AppDefaults));
        Assert.Contains("DummyAssembly1.Class1.MapperField", exception.Message);
        Assert.Contains("Property 'XString' was not found on source 'DummyA'.", exception.Message);
    }

    [Fact]
    public void MismatchingIgnoreWithPropertyList()
    {
        var exception = Assert.ThrowsAny<Exception>(() =>
            _validator.AllAreValidInAssembly("DummyAssembly3", TestCases.AppDefaults));
        Assert.Contains("DummyAssembly3.Class1.MapperField", exception.Message);
        Assert.Contains("Use 'IgnoreSourceProperty' if PropertyList is Source.", exception.Message);
    }
}
