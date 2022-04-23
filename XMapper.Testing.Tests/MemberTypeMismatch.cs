using System;
using Xunit;
using Xunit.Abstractions;

namespace XMapper.Testing.Tests;
public class MemberTypeMismatch
{
    private readonly XMapperValidator _validator;

    public MemberTypeMismatch(ITestOutputHelper testOutputHelper)
    {
        _validator = new XMapperValidator(testOutputHelper.WriteLine);
    }
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
        var ex = Assert.ThrowsAny<Exception>(() => _validator.IsValid(mapper, TestCases.NotNullDefaults));
        Assert.Equal("'DummyA.XString' is of type 'System.String', but 'DummyB.XString' is of type 'System.Int32'.", ex.Message);
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
        var ex = Assert.ThrowsAny<Exception>(() => _validator.IsValid(mapper, TestCases.NotNullDefaults));
        Assert.Equal("'DummyC.XString' is of type 'System.Int32', but 'DummyD.XString' is of type 'System.String'.", ex.Message);
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
        var ex = Assert.ThrowsAny<Exception>(() => _validator.IsValid(mapper, TestCases.NotNullDefaults));
        Assert.Equal("'DummyE.XString' is of type 'System.String', but 'DummyF.XString' is of type 'System.Int32'.", ex.Message);
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
        var ex = Assert.ThrowsAny<Exception>(() => _validator.IsValid(mapper, TestCases.NullDefaults));
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
        var ex = Assert.ThrowsAny<Exception>(() => _validator.IsValid(mapper, TestCases.NullDefaults));
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
        var ex = Assert.ThrowsAny<Exception>(() => _validator.IsValid(mapper, TestCases.NullDefaults));
        Assert.Contains("'DummyK.MySharedMemberType' was null, but 'DummyL.MySharedMemberType' is not nullable.", ex.ToString());
    }

    [Fact]
    public void ValidNonNullableReferenceTypeMapping()
    {
        var mapper = new XMapper<DummyL, DummyL>(PropertyList.Source);
        Does.NotThrow(() => _validator.IsValid(mapper, TestCases.All));
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

        Does.NotThrow(() => _validator.IsValid(mapper, TestCases.All));
    }
}
