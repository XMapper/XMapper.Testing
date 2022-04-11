﻿using System;
using Xunit;

namespace XMapper.Testing.Tests;
public class DummyAssemblyWithStaticFieldTests
{
    [Fact]
    public void PropertyNotFound()
    {
        var exception = Assert.ThrowsAny<Exception>(() =>
            AssertXMapper.AllAreValidInAssembly("DummyAssembly1"));
        Assert.Contains("DummyAssembly1.Class1.MapperField", exception.Message);
        Assert.Contains("Property 'XString' was not found on source.", exception.Message);
    }

    [Fact]
    public void MismatchingIgnoreWithPropertyList()
    {
        var exception = Assert.ThrowsAny<Exception>(() =>
            AssertXMapper.AllAreValidInAssembly("DummyAssembly3"));
        Assert.Contains("DummyAssembly3.Class1.MapperField", exception.Message);
        Assert.Contains("Use IgnoreSourceProperty if PropertyList is Source.", exception.Message);
    }
}