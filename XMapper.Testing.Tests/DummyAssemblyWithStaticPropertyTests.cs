using System;
using Xunit;

namespace XMapper.Testing.Tests;
public class DummyAssemblyWithStaticPropertyTests
{
    [Fact]
    public void ShouldFail()
    {
        var exception = Assert.ThrowsAny<Exception>(() =>
            AssertXMapper.AllAreValidInAssemblies(new[] { "DummyAssembly2" }));
        Assert.Contains("DummyAssembly2.Class1.MapperProperty", exception.Message);
        Assert.Contains("Property 'XString' was not found on target.", exception.Message);
    }
}
