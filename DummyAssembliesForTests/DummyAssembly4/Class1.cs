using XMapper;

namespace DummyAssembly4;

public class Class1
{
    private static readonly XMapper<Dummy1, Dummy2> _myValidMapper1 = new XMapper<Dummy1, Dummy2>(PropertyList.Source);
    private static readonly XMapper<Dummy2, Dummy1> _myValidMapper2 = new XMapper<Dummy2, Dummy1>(PropertyList.Target);


    public class Dummy1
    {
        public string Name { get; set; } = "";
        public string[]? StringArray { get; set; }
    }

    public class Dummy2
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string[]? StringArray { get; set; }
    }
}
