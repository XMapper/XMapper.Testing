using XMapper;

namespace DummyAssembly1;

public class Class1
{
    public static XMapper<DummyA, DummyB> MapperField =
        new XMapper<DummyA, DummyB>(PropertyList.Target); // Should fail: DummyB.XString has no match.
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
