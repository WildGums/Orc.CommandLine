#l "./buildserver.cake"

//-------------------------------------------------------------

public class UwpContext : BuildContextWithItemsBase
{
    public UwpContext(ICakeContext cakeContext)
        : base(cakeContext)
    {
    }

    public string WindowsStoreAppId { get; set; }
    public string WindowsStoreClientId { get; set; }
    public string WindowsStoreClientSecret { get; set; }
    public string WindowsStoreTenantId { get; set; }

    protected override void ValidateContext()
    {
    }
    
    protected override void LogStateInfoForContext()
    {
        CakeContext.Information($"Found '{Items.Count}' uwp projects");
    }
}

//-------------------------------------------------------------

private UwpContext InitializeUwpContext(ICakeContext cakeContext)
{
    var data = new UwpContext(cakeContext)
    {
        Items = UwpApps ?? new List<string>(),
        WindowsStoreAppId = GetBuildServerVariable("WindowsStoreAppId", showValue: true),
        WindowsStoreClientId = GetBuildServerVariable("WindowsStoreClientId", showValue: false),
        WindowsStoreClientSecret = GetBuildServerVariable("WindowsStoreClientSecret", showValue: false),
        WindowsStoreTenantId = GetBuildServerVariable("WindowsStoreTenantId", showValue: false)
    };

    return data;
}

//-------------------------------------------------------------

List<string> _uwpApps;

public List<string> UwpApps
{
    get 
    {
        if (_uwpApps is null)
        {
            _uwpApps = new List<string>();
        }

        return _uwpApps;
    }
}