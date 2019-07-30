#l "./buildserver.cake"

//-------------------------------------------------------------

public class WebContext : BuildContextWithItemsBase
{
    public WebContext(ICakeContext cakeContext)
        : base(cakeContext)
    {
    }

    protected override void ValidateContext()
    {
    }
    
    protected override void LogStateInfoForContext()
    {
        CakeContext.Information($"Found '{Items.Count}' web projects");
    }
}

//-------------------------------------------------------------

private WebContext InitializeWebContext(ICakeContext cakeContext)
{
    var data = new WebContext(cakeContext)
    {
        Items = WebApps ?? new List<string>()
    };

    return data;
}

//-------------------------------------------------------------

List<string> _webApps;

public List<string> WebApps
{
    get 
    {
        if (_webApps is null)
        {
            _webApps = new List<string>();
        }

        return _webApps;
    }
}