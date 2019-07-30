#l "buildserver.cake"

//-------------------------------------------------------------

public class ComponentsContext : BuildContextWithItemsBase
{
    public ComponentsContext(IBuildContext parentBuildContext)
        : base(parentBuildContext)
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

private ComponentsContext InitializeComponentsContext(IBuildContext parentBuildContext)
{
    var data = new ComponentsContext(parentBuildContext)
    {
        Items = Components ?? new List<string>(),
        NuGetRepositoryUrl = GetBuildServerVariable(parentBuildContext, "NuGetRepositoryUrl", showValue: true),
        NuGetRepositoryApiKey = GetBuildServerVariable(parentBuildContext, "NuGetRepositoryApiKey", showValue: false)
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