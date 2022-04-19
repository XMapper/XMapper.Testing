using System;
using Xunit;
using Xunit.Abstractions;

namespace XMapper.Testing.Tests;
public class ReferenceTypeMemberNullCaseNotTakenIntoAccountTests
{
    private readonly XMapperValidator _validator;

    public ReferenceTypeMemberNullCaseNotTakenIntoAccountTests(ITestOutputHelper testOutputHelper)
    {
        _validator = new XMapperValidator(testOutputHelper.WriteLine);
    }
    public class DummyA
    {
        public MemberA? TheMember { get; set; }
    }

    public class DummyB
    {
        public MemberB? TheMember { get; set; } = new();
    }

    public class MemberA { }
    public class MemberB { }


    [Theory]
    [InlineData(TestCases.NullDefaults)]
    [InlineData(TestCases.TargetNullDefaults)]

    public void NonEnumerable_Invalid_1(TestCases testCases)
    {
        var mXm = new XMapper<MemberA, MemberB>(PropertyList.Target);
        var mapper = new XMapper<DummyA, DummyB>(PropertyList.Target)
            .IgnoreTargetProperty(x => x.TheMember)
            .IncludeAction((source, target) => mXm.Map(source.TheMember!, target.TheMember!));

        var exception = Assert.ThrowsAny<Exception>(() => _validator.IsValid(mapper, testCases));

        Assert.Contains("in 'XMapper<MemberA, MemberB>.Map(...)' should not be null.", exception.Message);
    }

    [Fact]
    public void NonEnumerable_Invalid_2()
    {
        var mXm = new XMapper<MemberA, MemberB>(PropertyList.Target);
        var mapper = new XMapper<DummyA, DummyB>(PropertyList.Target)
            .IgnoreTargetProperty(x => x.TheMember)
            .IncludeAction((source, target) => mXm.Map(source.TheMember!, target.TheMember ??= new()));

        var exception = Assert.ThrowsAny<Exception>(() => _validator.IsValid(mapper, TestCases.NullDefaults));

        Assert.Contains("Argument 'source' in 'XMapper<MemberA, MemberB>.Map(...)' should not be null.", exception.Message);
    }

    [Fact]
    public void NonEnumerable_Invalid_3()
    {
        var mXm = new XMapper<MemberA, MemberB>(PropertyList.Target);
        var mapper = new XMapper<DummyA, DummyB>(PropertyList.Target)
            .IgnoreTargetProperty(x => x.TheMember)
            .IncludeAction((source, target) =>
            {
                if (source.TheMember == null)
                {
                    target.TheMember = null;
                }
                else
                {
                    mXm.Map(source.TheMember, target.TheMember!);
                }
            });

        var exception = Assert.ThrowsAny<Exception>(() => _validator.IsValid(mapper, TestCases.TargetNullDefaults));

        Assert.Contains("Argument 'target' in 'XMapper<MemberA, MemberB>.Map(...)' should not be null.", exception.Message);
    }

    [Fact]
    public void NonEnumerable_Valid()
    {
        var mXm = new XMapper<MemberA, MemberB>(PropertyList.Target);
        var mapper = new XMapper<DummyA, DummyB>(PropertyList.Target)
            .IgnoreTargetProperty(x => x.TheMember)
            .IncludeAction((source, target) =>
            {
                if (source.TheMember == null)
                {
                    target.TheMember = null;
                }
                else
                {
                    mXm.Map(source.TheMember, target.TheMember ??= new());
                }
            });
        Does.NotThrow(() => _validator.IsValid(mapper, TestCases.All));
    }
}
