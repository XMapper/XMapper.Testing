using XMapper;

namespace DummyAssembly2;

public class Class1
{
    public static XMapper<DummyA, DummyB> MapperProperty =>
        new XMapper<DummyA, DummyB>(PropertyList.Source); // should fail: DummyB.XString has no match.
}

public class DummyA
{
    public int XInt { get; set; } = 1;
    public string XString { get; set; } = "A";

}

public class DummyB
{
    public int XInt { get; set; } = 2;
}
