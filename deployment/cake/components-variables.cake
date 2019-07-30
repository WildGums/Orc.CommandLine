#l "buildserver.cake"

//-------------------------------------------------------------

public class ComponentsContext : BuildContextWithItemsBase
{
    public ComponentsContext(ICakeContext cakeContext)
        : base(cakeContext)
    {
    }

    public string NuGetRepositoryUrl { get; set; }
    public string NuGetRepositoryApiKey { get; set; }

    protected override void ValidateContext()
    {

    }
    
    protected override void LogStateInfoForContext()
    {
        CakeContext.Information($"Found '{Items.Count}' component projects");
    }
}

//-------------------------------------------------------------

private ComponentsContext InitializeComponentsContext(ICakeContext cakeContext)
{
    var data = new ComponentsContext(cakeContext)
    {
        Items = Components ?? new List<string>(),
        NuGetRepositoryUrl = GetBuildServerVariable("NuGetRepositoryUrl", showValue: true),
        NuGetRepositoryApiKey = GetBuildServerVariable("NuGetRepositoryApiKey", showValue: false)
    };

    return data;
}

//-------------------------------------------------------------

List<string> _components;

public List<string> Components
{
    get 
    {
        if (_components is null)
        {
            _components = new List<string>();
        }

        return _components;
    }
}