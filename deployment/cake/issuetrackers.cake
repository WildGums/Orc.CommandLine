// Customize this file when using a different issue tracker
// #l "buildserver-github.cake"
#l "issuetrackers-jira.cake"

//-------------------------------------------------------------

public class IssueTrackersContext : BuildContextBase
{
    public IssueTrackersContext(IBuildContext parentBuildContext)
        : base(parentBuildContext)
    {
    }

    public JiraContext Jira { get; set; }

    protected override void ValidateContext()
    {
    
    }
    
    protected override void LogStateInfoForContext()
    {

    }
}

//-------------------------------------------------------------

private IssueTrackersContext InitializeIssueTrackersContext(IBuildContext parentBuildContext)
{
    var data = new IssueTrackersContext(parentBuildContext)
    {
    };

    data.Jira = InitializeJiraContext(data);

    return data;
}

//-------------------------------------------------------------

public static async Task CreateAndReleaseVersionAsync(BuildContext buildContext)
{
    LogSeparator("Creating and releasing version");

    await CreateAndReleaseVersionInJiraAsync(buildContext);

    // TODO: Add more issue tracker integrations (such as GitHub)
}
