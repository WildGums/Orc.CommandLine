#tool "nuget:?package=JiraCli&version=1.2.0-beta0002&prerelease"

//-------------------------------------------------------------

public class JiraContext : BuildContextBase
{
    public JiraContext(IBuildContext parentBuildContext)
        : base(parentBuildContext)
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

private JiraContext InitializeJiraContext(IBuildContext parentBuildContext)
{
    var data = new JiraContext(parentBuildContext)
    {
        Url = GetBuildServerVariable(parentBuildContext, "JiraUrl", showValue: true),
        Username = GetBuildServerVariable(parentBuildContext, "JiraUsername", showValue: true),
        Password = GetBuildServerVariable(parentBuildContext, "JiraPassword", showValue: false),
        ProjectName = GetBuildServerVariable(parentBuildContext, "JiraProjectName", showValue: true)
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
        buildContext.CakeContext.Information("JIRA is not available, skipping JIRA integration");
        return;
    }

    var version = buildContext.General.Version.FullSemVer;

    buildContext.CakeContext.Information("Releasing version '{0}' in JIRA", version);

    // Example call:
    // JiraCli.exe -url %JiraUrl% -user %JiraUsername% -pw %JiraPassword% -action createandreleaseversion 
    // -project %JiraProjectName% -version %GitVersion_FullSemVer% -merge %IsOfficialBuild%

    var nugetPath = Context.Tools.Resolve("JiraCli.exe");
    buildContext.CakeContext.StartProcess(nugetPath, new ProcessSettings 
    {
        Arguments = new ProcessArgumentBuilder()
            .AppendSwitch("-url", buildContext.IssueTrackers.Jira.Url)
            .AppendSwitch("-user", buildContext.IssueTrackers.Jira.Username)
            .AppendSwitchSecret("-pw", buildContext.IssueTrackers.Jira.Password)
            .AppendSwitch("-action", "createandreleaseversion")
            .AppendSwitch("-project", buildContext.IssueTrackers.Jira.ProjectName)
            .AppendSwitch("-version", version)
            .AppendSwitch("-merge", buildContext.General.IsOfficialBuild.ToString())
    });

    buildContext.CakeContext.Information("Released version in JIRA");
}