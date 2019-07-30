#l "buildserver.cake"

//-------------------------------------------------------------

public class GeneralContext : BuildContextWithItemsBase
{
    public GeneralContext(ICakeContext cakeContext)
        : base(cakeContext)
    {
    }

    public string Target { get; set; }
    public string RootDirectory { get; set; }
    public string OutputRootDirectory { get; set; }

    public bool IsCiBuild { get; set; }
    public bool IsAlphaBuild { get; set; }
    public bool IsBetaBuild { get; set; }
    public bool IsOfficialBuild { get; set; }
    public bool IsLocalBuild { get; set; }
    public bool UseVisualStudioPrerelease { get; set; }
    public bool VerifyDependencies { get; set; }

    public VersionContext Version { get; set; }
    public CopyrightContext Copyright { get; set; }
    public NuGetContext NuGet { get; set; }
    public SolutionContext Solution { get; set; }
    public SourceLinkContext SourceLink { get; set; }
    public CodeSignContext CodeSign { get; set; }
    public RepositoryContext Repository { get; set; }
    public SonarQubeContext SonarQube { get; set; }

    public List<string> Includes { get; set; }
    public List<string> Excludes { get; set; }

    protected override void ValidateContext()
    {
    }
    
    protected override void LogStateInfoForContext()
    {
        Information($"Running target '{Target}'");
        Information($"Using output directory '{OutputRootDirectory}'");
    }
}

//-------------------------------------------------------------

public class VersionContext : ContextBase
{
    GitVersion _gitVersionContext;

    public VersionContext(ICakeContext cakeContext)
        : base(cakeContext)
    {
    }

    public GitVersion GitVersionContext
    {
        get
        {
            if (_gitVersionContext is null)
            {
                var gitVersionSettings = new GitVersionSettings
                {
                    UpdateAssemblyInfo = false
                };

                var gitDirectory = ".git";
                if (!DirectoryExists(gitDirectory))
                {
                    Information("No local .git directory found, treating as dynamic repository");

                    // TEMP CODE - START

                    Warning("Since dynamic repositories do not yet work correctly, we clear out the cloned temp directory (which is slow, but should be fixed in 5.0 beta)");

                    // Make a *BIG* assumption that the solution name == repository name
                    var repositoryName = SolutionName;
                    var tempDirectory = $"{System.IO.Path.GetTempPath()}\\{repositoryName}";

                    if (DirectoryExists(tempDirectory))
                    {
                        DeleteDirectory(tempDirectory, new DeleteDirectorySettings
                        {
                            Force = true,
                            Recursive = true
                        });
                    }

                    // TEMP CODE - END

                    // Dynamic repository
                    gitVersionSettings.UserName = RepositoryUsername;
                    gitVersionSettings.Password = RepositoryPassword;
                    gitVersionSettings.Url = RepositoryUrl;
                    gitVersionSettings.Branch = RepositoryBranchName;
                    gitVersionSettings.Commit = RepositoryCommitId;
                }

                _gitVersionContext = GitVersion(gitVersionSettings);
            }

            return _gitVersionContext;
        }
    }

    public string MajorMinorPatch { get; set; }
    public string FullSemVer { get; set; }
    public string NuGet { get; set; }
    public string CommitsSinceVersionSource { get; set; }

    protected override void ValidateContext()
    {
    
    }
    
    protected override void LogStateInfoForContext()
    {
    
    }
}

//-------------------------------------------------------------

public class CopyrightContext : ContextBase
{
    public CopyrightContext(ICakeContext cakeContext)
        : base(cakeContext)
    {
    }

    public string Company { get; set; }
    public string StartYear { get; set; }

    protected override void ValidateContext()
    {
        if (string.IsNullOrWhiteSpace(Company))
        {
            throw new Exception($"Company must be defined");
        }    
    }
    
    protected override void LogStateInfoForContext()
    {
    
    }
}

//-------------------------------------------------------------

public class NuGetContext : ContextBase
{
    public NuGetContext(ICakeContext cakeContext)
        : base(cakeContext)
    {
    }

    public string PackageSources { get; set; }
    public string Executable { get; set; }
    public string LocalPackagesDirectory { get; set; }

    protected override void ValidateContext()
    {
    
    }
    
    protected override void LogStateInfoForContext()
    {
    
    }
}

//-------------------------------------------------------------

public class SolutionContext : ContextBase
{
    public SolutionContext(ICakeContext cakeContext)
        : base(cakeContext)
    {
    }

    public string Name { get; set; }
    public string AssemblyInfoFileName { get; set; }
    public string FileName { get; set; }

