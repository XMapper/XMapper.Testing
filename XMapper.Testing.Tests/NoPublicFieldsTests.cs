using System;
using Xunit;
using Xunit.Abstractions;

namespace XMapper.Testing.Tests;
public class NoPublicFieldsTests
{
    private readonly XMapperValidator _validator;

    public NoPublicFieldsTests(ITestOutputHelper testOutputHelper)
    {
        _validator = new XMapperValidator(testOutputHelper.WriteLine);
    }

    public class Dummy1
    {
        public string? Name;
    }
    public class Dummy2
    {
        public string? Name { get; set; }
    }


    [Fact]
    public void PublicFieldOnSourceShouldThrow()
    {
        var mapper = new XMapper<Dummy1, Dummy2>(PropertyList.Source);
        var ex = Assert.ThrowsAny<Exception>(() => _validator.IsValid(mapper, TestCases.NoPublicFields));
        Assert.Equal("'XMapper.Testing.Tests.NoPublicFieldsTests+Dummy1.Name' is a public field instead of a property. " +
            "XMapper does not map fields (but ignores them silently). " +
            "Because you use XMapper.Testing's test case 'NoPublicFields', this exception is thrown.", ex.Message);
    }

    public class Dummy3
    {
        public string? Name { get; set; }
    }
    public class Dummy4
    {
        public string? Name;
    }

    [Fact]
    public void PublicFieldOnTargetShouldThrow()
    {
        var mapper = new XMapper<Dummy3, Dummy4>(PropertyList.Target);
        var ex = Assert.ThrowsAny<Exception>(() => _validator.IsValid(mapper, TestCases.NoPublicFields));
        Assert.Equal("'XMapper.Testing.Tests.NoPublicFieldsTests+Dummy4.Name' is a public field instead of a property. " +
            "XMapper does not map fields (but ignores them silently). " +
            "Because you use XMapper.Testing's test case 'NoPublicFields', this exception is thrown.", ex.Message);
    }
}
