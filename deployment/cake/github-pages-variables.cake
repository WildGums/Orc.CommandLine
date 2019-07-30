#l "buildserver.cake"

//-------------------------------------------------------------

public class GitHubPagesContext : BuildContextWithItemsBase
{
    public GitHubPagesContext(ICakeContext cakeContext)
        : base(cakeContext)
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

private GitHubPagesContext InitializeGitHubPagesContext(ICakeContext cakeContext)
{
    var data = new GitHubPagesContext(cakeContext)
    {
        Items = GitHubPages ?? new List<string>(),
        RepositoryUrl = GetBuildServerVariable("GitHubPagesRepositoryUrl", RepositoryUrl, showValue: true),
        BranchName = GetBuildServerVariable("GitHubPagesRepositoryUrl", "gh-pages", showValue: true),
        Email = GetBuildServerVariable("GitHubPagesEmail", showValue: true),
        UserName = GetBuildServerVariable("GitHubPagesUserName", showValue: true),
        ApiToken = GetBuildServerVariable("GitHubPagesApiToken", showValue: false),
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