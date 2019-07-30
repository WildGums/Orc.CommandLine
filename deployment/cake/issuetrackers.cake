// Customize this file when using a different issue tracker
// #l "buildserver-github.cake"
#l "issuetrackers-jira.cake"

//-------------------------------------------------------------

public class IssueTrackersContext : ContextBase
{
    public IssueTrackersContext(ICakeContext cakeContext)
        : base(cakeContext)
    {
    }

    public JiraContext Jira { get; set; }
}

//-------------------------------------------------------------

private IssueTrackersContext InitializeIssueTrackersContext(ICakeContext cakeContext)
{
    var data = new IssueTrackersContext(cakeContext)
    {
        JiraContext = InitializeJiraContext(cakeContext)
    };

    return data;
}

//-------------------------------------------------------------

public static async Task CreateAndReleaseVersionAsync(BuildContext buildContext)
{
    LogSeparator("Creating and releasing version");

    await CreateAndReleaseVersionInJiraAsync(buildContext);

    // TODO: Add more issue tracker integrations (such as GitHub)
}
