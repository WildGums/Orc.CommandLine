#l "github-pages-variables.cake"

#addin "nuget:?package=Cake.Git&version=0.19.0"

//-------------------------------------------------------------

public class GitHubPagesProcessor : ProcessorBase
{
    public GitHubPagesProcessor(ICakeContext cakeContext)
        : base(cakeContext)
    {
        
    }    
    
    public override bool HasItems(BuildContext buildContext)
    {
        return buildContext.GitHubPages.Items.Count > 0;
    }

    private string GetGitHubPagesRepositoryUrl(BuildContext buildContext, string projectName)
    {
        // Allow per project overrides via "GitHubPagesRepositoryUrlFor[ProjectName]"
        return CakeContext.GetProjectSpecificConfigurationValue(projectName, "GitHubPagesRepositoryUrlFor", buildContext.GitHubPages.RepositoryUrl);
    }

    private string GetGitHubPagesBranchName(BuildContext buildContext, string projectName)
    {
        // Allow per project overrides via "GitHubPagesBranchNameFor[ProjectName]"
        return CakeContext.GetProjectSpecificConfigurationValue(projectName, "GitHubPagesBranchNameFor", buildContext.GitHubPages.BranchName);
    }

    private string GetGitHubPagesEmail(BuildContext buildContext, string projectName)
    {
        // Allow per project overrides via "GitHubPagesEmailFor[ProjectName]"
        return CakeContext.GetProjectSpecificConfigurationValue(projectName, "GitHubPagesEmailFor", buildContext.GitHubPages.Email);
    }

    private string GetGitHubPagesUserName(BuildContext buildContext, string projectName)
    {
        // Allow per project overrides via "GitHubPagesUserNameFor[ProjectName]"
        return CakeContext.GetProjectSpecificConfigurationValue(projectName, "GitHubPagesUserNameFor", buildContext.GitHubPages.UserName);
    }

    private string GetGitHubPagesApiToken(BuildContext buildContext, string projectName)
    {
        // Allow per project overrides via "GitHubPagesApiTokenFor[ProjectName]"
        return CakeContext.GetProjectSpecificConfigurationValue(projectName, "GitHubPagesApiTokenFor", buildContext.GitHubPages.ApiToken);
    }

    public override async Task PrepareAsync(BuildContext buildContext)
    {
        if (!HasItems(buildContext))
        {
            return;
        }

        // Check whether projects should be processed, `.ToList()` 
        // is required to prevent issues with foreach
        foreach (var gitHubPage in buildContext.GitHubPages.Items.ToList())
        {
            if (!CakeContext.ShouldProcessProject(buildContext, gitHubPage))
            {
                buildContext.GitHubPages.Items.Remove(gitHubPage);
            }
        }        
    }

    public override async Task UpdateInfoAsync(BuildContext buildContext)
    {
        if (!HasItems(buildContext))
        {
            return;
        }

        foreach (var gitHubPage in buildContext.GitHubPages.Items)
        {
            CakeContext.Information("Updating version for GitHub page '{0}'", gitHubPage);

            var projectFileName = CakeContext.GetProjectFileName(gitHubPage);

            CakeContext.TransformConfig(projectFileName, new TransformationCollection 
            {
                { "Project/PropertyGroup/PackageVersion", buildContext.General.Version.NuGet }
            });
        }        
    }

    public override async Task BuildAsync(BuildContext buildContext)
    {
        if (!HasItems(buildContext))
        {
            return;
        }

        foreach (var gitHubPage in buildContext.GitHubPages.Items)
        {
            CakeContext.LogSeparator("Building GitHub page '{0}'", gitHubPage);

            var projectFileName = CakeContext.GetProjectFileName(gitHubPage);
            
            var msBuildSettings = new MSBuildSettings {
                Verbosity = Verbosity.Quiet, // Verbosity.Diagnostic
                ToolVersion = MSBuildToolVersion.Default,
                Configuration = buildContext.General.Solution.ConfigurationName,
                MSBuildPlatform = MSBuildPlatform.x86, // Always require x86, see platform for actual target platform
                PlatformTarget = PlatformTarget.MSIL
            };

            CakeContext.ConfigureMsBuild(buildContext, msBuildSettings, gitHubPage);

            // Always disable SourceLink
            msBuildSettings.WithProperty("EnableSourceLink", "false");

            // Note: we need to set OverridableOutputPath because we need to be able to respect
            // AppendTargetFrameworkToOutputPath which isn't possible for global properties (which
            // are properties passed in using the command line)
            var outputDirectory = string.Format("{0}/{1}/", buildContext.General.OutputRootDirectory, gitHubPage);
            CakeContext.Information("Output directory: '{0}'", outputDirectory);
            msBuildSettings.WithProperty("OverridableOutputPath", outputDirectory);
            msBuildSettings.WithProperty("PackageOutputPath", buildContext.General.OutputRootDirectory);

            CakeContext.MSBuild(projectFileName, msBuildSettings);
        }        
    }

