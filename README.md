# XMapper.Testing
Automate the testing of all your object-to-object mappers with one line of unit test code.
<p align="center">
    <img src="https://avatars.githubusercontent.com/u/103217522?s=150&v=4" alt="XMapper logo"/>
</p>

Available via NuGet.

For more information about creating and using a mapper, see [XMapper](https://github.com/XMapper/XMapper).

## Example

1. Define your mappers as static fields or static properties:
```csharp
using XMapper;

namespace DummyAssembly1;

public class Class1 // An example of an incomplete setup: DummyB.XStringB has no match.
{ 
    public static XMapper<DummyA, DummyB> MapperField =
        new XMapper<DummyA, DummyB>(PropertyList.Target)
        .IgnoreTargetProperty(x => x.XStringA);
}

public class DummyA
{
    public int XInt { get; set; } = 1;
}

public class DummyB
{
    public int XInt { get; set; } = 2;
    public string XStringA { get; set; } = "A";
    public string XStringB { get; set; } = "B";
}
```

2. Test all mappers via a single method call inside a unit test:
```csharp
using XMapper.Testing;
using Xunit;

public class MyXMapperTests
{
    [Fact]
    public void AllAreValid()
    {
        AssertXMapper.AllAreValidInAssembly("DummyAssembly1", TestCases.All);
    }
}

```

## All assert options
```csharp
using XMapper.Testing;


// and inside a unit test method, call one of these:

AssertXMapper.AllAreValidInAssembly("Project1", TestCases.All);

AssertXMapper.AllAreValidInAssemblies(new [] { "MyProject1", "MyProject2" }, TestCases.All);

AssertXMapper.AllAreValidInAssembly(Assembly.Load("AnotherAssembly"), TestCases.All); 

AssertXMapper.AllAreValidInAssemblies(new [] { Assembly.Load("MyAssembly1"), Assembly.Load("MyAssembly2") }, TestCases.All);


// or only validate specific mappers:

AssertXMapper.IsValid(mapper, TestCases.All);

mapper.IsValid(TestCases.All)); // extension method


// or specify only specific test cases from the TestCases flags enum:
AssertXMapper.AllAreValidInAssembly("Project1", TestCases.NullDefaults | TestCases.NotNullDefaults | TestCases.TargetReferenceTypeMembersNull);
```

Hovering over methods and `TestCases` enum values in your editor will provide guiding documentation.
