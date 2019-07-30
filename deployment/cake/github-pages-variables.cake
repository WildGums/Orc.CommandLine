#l "buildserver.cake"

//-------------------------------------------------------------

public class GitHubPagesContext : BuildContextWithItemsBase
{
    public GitHubPagesContext(IBuildContext parentBuildContext)
        : base(parentBuildContext)
    {
    }

    public string RepositoryUrl { get; set; }
    public string BranchName { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public string ApiToken { get; set; }

    protected override void ValidateContext()
    {
        if (Items.Count == 0)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(RepositoryUrl))
        {
            throw new Exception("GitHubPagesRepositoryUrl must be defined");
        }

        if (string.IsNullOrWhiteSpace(BranchName))
        {
            throw new Exception("GitHubPagesBranchName must be defined");
        }
                    
        if (string.IsNullOrWhiteSpace(Email))
        {
            throw new Exception("GitHubPagesEmail must be defined");
        }

        if (string.IsNullOrWhiteSpace(UserName))
        {
            throw new Exception("GitHubPagesUserName must be defined");
        }

        if (string.IsNullOrWhiteSpace(ApiToken))
        {
            throw new Exception("GitHubPagesApiToken must be defined");
        }        
    }
    
    protected override void LogStateInfoForContext()
    {
        CakeContext.Information($"Found '{Items.Count}' GitHub pages projects");
    }
}

//-------------------------------------------------------------

private GitHubPagesContext InitializeGitHubPagesContext(IBuildContext parentBuildContext)
{
    var data = new GitHubPagesContext(parentBuildContext)
    {
        Items = GitHubPages ?? new List<string>(),
        RepositoryUrl = GetBuildServerVariable(parentBuildContext, "GitHubPagesRepositoryUrl", ((BuildContext)parentBuildContext).General.Repository.Url, showValue: true),
        BranchName = GetBuildServerVariable(parentBuildContext, "GitHubPagesRepositoryUrl", "gh-pages", showValue: true),
        Email = GetBuildServerVariable(parentBuildContext, "GitHubPagesEmail", showValue: true),
        UserName = GetBuildServerVariable(parentBuildContext, "GitHubPagesUserName", showValue: true),
        ApiToken = GetBuildServerVariable(parentBuildContext, "GitHubPagesApiToken", showValue: false),
    };

    return data;
}

//-------------------------------------------------------------

List<string> _gitHubPages;

public List<string> GitHubPages
{
    get 
    {
        if (_gitHubPages is null)
        {
            _gitHubPages = new List<string>();
        }

        return _gitHubPages;
    }
}