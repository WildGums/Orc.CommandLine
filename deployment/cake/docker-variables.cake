#l "buildserver.cake"

//-------------------------------------------------------------

public class DockerImagesContext : BuildContextWithItemsBase
{
    public DockerImagesContext(IBuildContext parentBuildContext)
        : base(parentBuildContext)
    {
    }

    public string DockerEngineUrl { get; set; }
    public string DockerRegistryUrl { get; set; }
    public string DockerRegistryUserName { get; set; }
    public string DockerRegistryPassword { get; set; }

    protected override void ValidateContext()
    {
    }
    
    protected override void LogStateInfoForContext()
    {
        CakeContext.Information($"Found '{Items.Count}' docker image projects");
    }
}

//-------------------------------------------------------------

private DockerImagesContext InitializeDockerImagesContext(IBuildContext parentBuildContext)
{
    var data = new DockerImagesContext(parentBuildContext)
    {
        Items = DockerImages ?? new List<string>(),
        DockerEngineUrl = GetBuildServerVariable(parentBuildContext, "DockerEngineUrl", showValue: true),
        DockerRegistryUrl = GetBuildServerVariable(parentBuildContext, "DockerRegistryUrl", showValue: true),
        DockerRegistryUserName = GetBuildServerVariable(parentBuildContext, "DockerRegistryUserName", showValue: false),
        DockerRegistryPassword = GetBuildServerVariable(parentBuildContext, "DockerRegistryPassword", showValue: false)
    };

    return data;
}

//-------------------------------------------------------------

List<string> _dockerImages;

public List<string> DockerImages
{
    get
    {
        if (_dockerImages is null)
        {
            _dockerImages = new List<string>();
        }

        return _dockerImages;
    }
}