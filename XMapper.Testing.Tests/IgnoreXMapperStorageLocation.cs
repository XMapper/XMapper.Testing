using System;
using Xunit;
using Xunit.Abstractions;

namespace XMapper.Testing.Tests;
public class IgnoreXMapperStorageLocation
{
    private readonly ITestOutputHelper _testOutputHelper;

    public IgnoreXMapperStorageLocation(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void IgnoreMappers()
    {
        var validator = new XMapperValidator(_testOutputHelper.WriteLine)
            .Ignore("DummyAssembly5.Class1._myNonStaticMapperField")
            .Ignore("DummyAssembly5.Class1._myNonStaticMapperProperty");
        Does.NotThrow(() => validator.AllAreValidInAssembly("DummyAssembly5", TestCases.All));
    }

    [Fact]
    public void NonStaticFieldThrows()
    {
        var validator = new XMapperValidator(_testOutputHelper.WriteLine)
            .Ignore("DummyAssembly5.Class1._myNonStaticMapperField");
        var ex = Assert.ThrowsAny<Exception>(() => validator.AllAreValidInAssembly("DummyAssembly5", TestCases.All));

        Assert.Contains("'DummyAssembly5.Class1._myNonStaticMapperProperty' is not static " +
            "and also not ignored via 'XMapperValidator.Ignore'. " +
            "Mark the field/property static to include the mapper in the test or explicitly ignore it.", ex.Message);
    }

    [Fact]
    public void NonStaticPropertyThrows()
    {
        var validator = new XMapperValidator(_testOutputHelper.WriteLine)
            .Ignore("DummyAssembly5.Class1._myNonStaticMapperProperty");
        var ex = Assert.ThrowsAny<Exception>(() => validator.AllAreValidInAssembly("DummyAssembly5", TestCases.All));

        Assert.Contains("'DummyAssembly5.Class1._myNonStaticMapperField' is not static " +
            "and also not ignored via 'XMapperValidator.Ignore'. " +
            "Mark the field/property static to include the mapper in the test or explicitly ignore it.", ex.Message);
    }
}