    public override async Task PackageAsync(BuildContext buildContext)
    {
        if (!HasItems(buildContext))
        {
            return;
        }

        foreach (var gitHubPage in buildContext.GitHubPages.Items)
        {
            CakeContext.LogSeparator("Packaging GitHub pages '{0}'", gitHubPage);

            var projectFileName = string.Format("./src/{0}/{0}.csproj", gitHubPage);

            var outputDirectory = string.Format("{0}/{1}/", buildContext.General.OutputRootDirectory, gitHubPage);
            CakeContext.Information("Output directory: '{0}'", outputDirectory);

            CakeContext.Information("1) Using 'dotnet publish' to package '{0}'", gitHubPage);

            var msBuildSettings = new DotNetCoreMSBuildSettings();

            // Note: we need to set OverridableOutputPath because we need to be able to respect
            // AppendTargetFrameworkToOutputPath which isn't possible for global properties (which
            // are properties passed in using the command line)
            msBuildSettings.WithProperty("OverridableOutputPath", outputDirectory);
            msBuildSettings.WithProperty("PackageOutputPath", outputDirectory);
            msBuildSettings.WithProperty("ConfigurationName", buildContext.General.Solution.ConfigurationName);
            msBuildSettings.WithProperty("PackageVersion", buildContext.General.Version.NuGet);

            var publishSettings = new DotNetCorePublishSettings
            {
                MSBuildSettings = msBuildSettings,
                OutputDirectory = outputDirectory,
                Configuration = buildContext.General.Solutions.ConfigurationName
            };

            CakeContext.DotNetCorePublish(projectFileName, publishSettings);
        }        
    }

    public override async Task DeployAsync(BuildContext buildContext)
    {
        if (!HasItems(buildContext))
        {
            return;
        }
        
        foreach (var gitHubPage in buildContext.GitHubPages.Items)
        {
            if (!CakeContext.ShouldDeployProject(buildContext, gitHubPage))
            {
                CakeContext.Information("GitHub page '{0}' should not be deployed", gitHubPage);
                continue;
            }

            CakeContext.LogSeparator("Deploying GitHub page '{0}'", gitHubPage);

            CakeContext.Warning("Only Blazor apps are supported as GitHub pages");

            var temporaryDirectory = CakeContext.GetTempDirectory("gh-pages", gitHubPage);

            CakeContext.CleanDirectory(temporaryDirectory);

            var repositoryUrl = GetGitHubPagesRepositoryUrl(buildContext, gitHubPage);
            var branchName = GetGitHubPagesBranchName(buildContext, gitHubPage);
            var email = GetGitHubPagesEmail(buildContext, gitHubPage);
            var userName = GetGitHubPagesUserName(buildContext, gitHubPage);
            var apiToken = GetGitHubPagesApiToken(buildContext, gitHubPage);

            CakeContext.Information("1) Cloning repository '{0}' using branch name '{1}'", repositoryUrl, branchName);

            CakeContext.GitClone(repositoryUrl, temporaryDirectory, userName, apiToken, new GitCloneSettings
            {
                BranchName = branchName
            });

            CakeContext.Information("2) Updating the GitHub pages branch with latest source");

            // Special directory we need to distribute (e.g. output\Release\Blazorc.PatternFly.Example\Blazorc.PatternFly.Example\dist)
            var sourceDirectory = string.Format("{0}/{1}/{1}/dist", buildContext.General.OutputRootDirectory, gitHubPage);
            var sourcePattern = string.Format("{0}/**/*", sourceDirectory);

            CakeContext.Debug("Copying all files from '{0}' => '{1}'", sourcePattern, temporaryDirectory);

            CakeContext.CopyFiles(sourcePattern, temporaryDirectory, true);

            CakeContext.Information("3) Committing latest GitHub pages");

            CakeContext.GitAddAll(temporaryDirectory);
            CakeContext.GitCommit(temporaryDirectory, "Build server", email, string.Format("Auto-update GitHub pages: '{0}'", buildContext.General.Version.NuGet));

            CakeContext.Information("4) Pushing code back to repository '{0}'", repositoryUrl);

            CakeContext.GitPush(temporaryDirectory, userName, apiToken);

            await CakeContext.NotifyAsync(buildContext, gitHubPage, string.Format("Deployed to GitHub pages"), TargetType.GitHubPages);
        }        
    }

    public override async Task FinalizeAsync(BuildContext buildContext)
    {

    }
}
