#l "buildserver.cake"

//-------------------------------------------------------------

{
    public string NuGetRepositoryUrls { get; set; }
    public string NuGetRepositoryApiKeys { get; set; }

    protected override void ValidateContext()
    {

    }
    
    protected override void LogStateInfoForContext()
    {
        Information($"Found '{Items.Count}' tool projects");
    }
}

//-------------------------------------------------------------

private ToolsContext InitializeToolsContext(ICakeLog log)
{
    var data = new ToolsContext(log)
    {
        Items = Tools ?? new List<string>(),
        NuGetRepositoryUrls = GetBuildServerVariable("ToolsNuGetRepositoryUrls", showValue: true),
        NuGetRepositoryApiKeys = GetBuildServerVariable("ToolsNuGetRepositoryApiKeys", showValue: false)
    };

    return data;
}

//-------------------------------------------------------------

List<string> _tools;

public List<string> Tools
{
    get 
    {
        if (_tools is null)
        {
            _tools = new List<string>();
        }

        return _tools;
    }
}