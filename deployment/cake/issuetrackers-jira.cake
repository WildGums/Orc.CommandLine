#tool "nuget:?package=JiraCli&version=1.2.0-beta0002&prerelease"

//-------------------------------------------------------------

public class JiraContext : ContextBase
{
    public JiraContext(ICakeContext cakeContext)
        : base(cakeContext)
    {
    }

    public string Url { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string ProjectName { get; set; }
    public bool IsAvailable { get; set; }

    protected override void ValidateContext()
    {
    }
    
    protected override void LogStateInfoForContext()
    {
        if (IsAvailable)
        {
            CakeContext.Information($"Jira is available");
        }
    }
}

//-------------------------------------------------------------

private JiraContext InitializeJiraContext(ICakeContext cakeContext)
{
    var data = new JiraContext(cakeContext)
    {
        Url = GetBuildServerVariable("JiraUrl", showValue: true),
        Username = GetBuildServerVariable("JiraUsername", showValue: true),
        Password = GetBuildServerVariable("JiraPassword", showValue: false),
        ProjectName = GetBuildServerVariable("JiraProjectName", showValue: true)
    };

    if (!string.IsNullOrWhiteSpace(data.Url) &&
        !string.IsNullOrWhiteSpace(data.ProjectName))
    {
        data.IsAvailable = true;
    }
    
    return data;
}

//-------------------------------------------------------------

public async Task CreateAndReleaseVersionInJiraAsync(BuildContext buildContext)
{
    if (!buildContext.Jira.IsAvailable)
    {
        Information("JIRA is not available, skipping JIRA integration");
        return;
    }

    var version = buildContext.General.Version.FullSemVer;

    Information("Releasing version '{0}' in JIRA", version);

    // Example call:
    // JiraCli.exe -url %JiraUrl% -user %JiraUsername% -pw %JiraPassword% -action createandreleaseversion 
    // -project %JiraProjectName% -version %GitVersion_FullSemVer% -merge %IsOfficialBuild%

    var nugetPath = Context.Tools.Resolve("JiraCli.exe");
    StartProcess(nugetPath, new ProcessSettings 
    {
        Arguments = new ProcessArgumentBuilder()
            .AppendSwitch("-url", JiraUrl)
            .AppendSwitch("-user", JiraUsername)
            .AppendSwitchSecret("-pw", JiraPassword)
            .AppendSwitch("-action", "createandreleaseversion")
            .AppendSwitch("-project", JiraProjectName)
            .AppendSwitch("-version", version)
            .AppendSwitch("-merge", IsOfficialBuild.ToString())
    });

    Information("Released version in JIRA");
}