#l "./buildserver.cake"

//-------------------------------------------------------------

public class WebContext : BuildContextWithItemsBase
{
    protected override void ValidateContext()
    {
    }
    
    protected override void LogStateInfoForContext()
    {
        Information($"Found '{Items.Count}' web projects");
    }
}

//-------------------------------------------------------------

private WebContext InitializeWebContext(ICakeLog log)
{
    var data = new WebContext(log)
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