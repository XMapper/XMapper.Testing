# XMapper.Testing
Automate the testing of all your object-to-object mappers with one line of unit test code.
<p align="center">
    <img src="https://avatars.githubusercontent.com/u/103217522?s=150&v=4" alt="XMapper logo"/>
</p>

Available via [NuGet](https://www.nuget.org/packages/XMapper.Testing).

For more information about creating and using a mapper, see [XMapper](https://github.com/XMapper/XMapper).

## Example

1. Assign your mappers to static fields or static properties:
```csharp
using XMapper;

namespace DummyAssembly1;

public class Class1
{ 
    private static readonly XMapper<DummyA, DummyB> MapperField =
        new XMapper<DummyA, DummyB>(PropertyList.Target)
        .IgnoreTargetProperty(x => x.Id);
}
```

2. Test all mappers via a single method call inside a unit test:
```csharp
using XMapper.Testing;
using Xunit;
using Xunit.Abstractions;

public class XMapperTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    public XMapperTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void AllAreValid()
    {
        new XMapperValidator(_testOutputHelper.WriteLine).AllAreValidInAssembly("DummyAssembly1", TestCases.All);
    }
}
```
In case the unit test passes, the `Standard Output` in `Visual Studio`'s `Test Explorer` will show something like:

    Start collecting XMapper instance storage locations (static fields/properties).
    Found 2 XMapper instance storage locations to test.
    Validating 'DummyAssembly4.Class1.MyValidMapper1'.
    Test case: AppDefaults
    Test case: NotNullDefaults
    Test case: TargetNullDefaults
    Test case: NullDefaults
    Validating 'DummyAssembly4.Class1.MyValidMapper2'.
    Test case: AppDefaults
    Test case: NotNullDefaults
    Test case: TargetNullDefaults
    Test case: NullDefaults
    Finished validating all XMapper instances.

## All assert options
```csharp

AllAreValidInAssembly("Project1", TestCases.All);

AllAreValidInAssemblies(new [] { "MyProject1", "MyProject2" }, TestCases.All);

AllAreValidInAssembly(Assembly.Load("AnotherAssembly"), TestCases.All); 

AllAreValidInAssemblies(new [] { Assembly.Load("MyAssembly1"), Assembly.Load("MyAssembly2") }, TestCases.All);


// or only validate specific mappers:
IsValid(mapper, TestCases.All);


// or specify only specific test cases from the TestCases flags enum:
AllAreValidInAssembly("Project1", TestCases.NotNullDefaults | TestCases.TargetNullDefaults);
```

Hovering over methods and `TestCases` enum values in your editor will provide guiding documentation.
