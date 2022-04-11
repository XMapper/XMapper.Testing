using System;
using Xunit;

namespace XMapper.Testing.Tests;
public class ReferenceTypeMemberNullCaseNotTakenIntoAccountTests
{

    public class Dummy1
    {
        public Member? Member { get; set; }
    }
    public class Member
    {

    }

    [Fact]
    public void NonEnumerable()
    {
        Assert.ThrowsAny<Exception>(() =>
        {
            var mXm = new XMapper<Member, Member>(PropertyList.Target);
            var mapper = new XMapper<Dummy1, Dummy1>(PropertyList.Target)
                .IncludeAction((source, target) => mXm.Map(source.Member!, target.Member!));
            mapper.IsValid();
        });
    }

    [Fact]
    public void NonEnumerable_Valid()
    {
        var mXm = new XMapper<Member, Member>(PropertyList.Target);
        var mapper = new XMapper<Dummy1, Dummy1>(PropertyList.Target)
            .IncludeAction((source, target) =>
            {
                if (source.Member == null)
                {
                    target.Member = null;
                }
                else
                {
                    mXm.Map(source.Member!, target.Member ??= new());
                }
            });
        Does.NotThrow(mapper.IsValid);
    }
}
