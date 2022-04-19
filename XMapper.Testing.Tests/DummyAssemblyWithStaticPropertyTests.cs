using System;
using Xunit;
using Xunit.Abstractions;

namespace XMapper.Testing.Tests;
public class DummyAssemblyWithStaticPropertyTests
{
    private readonly XMapperValidator _validator;

    public DummyAssemblyWithStaticPropertyTests(ITestOutputHelper testOutputHelper)
    {
        _validator = new XMapperValidator(testOutputHelper.WriteLine);
    }

    [Fact]
    public void ShouldFail()
    {
        var exception = Assert.ThrowsAny<Exception>(() =>
            _validator.AllAreValidInAssembly("DummyAssembly2", TestCases.All));
        Assert.Contains("DummyAssembly2.Class1.MapperProperty", exception.Message);
        Assert.Contains("Property 'XString' was not found on target 'DummyB'.", exception.Message);
    }
}
