#l "lib-generic.cake"
#l "lib-nuget.cake"
#l "lib-sourcelink.cake"
#l "issuetrackers.cake"
#l "notifications.cake"
#l "generic-tasks.cake"
#l "apps-uwp-tasks.cake"
#l "apps-web-tasks.cake"
#l "apps-wpf-tasks.cake"
#l "components-tasks.cake"
#l "tools-tasks.cake"
#l "docker-tasks.cake"
#l "github-pages-tasks.cake"
#l "vsextensions-tasks.cake"
#l "tests.cake"

#addin "nuget:?package=System.Net.Http&version=4.3.3"
#addin "nuget:?package=Newtonsoft.Json&version=11.0.2"
#addin "nuget:?package=Cake.Sonar&version=1.1.0"

#tool "nuget:?package=MSBuild.SonarQube.Runner.Tool&version=4.3.0"
#tool "nuget:?package=GitVersion.CommandLine&version=4.0.0-beta0012&prerelease"

//-------------------------------------------------------------

public class BuildContext : BuildContextBase
{
    public BuildContext(ICakeContext cakeContext)
        : base(cakeContext)
    {
    }

    public List<IProcessor> Processors { get; set; }
    public Dictionary<string, object> Parameters { get; set; }

    // Integrations
    public BuildServerIntegration BuildServer { get; set; }
    public IssueTrackerIntegration IssueTracker { get; set; }
    public NotificationIntegration Notification { get; set; }

    // Contexts
    public GeneralContext General { get; set; }
    public TestsContext Tests { get; set; }

    public ComponentsContext Components { get; set; }
    public DockerImagesContext DockerImages { get; set; }
    public GitHubPagesContext GitHubPages { get; set; }
    public ToolsContext Tools { get; set; }
    public UwpContext Uwp { get; set; }
    public VsExtensionsContext VsExtensions { get; set; }
    public WebContext Web { get; set; }
    public WpfContext Wpf { get; set; }

    protected override void ValidateContext()
    {
    }
    
    protected override void LogStateInfoForContext()
    {
    }
}

//-------------------------------------------------------------
// TASKS
//-------------------------------------------------------------

Setup<BuildContext>(setupContext =>
{
    var buildContext = new BuildContext(setupContext);

    // Important, set parameters first
    buildContext.Parameters = Parameters ?? new Dictionary<string, object>();

    LogSeparator("Creating integrations");

    //  Important: build server first so other integrations can read values from config
    buildContext.BuildServer = new BuildServerIntegration(buildContext);
    buildContext.IssueTracker = new IssueTrackerIntegration(buildContext);
    buildContext.Notifications = new NotificationIntegration(buildContext);

    LogSeparator("Creating build context");

    buildContext.General = InitializeGeneralContext(buildContext);
    buildContext.Tests = InitializeTestsContext(buildContext);

    buildContext.Components = InitializeComponentsContext(buildContext);
    buildContext.DockerImages = InitializeDockerImagesContext(buildContext);
    buildContext.GitHubPages = InitializeGitHubPagesContext(buildContext);
    buildContext.Tools = InitializeToolsContext(buildContext);
    buildContext.Uwp = InitializeUwpContext(buildContext);
    buildContext.VsExtensions = InitializeVsExtensionsContext(buildContext);
    buildContext.Web = InitializeWebContext(buildContext);
    buildContext.Wpf = InitializeWpfContext(buildContext);

    LogSeparator("Validating build context");

    buildContext.Validate();

    LogSeparator("Creating processors");

    buildContext.Processors.Add(new ComponentsProcessor(buildContext));
    buildContext.Processors.Add(new DockerImagesProcessor(buildContext));
    buildContext.Processors.Add(new GitHubPagesProcessor(buildContext));
    buildContext.Processors.Add(new ToolsProcessor(buildContext));
    buildContext.Processors.Add(new UwpProcessor(buildContext));
    buildContext.Processors.Add(new VsExtensionsProcessor(buildContext));
    buildContext.Processors.Add(new WebProcessor(buildContext));
    buildContext.Processors.Add(new WpfProcessor(buildContext));

    return buildContext;
});

//-------------------------------------------------------------

Task("Initialize")
    .Does<BuildContext>(async buildContext =>
{
    LogSeparator("Writing special values back to build server");

    var displayVersion = buildContext.General.Version.FullSemVer;
    if (buildContext.General.IsCiBuild)
    {
        displayVersion += " ci";
    }

    SetBuildServerVersion(displayVersion);

    var variablesToUpdate = new Dictionary<string, string>();
    variablesToUpdate["channel"] = buildContext.Wpf.Channel;
    variablesToUpdate["publishType"] = buildContext.General.Solution.PublishType.ToString();
    variablesToUpdate["isAlphaBuild"] = buildContext.General.IsAlphaBuild.ToString();
    variablesToUpdate["isBetaBuild"] = buildContext.General.IsBetaBuild.ToString();
    variablesToUpdate["isOfficialBuild"] = buildContext.General.IsOfficialBuild.ToString();

    // Also write back versioning (then it can be cached), "worst case scenario" it's writing back the same versions
    variablesToUpdate["GitVersion_MajorMinorPatch"] = buildContext.General.Version.MajorMinorPatch;
    variablesToUpdate["GitVersion_FullSemVer"] = buildContext.General.Version.FullSemVer;
    variablesToUpdate["GitVersion_NuGetVersion"] = buildContext.General.Version.NuGet;
    variablesToUpdate["GitVersion_CommitsSinceVersionSource"] = buildContext.General.Version.CommitsSinceVersionSource;

    foreach (var variableToUpdate in variablesToUpdate)
    {
        SetBuildServerVariable(variableToUpdate.Key, variableToUpdate.Value);
    }
});