    public string PublishType { get; set; }
    public string ConfigurationName { get; set; }

    protected override void ValidateContext()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new Exception($"SolutionName must be defined");
        }
    }
    
    protected override void LogStateInfoForContext()
    {
    
    }
}

//-------------------------------------------------------------

public class SourceLinkContext : ContextBase
{
    public SourceLinkContext(ICakeContext cakeContext)
        : base(cakeContext)
    {
    }

    public bool IsDisabled { get; set; }

    protected override void ValidateContext()
    {
    
    }
    
    protected override void LogStateInfoForContext()
    {
    
    }
}

//-------------------------------------------------------------

public class CodeSignContext : ContextBase
{
    public CodeSignContext(ICakeContext cakeContext)
        : base(cakeContext)
    {
    }

    public string WildCard { get; set; }
    public string CertificateSubjectName { get; set; }
    public string TimeStampUri { get; set; }

    protected override void ValidateContext()
    {
    
    }
    
    protected override void LogStateInfoForContext()
    {
    
    }
}

//-------------------------------------------------------------

public class RepositoryContext : ContextBase
{
    public RepositoryContext(ICakeContext cakeContext)
        : base(cakeContext)
    {
    }

    public string Url  { get; set; }
    public string BranchName  { get; set; }
    public string CommitId  { get; set; }
    public string Username  { get; set; }
    public string Password  { get; set; }

    protected override void ValidateContext()
    {
        if (string.IsNullOrWhiteSpace(Url))
        {
            throw new Exception($"RepositoryUrl must be defined");
        }
    }
    
    protected override void LogStateInfoForContext()
    {
    
    }
}

//-------------------------------------------------------------

public class SonarQubeContext : ContextBase
{
    public SonarQubeContext(ICakeContext cakeContext)
        : base(cakeContext)
    {
    }

