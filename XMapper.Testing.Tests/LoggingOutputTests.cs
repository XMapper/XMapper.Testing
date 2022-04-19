using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace XMapper.Testing.Tests;


public class MyXMapperTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    public MyXMapperTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void AllAreValid()
    {
        var sb = new StringBuilder();
        new XMapperValidator(output =>
        {
            _testOutputHelper.WriteLine(output);
            sb.AppendLine(output);

        }).AllAreValidInAssembly("DummyAssembly4", TestCases.All);

        Assert.Equal(
@"Start collecting XMapper instance storage locations (static fields/properties).
Found 2 XMapper instance storage locations to test.
Validating 'DummyAssembly4.Class1._myValidMapper1'.
Test case: AppDefaults
Test case: NotNullDefaults
Test case: TargetNullDefaults
Test case: NullDefaults
Validating 'DummyAssembly4.Class1._myValidMapper2'.
Test case: AppDefaults
Test case: NotNullDefaults
Test case: TargetNullDefaults
Test case: NullDefaults
Finished validating all XMapper instances.
", sb.ToString());
    }
}