//-------------------------------------------------------------

Task("Prepare")
    .Does<BuildContext>(async buildContext =>
{
    foreach (var processor in _processors)
    {
        await processor.PrepareAsync(buildContext);
    }
});

//-------------------------------------------------------------

Task("UpdateInfo")
    .IsDependentOn("Prepare")
    .Does<BuildContext>(async buildContext =>
{
    UpdateSolutionAssemblyInfo(buildContext);
    
    foreach (var processor in _processors)
    {
        await processor.UpdateInfoAsync(buildContext);
    }
});

//-------------------------------------------------------------

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("UpdateInfo")
    .IsDependentOn("VerifyDependencies")
    .IsDependentOn("CleanupCode")
    .Does<BuildContext>(async buildContext =>
{
    var sonarUrl = buildContext.General.SonarQube.Url;

    var enableSonar = !buildContext.General.SonarQube.IsDisabled && 
                      !string.IsNullOrWhiteSpace(sonarUrl);
    if (enableSonar)
    {
        SonarBegin(new SonarBeginSettings 
        {
            // SonarQube info
            Url = sonarUrl,
            Login = buildContext.General.SonarQube.Username,
            Password = buildContext.General.SonarQube.Password,

            // Project info
            Key = buildContext.General.SonarQube.Project,
            // Branch only works with the branch plugin
            //Branch = RepositoryBranchName,
            Version = buildContext.General.Version.FullSemVer,
            
            // Minimize extreme logging
            Verbose = false,
            Silent = true,
        });
    }
    else
    {
        Information("Skipping Sonar integration since url is not specified or it has been explicitly disabled");
    }

    foreach (var processor in _processors)
    {
        await processor.BuildAsync(buildContext);
    }

    if (enableSonar)
    {
        SonarEnd(new SonarEndSettings 
        {
            Login = buildContext.General.SonarQube.Username,
            Password = buildContext.General.SonarQube.Password,
        });
        
        Information("Checking whether the project passed the SonarQube gateway...");
            
        var status = "none";

        // We need to use /api/qualitygates/project_status
        var client = new System.Net.Http.HttpClient();
        using (client)
        {
            var queryUri = string.Format("{0}/api/qualitygates/project_status?projectKey={1}", sonarUrl, buildContext.General.SonarQube.Project);

            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;

            var byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", buildContext.General.SonarQube.Username, buildContext.General.SonarQube.Password));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            Debug("Invoking GET request: '{0}'", queryUri);

            var response = await client.GetAsync(new Uri(queryUri));

            Debug("Parsing request contents");

            var content = response.Content;
            var jsonContent = await content.ReadAsStringAsync();

            Debug(jsonContent);

            dynamic result = Newtonsoft.Json.Linq.JObject.Parse(jsonContent);
            status = result.projectStatus.status;
        }

        Information("SonarQube gateway status returned from request: '{0}'", status);

        if (string.IsNullOrWhiteSpace(status))
        {
            status = "none";
        }

        status = status.ToLower();

        switch (status)
        {
            case "error":
                throw new Exception(string.Format("The SonarQube gateway for '{0}' returned ERROR, please check the error(s) at {1}/dashboard?id={0}", buildContext.General.SonarQube.Project, sonarUrl));

            case "warn":
                Warning("The SonarQube gateway for '{0}' returned WARNING, please check the warning(s) at {1}/dashboard?id={0}", buildContext.General.SonarQube.Project, sonarUrl);
                break;

            case "none":
                Warning("The SonarQube gateway for '{0}' returned NONE, please check why no gateway status is available at {1}/dashboard?id={0}", buildContext.General.SonarQube.Project, sonarUrl);
                break;

            case "ok":
                Information("The SonarQube gateway for '{0}' returned OK, well done! If you want to show off the results, check out {1}/dashboard?id={0}", buildContext.General.SonarQube.Project, sonarUrl);
                break;

            default:
                throw new Exception(string.Format("Unexpected SonarQube gateway status '{0}' for project '{1}'", status, buildContext.General.SonarQube.Project));
        }
    }

    BuildTestProjects(buildContext);
});

//-------------------------------------------------------------