    public bool IsDisabled { get; set; }
    public string Url { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Project { get; set; }

    protected override void ValidateContext()
    {

    }
    
    protected override void LogStateInfoForContext()
    {
    
    }
}

//-------------------------------------------------------------

private GeneralContext InitializeGeneralContext(ICakeContext cakeContext)
{
    var data = new GeneralContext(log)
    {
        Target = GetBuildServerVariable("Target", "Default", showValue: true),
    };

    data.Version = new VersionContext(log)
    {
        MajorMinorPatch = GetBuildServerVariable("GitVersion_MajorMinorPatch", "unknown", showValue: true),
        FullSemVer = GetBuildServerVariable("GitVersion_FullSemVer", "unknown", showValue: true),
        NuGet = GetBuildServerVariable("GitVersion_NuGetVersion", "unknown", showValue: true),
        CommitsSinceVersionSource = GetBuildServerVariable("GitVersion_CommitsSinceVersionSource", "unknown", showValue: true)
    };

    data.Copyright = new CopyrightContext(log)
    {
        Company = GetBuildServerVariable("Company", showValue: true),
        StartYear = GetBuildServerVariable("StartYear", showValue: true)
    };

    data.NuGet = new NuGetContext(log)
    {
        PackageSources = GetBuildServerVariable("NuGetPackageSources", showValue: true),
        Executable = "./tools/nuget.exe",
        LocalPackagesDirectory = "c:\\source\\_packages"
    };

    data.Solution = new SolutionContext(log)
    {
        Name = GetBuildServerVariable("SolutionName", showValue: true),
        AssemblyInfoFileName = "./src/SolutionAssemblyInfo.cs",
        FileName = string.Format("./src/{0}", string.Format("{0}.sln", SolutionName)),
        PublishType = GetBuildServerVariable("PublishType", "Unknown", showValue: true),
        ConfigurationName = GetBuildServerVariable("ConfigurationName", "Release", showValue: true)
    };

    data.RootDirectory = System.IO.Path.GetFullPath(".");
    data.OutputRootDirectory = System.IO.Path.GetFullPath(GetBuildServerVariable("OutputRootDirectory", string.Format("./output/{0}", data.Solution.ConfigurationName), showValue: true));
    data.IsCiBuild = GetBuildServerVariableAsBool("IsCiBuild", false, showValue: true);
    data.IsAlphaBuild = GetBuildServerVariableAsBool("IsAlphaBuild", false, showValue: true);
    data.IsBetaBuild = GetBuildServerVariableAsBool("IsBetaBuild", false, showValue: true);
    data.IsOfficialBuild = GetBuildServerVariableAsBool("IsOfficialBuild", false, showValue: true);
    data.IsLocalBuild = data.Target.ToLower().Contains("local");
    data.UseVisualStudioPrerelease = GetBuildServerVariableAsBool("UseVisualStudioPrerelease", false, showValue: true);
    data.VerifyDependencies = !GetBuildServerVariableAsBool("DependencyCheckDisabled", false, showValue: true);

    // If local, we want full pdb, so do a debug instead
    if (data.Solution.IsLocalBuild)
    {
        Warning("Enforcing configuration 'Debug' because this is seems to be a local build, do not publish this package!");
        data.Solution.ConfigurationName = "Debug";
    }

    data.SourceLink = new SourceLinkContext(log)
    {
        IsDisabled = GetBuildServerVariableAsBool("SourceLinkDisabled", false, showValue: true)
    };

    data.CodeSign = new CodeSignContext(log)
    {
        SignWildCard = GetBuildServerVariable("CodeSignWildcard", showValue: true),
        SignCertificateSubjectName = GetBuildServerVariable("CodeSignCertificateSubjectName", Company, showValue: true),
        SignTimeStampUri = GetBuildServerVariable("CodeSignTimeStampUri", "http://timestamp.comodoca.com/authenticode", showValue: true)
    };

    data.Repository = new RepositoryContext(log)
    {
        Url = GetBuildServerVariable("RepositoryUrl", showValue: true),
        BranchName = GetBuildServerVariable("RepositoryBranchName", showValue: true),
        CommitId = GetBuildServerVariable("RepositoryCommitId", showValue: true),
        Username = GetBuildServerVariable("RepositoryUsername", showValue: false),
        Password = GetBuildServerVariable("RepositoryPassword", showValue: false)
    };

    data.SonarQube = new SonarQube(log)
    {
        IsDisabled = GetBuildServerVariableAsBool("SonarDisabled", false, showValue: true),
        Url = GetBuildServerVariable("SonarUrl", showValue: true),
        Username = GetBuildServerVariable("SonarUsername", showValue: false),
        Password = GetBuildServerVariable("SonarPassword", showValue: false),
        Project = GetBuildServerVariable("SonarProject", SolutionName, showValue: true)
    };

    data.Includes = SplitCommaSeparatedList(GetBuildServerVariable("Include", string.Empty, showValue: true));
    data.Excludes = SplitCommaSeparatedList(GetBuildServerVariable("Exclude", string.Empty, showValue: true));

    // Specific overrides, done when we have *all* info
    Information("Ensuring correct runtime data based on version");

    var versionContext = data.Version;
    if (string.IsNullOrWhiteSpace(versionContext.NuGet) || versionContext.NuGet == "unknown")
    {
        Information("No version info specified, falling back to GitVersion");

        var gitVersion = versionContext.GitVersionContext;
        
        versionContext.MajorMinorPatch = gitVersion.MajorMinorPatch;
        versionContext.FullSemVer = gitVersion.FullSemVer;
        versionContext.NuGet = gitVersion.NuGetVersionV2;
        versionContext.CommitsSinceVersionSource = (gitVersion.CommitsSinceVersionSource ?? 0).ToString();
    }    

    Information("Defined version: '{0}', commits since version source: '{1}'", versionContext.FullSemVer, versionContext.CommitsSinceVersionSource);

    if (string.IsNullOrWhiteSpace(data.Repository.CommitId))
    {
        Information("No commit id specified, falling back to GitVersion");

        var gitVersion = versionContext.GitVersionContext;
        
        data.Repository.CommitId = gitVersion.Sha;
    }

    var versionToCheck = versionContext.FullSemVer;
    if (versionToCheck.Contains("alpha"))
    {
        data.IsAlphaBuild = true;
    }
    else if (versionToCheck.Contains("beta"))
    {
        data.IsBetaBuild = true;
    }
    else
    {
        data.IsOfficialBuild = true;
    }

    return data;
}

//-------------------------------------------------------------

private static string DetermineChannel(GeneralContext context)
{
    var version = context.Version.FullSemVer;

    var channel = "stable";

    if (context.IsAlphaBuild)
    {
        channel = "alpha";
    }
    else if (context.IsBetaBuild)
    {
        channel = "beta";
    }

    return channel;
}

//-------------------------------------------------------------

private static string DeterminePublishType(GeneralContext context)
{
    var publishType = "Unknown";

    if (context.IsOfficialBuild)
    {
        publishType = "Official";
    }
    else if (context.IsBetaBuild)
    {
        publishType = "Beta";
    }
    else if (context.IsAlphaBuild)
    {
        publishType = "Alpha";
    }
    
    return publishType;
}