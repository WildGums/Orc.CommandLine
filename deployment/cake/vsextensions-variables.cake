#l "buildserver.cake"

public class VsExtensionsContext : BuildContextWithItemsBase
{
    public VsExtensionsContext(ICakeContext cakeContext)
        : base(cakeContext)
    {
    }    

    public string PublisherName { get; set; }
    public string PersonalAccessToken { get; set; }

    protected override void ValidateContext()
    {
    
    }
    
    protected override void LogStateInfoForContext()
    {
        CakeContext.Information($"Found '{Items.Count}' vs extension projects");
    }
}

//-------------------------------------------------------------

private VsExtensionsContext InitializeVsExtensionsContext(ICakeContext cakeContext)
{
    var data = new VsExtensionsContext(cakeContext)
    {
        Items = VsExtensions ?? new List<string>(),
        PublisherName = GetBuildServerVariable("VsExtensionsPublisherName", showValue: true),
        PersonalAccessToken = GetBuildServerVariable("VsExtensionsPersonalAccessToken", showValue: false),
    };

    return data;
}

//-------------------------------------------------------------

List<string> _vsExtensions;

public List<string> VsExtensions
{
    get 
    {
        if (_vsExtensions is null)
        {
            _vsExtensions = new List<string>();
        }

        return _vsExtensions;
    }
}