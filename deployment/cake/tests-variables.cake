#l "buildserver.cake"

//-------------------------------------------------------------

public class TestsContext : BuildContextWithItemsBase
{
    public TestsContext(ICakeContext cakeContext)
        : base(cakeContext)
    {
    }

    public string Framework { get; set; }
    public string TargetFramework { get; set; }
    public string ProcessBit { get; set; }

    protected override void ValidateContext()
    {
        if (Items.Count == 0)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(Framework))
        {
            throw new Exception("Framework is required, specify via 'TestFramework'");
        }

        if (string.IsNullOrWhiteSpace(TargetFramework))
        {
            throw new Exception("TestFramework is required, specify via 'TestTargetFramework'");
        }

        if (string.IsNullOrWhiteSpace(ProcessBit))
        {
            throw new Exception("ProcessBit is required, specify via 'TestProcessBit'");
        }
    }

    protected override void LogStateInfoForContext()
    {
        CakeContext.Information($"Found '{Items.Count}' test projects");
    }
}

//-------------------------------------------------------------

private TestsContext InitializeTestsContext(ICakeContext cakeContext)
{
    var data = new TestsContext(cakeContext)
    {
        Items = TestProjects,

        Framework = GetBuildServerVariable("TestFramework", "nunit", showValue: true),
        TargetFramework = GetBuildServerVariable("TestTargetFramework", "net47", showValue: true),
        ProcessBit = GetBuildServerVariable("TestProcessBit", "X86", showValue: true)
    };

    return data;
}

//-------------------------------------------------------------

List<string> _testProjects;

public List<string> TestProjects
{
    get 
    {
        if (_testProjects is null)
        {
            _testProjects = new List<string>();
        }

        return _testProjects;
    }
}