Task("Test")
    // Note: no dependency on 'build' since we might have already built the solution
    .Does<BuildContext>(buildContext =>
{
    foreach (var testProject in buildContext.Tests.TestProjects)
    {
        LogSeparator("Running tests for '{0}'", testProject);

        RunUnitTests(testProject);
    }
});

//-------------------------------------------------------------

Task("Package")
    // Note: no dependency on 'build' since we might have already built the solution
    // Make sure we have the temporary "project.assets.json" in case we need to package with Visual Studio
    .IsDependentOn("RestorePackages")
    // Make sure to update if we are running on a new agent so we can sign nuget packages
    .IsDependentOn("UpdateNuGet")
    .IsDependentOn("CodeSign")
    .Does<BuildContext>(async buildContext =>
{
    foreach (var processor in _processors)
    {
        await processor.PackageAsync(buildContext);
    }
});

//-------------------------------------------------------------

Task("PackageLocal")
    .IsDependentOn("Package")
    .Does<BuildContext>(buildContext =>
{
    // For now only package components, we might need to move this to components-tasks.cake in the future
    if (buildContext.Components.Items.Count == 0 && 
        buildContext.Tools.Items.Count == 0)
    {
        return;
    }

    var localPackagesDirectory = buildContext.General.NuGet.LocalPackagesDirectory;

    Information("Copying build artifacts to '{0}'", localPackagesDirectory);
    
    CreateDirectory(localPackagesDirectory);

    foreach (var component in buildContext.Components.Items)
    {
        Information("Copying build artifact for '{0}'", component);
    
        var sourceFile = string.Format("{0}/{1}.{2}.nupkg", buildContext.General.OutputRootDirectory, 
            component, buildContext.General.Version.NuGet);
        CopyFiles(new [] { sourceFile }, localPackagesDirectory);
    }
});

//-------------------------------------------------------------

Task("Deploy")
    // Note: no dependency on 'package' since we might have already packaged the solution
    // Make sure we have the temporary "project.assets.json" in case we need to package with Visual Studio
    .IsDependentOn("RestorePackages")
    .Does<BuildContext>(async buildContext =>
{
    foreach (var processor in _processors)
    {
        await processor.DeployAsync(buildContext);
    }
});

//-------------------------------------------------------------

Task("Finalize")
    // Note: no dependency on 'deploy' since we might have already deployed the solution
    .Does<BuildContext>(async buildContext =>
{
    Information("Finalizing release '{0}'", buildContext.General.Version.FullSemVer);

    foreach (var processor in _processors)
    {
        await processor.FinalizeAsync(buildContext);
    }

    await CreateAndReleaseVersionAsync();
});

//-------------------------------------------------------------
// Wrapper tasks since we don't want to add "Build" as a 
// dependency to "Package" because we want to run in multiple
// stages
//-------------------------------------------------------------

Task("BuildAndTest")
    .IsDependentOn("Initialize")
    .IsDependentOn("Build")
    .IsDependentOn("Test");

//-------------------------------------------------------------

Task("BuildAndPackage")
    .IsDependentOn("Initialize")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("Package");

//-------------------------------------------------------------

Task("BuildAndPackageLocal")
    .IsDependentOn("Initialize")
    .IsDependentOn("Build")
    //.IsDependentOn("Test") // Note: don't test for performance on local builds
    .IsDependentOn("PackageLocal");

//-------------------------------------------------------------

Task("BuildAndDeploy")
    .IsDependentOn("Initialize")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("Package")
    .IsDependentOn("Deploy");

//-------------------------------------------------------------

Task("Default")    
    .Does<BuildContext>(async buildContext =>
{
    Error("No target specified, please specify one of the following targets:\n" +
          " - Prepare\n" +
          " - UpdateInfo\n" +
          " - Build\n" + 
          " - Test\n" + 
          " - Package\n" + 
          " - Deploy\n" + 
          " - Finalize\n\n" + 
          "or one of the combined ones:\n" +
          " - BuildAndTest\n" + 
          " - BuildAndPackage\n" + 
          " - BuildAndPackageLocal\n" + 
          " - BuildAndDeploy\n");
});


//-------------------------------------------------------------
// Test wrappers
//-------------------------------------------------------------

Task("TestNotifications")    
    .Does<BuildContext>(async buildContext =>
{
    await NotifyAsync(buildContext, "MyProject", "This is a generic test");
    await NotifyAsync(buildContext, "MyProject", "This is a component test", TargetType.Component);
    await NotifyAsync(buildContext, "MyProject", "This is a docker image test", TargetType.DockerImage);
    await NotifyAsync(buildContext, "MyProject", "This is a web app test", TargetType.WebApp);
    await NotifyAsync(buildContext, "MyProject", "This is a wpf app test", TargetType.WpfApp);
    await NotifyErrorAsync(buildContext, "MyProject", "This is an error");
});

//-------------------------------------------------------------

var localTarget = GetBuildServerVariable("Target", "Default", showValue: true);
RunTarget(localTarget);