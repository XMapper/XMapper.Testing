using XMapper;

namespace DummyAssembly3;

public class Class1
{
    public static XMapper<DummyA, DummyB> MapperField =
        new XMapper<DummyA, DummyB>(PropertyList.Source)
        .IgnoreTargetProperty(x => x.XString); // Should fail: PropertyList.Source and IgnoreTargetProperty is an invalid combination.
}

public class DummyA
{
    public int XInt { get; set; } = 1;
}

public class DummyB
{
    public int XInt { get; set; } = 2;
    public string XString { get; set; } = "B";
}
