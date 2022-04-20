using XMapper;

namespace DummyAssembly5;

public class Class1
{
    private static readonly XMapper<Dummy1, Dummy2> _myStaticMapper = new XMapper<Dummy1, Dummy2>(PropertyList.Source);

    private readonly XMapper<Dummy1, Dummy2> _myNonStaticMapperField = new XMapper<Dummy1, Dummy2>(PropertyList.Source);
    private XMapper<Dummy2, Dummy1> _myNonStaticMapperProperty => new XMapper<Dummy2, Dummy1>(PropertyList.Target);

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
