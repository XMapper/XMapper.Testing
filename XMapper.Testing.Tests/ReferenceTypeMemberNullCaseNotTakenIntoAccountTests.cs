using System;
using Xunit;

namespace XMapper.Testing.Tests;
public class ReferenceTypeMemberNullCaseNotTakenIntoAccountTests
{
    public class DummyA
    {
        public MemberA? TheMember { get; set; }
    }

    public class DummyB
    {
        public MemberB? TheMember { get; set; }
    }

    public class MemberA { }
    public class MemberB { }


    [Fact]
    public void NonEnumerable_Invalid_1()
    {
        var mXm = new XMapper<MemberA, MemberB>(PropertyList.Target);
        var mapper = new XMapper<DummyA, DummyB>(PropertyList.Target)
            .IncludeAction((source, target) => mXm.Map(source.TheMember!, target.TheMember!));

        var exception = Assert.ThrowsAny<Exception>(() => mapper.IsValid(TestCases.All));

        Assert.Contains("Argument 'target' in 'XMapper<MemberA, MemberB>.Map(...)' should not be null.", exception.Message);
    }

    [Fact]
    public void NonEnumerable_Invalid_2()
    {
        var mXm = new XMapper<MemberA, MemberB>(PropertyList.Target);
        var mapper = new XMapper<DummyA, DummyB>(PropertyList.Target)
            .IncludeAction((source, target) => mXm.Map(source.TheMember!, target.TheMember ??= new()));

        var exception = Assert.ThrowsAny<Exception>(() => mapper.IsValid(TestCases.All));

        Assert.Contains("Argument 'source' in 'XMapper<MemberA, MemberB>.Map(...)' should not be null.", exception.Message);
    }

    [Fact]
    public void NonEnumerable_Invalid_3()
    {
        var mXm = new XMapper<MemberA, MemberB>(PropertyList.Target);
        var mapper = new XMapper<DummyA, DummyB>(PropertyList.Target)
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

        var exception = Assert.ThrowsAny<Exception>(() => mapper.IsValid(TestCases.All));

        Assert.Contains("Argument 'target' in 'XMapper<MemberA, MemberB>.Map(...)' should not be null.", exception.Message);
    }

    [Fact]
    public void NonEnumerable_Valid()
    {
        var mXm = new XMapper<MemberA, MemberB>(PropertyList.Target);
        var mapper = new XMapper<DummyA, DummyB>(PropertyList.Target)
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
        Does.NotThrow(() => mapper.IsValid(TestCases.All));
    }
}
