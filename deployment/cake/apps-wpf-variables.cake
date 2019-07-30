#l "buildserver.cake"

//-------------------------------------------------------------

public class WpfContext : BuildContextWithItemsBase
{
    public WpfContext(ICakeContext cakeContext)
        : base(cakeContext)
    {
    }


    public string DeploymentsShare { get; set; }
    public string Channel { get; set; }
    public bool UpdateDeploymentsShare { get; set; }
    public string AzureDeploymentsStorageConnectionString { get; set; }

    protected override void ValidateContext()
    {

    }
    
    protected override void LogStateInfoForContext()
    {
        CakeContext.Information($"Found '{Items.Count}' wpf projects");
    }
}

//-------------------------------------------------------------

private WpfContext InitializeWpfContext(ICakeContext cakeContext)
{
    var data = new WpfContext(cakeContext)
    {
        Items = WpfApps ?? new List<string>(),
        DeploymentsShare = GetBuildServerVariable("DeploymentsShare", showValue: true),
        Channel = GetBuildServerVariable("Channel", showValue: true),
        UpdateDeploymentsShare = GetBuildServerVariableAsBool("UpdateDeploymentsShare", true, showValue: true),
        AzureDeploymentsStorageConnectionString = GetBuildServerVariable("AzureDeploymentsStorageConnectionString")
    };

    return data;
}

//-------------------------------------------------------------

List<string> _wpfApps;

public List<string> WpfApps
{
    get 
    {
        if (_wpfApps is null)
        {
            _wpfApps = new List<string>();
        }

        return _wpfApps;
    }
}