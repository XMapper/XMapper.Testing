using System;
using Xunit;

namespace XMapper.Testing.Tests;
public class MemberTypeMismatch
{
    public class DummyA
    {
        public string XString { get; set; } = "";
    }

    public class DummyB
    {
        public int XString { get; set; }
    }

    [Fact]
    public void NotNull()
    {
        var mapper = new XMapper<DummyA, DummyB>(PropertyList.Source);
        var ex = Assert.ThrowsAny<Exception>(() => mapper.IsValid(TestCases.NotNullDefaults));
        Assert.Contains("type 'System.String' cannot be converted to type 'System.Int32'.", ex.ToString());
    }

    public class DummyC
    {
        public int? XString { get; set; }
    }

    public class DummyD
    {
        public string? XString { get; set; }
    }

    [Fact]
    public void NullIntSource()
    {
        var mapper = new XMapper<DummyC, DummyD>(PropertyList.Source);
        var ex = Assert.ThrowsAny<Exception>(() => mapper.IsValid(TestCases.NotNullDefaults));
        Assert.Contains("type 'System.Int32' cannot be converted to type 'System.String'.", ex.ToString());
    }

    public class DummyE
    {
        public string? XString { get; set; }
    }

    public class DummyF
    {
        public int? XString { get; set; }
    }

    [Fact]
    public void NullStringSource()
    {
        var mapper = new XMapper<DummyE, DummyF>(PropertyList.Source);
        var ex = Assert.ThrowsAny<Exception>(() => mapper.IsValid(TestCases.NotNullDefaults));
        Assert.Contains("type 'System.String' cannot be converted to type 'System.Nullable`1[System.Int32]'.", ex.ToString());
    }

    public class DummyG
    {
        public int? XInt { get; set; }
    }

    public class DummyH
    {
        public int XInt { get; set; } = 2;
    }

    [Fact]
    public void StrictNullCheck()
    {
        var mapper = new XMapper<DummyG, DummyH>(PropertyList.Source);
        var ex = Assert.ThrowsAny<Exception>(() => mapper.IsValid(TestCases.NullDefaults));
        Assert.Contains("'DummyG.XInt' was null, but 'DummyH.XInt' is not nullable.", ex.ToString());
    }

    public class DummyI
    {
        public string? XString { get; set; }
    }

    public class DummyJ
    {
        public string XString { get; set; } = "";
    }

    [Fact]
    public void StrictStringNullCheck()
    {
        var mapper = new XMapper<DummyI, DummyJ>(PropertyList.Source);
        var ex = Assert.ThrowsAny<Exception>(() => mapper.IsValid(TestCases.NullDefaults));
        Assert.Contains("'DummyI.XString' was null, but 'DummyJ.XString' is not nullable.", ex.ToString());
    }

    public class DummyK
    {
        public MySharedMemberType? MySharedMemberType { get; set; }
    }
    public class DummyL
    {
        public MySharedMemberType MySharedMemberType { get; set; } = new();
    }
    public class MySharedMemberType { }

    [Fact]
    public void StrictReferenceTypeNullCheck()
    {
        var mapper = new XMapper<DummyK, DummyL>(PropertyList.Source);
        var ex = Assert.ThrowsAny<Exception>(() => mapper.IsValid(TestCases.NullDefaults));
        Assert.Contains("'DummyK.MySharedMemberType' was null, but 'DummyL.MySharedMemberType' is not nullable.", ex.ToString());
    }

    public class DummyM
    {
        public string? XString { get; set; }
    }

    public class DummyN
    {
        public string? XString { get; set; }
    }

    [Fact]
    public void ValidNullableString()
    {
        var mapper = new XMapper<DummyM, DummyN>(PropertyList.Source);
        Does.NotThrow(() => mapper.IsValid(TestCases.All));
    }
}
