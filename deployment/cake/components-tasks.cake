#l "components-variables.cake"

#addin "nuget:?package=Cake.FileHelpers&version=3.0.0"

using System.Xml.Linq;

//-------------------------------------------------------------

public class ComponentsProcessor : ProcessorBase
{
    public ComponentsProcessor(ICakeContext cakeContext)
        : base(cakeContext)
    {
        
    }

    public override bool HasItems(BuildContext buildContext)
    {
        return buildContext.Components.Items.Count > 0;
    }

    private string GetComponentNuGetRepositoryUrl(BuildContext buildContext, string projectName)
    {
        // Allow per project overrides via "NuGetRepositoryUrlFor[ProjectName]"
        return GetProjectSpecificConfigurationValue(projectName, "NuGetRepositoryUrlFor", buildContext.Components.NuGetRepositoryUrl);
    }

    private string GetComponentNuGetRepositoryApiKey(BuildContext buildContext, string projectName)
    {
        // Allow per project overrides via "NuGetRepositoryApiKeyFor[ProjectName]"
        return GetProjectSpecificConfigurationValue(projectName, "NuGetRepositoryApiKeyFor", buildContext.Components.NuGetRepositoryApiKey);
    }

    public override async Task PrepareAsync(BuildContext buildContext)
    {
        if (!HasItems(buildContext))
        {
            return;
        }

        // Check whether projects should be processed, `.ToList()` 
        // is required to prevent issues with foreach
        foreach (var component in buildContext.Items.ToList())
        {
            if (!ShouldProcessProject(buildContext, component))
            {
                buildContext.Components.Items.Remove(component);
            }
        }

        if (buildContext.General.IsLocalBuild && buildContext.General.Target.ToLower().Contains("packagelocal"))
        {
            foreach (var component in buildContext.Components.Items)
            {
                var cacheDirectory = Environment.ExpandEnvironmentVariables(string.Format("%userprofile%/.nuget/packages/{0}/{1}", component, buildContext.General.Version.NuGet));

                Information("Checking for existing local NuGet cached version at '{0}'", cacheDirectory);

                var retryCount = 3;

                while (retryCount > 0)
                {
                    if (!DirectoryExists(cacheDirectory))
                    {
                        break;
                    }

                    Information("Deleting already existing NuGet cached version from '{0}'", cacheDirectory);
                    
                    DeleteDirectory(cacheDirectory, new DeleteDirectorySettings
                    {
                        Force = true,
                        Recursive = true
                    });

                    await System.Threading.Tasks.Task.Delay(1000);

                    retryCount--;
                }            
            }
        }        
    }

    public override async Task UpdateInfoAsync(BuildContext buildContext)
    {
        if (!HasItems(buildContext))
        {
            return;
        }

        foreach (var component in buildContext.Components.Items)
        {
            Information("Updating version for component '{0}'", component);

            var projectFileName = GetProjectFileName(component);

            TransformConfig(projectFileName, new TransformationCollection 
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
        
        foreach (var component in buildContext.Components.Items)
        {
            LogSeparator("Building component '{0}'", component);

            var projectFileName = GetProjectFileName(component);
            
            var msBuildSettings = new MSBuildSettings 
            {
                Verbosity = Verbosity.Quiet,
                //Verbosity = Verbosity.Diagnostic,
                ToolVersion = MSBuildToolVersion.Default,
                Configuration = buildContext.General.Solution.ConfigurationName,
                MSBuildPlatform = MSBuildPlatform.x86, // Always require x86, see platform for actual target platform
                PlatformTarget = PlatformTarget.MSIL
            };

            ConfigureMsBuild(msBuildSettings, component);
            
            // Note: we need to set OverridableOutputPath because we need to be able to respect
            // AppendTargetFrameworkToOutputPath which isn't possible for global properties (which
            // are properties passed in using the command line)
            var outputDirectory = GetProjectOutputDirectory(buildContext, component);
            Information("Output directory: '{0}'", outputDirectory);
            msBuildSettings.WithProperty("OverridableOutputPath", outputDirectory);
            msBuildSettings.WithProperty("PackageOutputPath", buildContext.General.OutputRootDirectory);

            // SourceLink specific stuff
            var repositoryUrl = buildContext.General.Repository.Url;
            var repositoryCommitId = buildContext.General.Repository.CommitId;
            if (!buildContext.General.SourceLink.IsDisabled && 
                !buildContext.General.IsLocalBuild && 
                !string.IsNullOrWhiteSpace(repositoryUrl))
            {       
                Information("Repository url is specified, enabling SourceLink to commit '{0}/commit/{1}'", 
                    repositoryUrl, repositoryCommitId);

                // TODO: For now we are assuming everything is git, we might need to change that in the future
                // See why we set the values at https://github.com/dotnet/sourcelink/issues/159#issuecomment-427639278
                msBuildSettings.WithProperty("EnableSourceLink", "true");
                msBuildSettings.WithProperty("EnableSourceControlManagerQueries", "false");
                msBuildSettings.WithProperty("PublishRepositoryUrl", "true");
                msBuildSettings.WithProperty("RepositoryType", "git");
                msBuildSettings.WithProperty("RepositoryUrl", repositoryUrl);
                msBuildSettings.WithProperty("RevisionId", repositoryCommitId);

                InjectSourceLinkInProjectFile(projectFileName);
            }

            MSBuild(projectFileName, msBuildSettings);
        }        
    }

    public override async Task PackageAsync(BuildContext buildContext)
    {
        if (!HasItems(buildContext))
        {
            return;
        }

        var configurationName = buildContext.General.Solution.ConfigurationName;

        foreach (var component in buildContext.Components.Items)
        {
            LogSeparator("Packaging component '{0}'", component);

            var projectDirectory = string.Format("./src/{0}", component);
            var projectFileName = string.Format("{0}/{1}.csproj", projectDirectory, component);
            var outputDirectory = GetProjectOutputDirectory(buildContext, component);
            Information("Output directory: '{0}'", outputDirectory);

            // Step 1: remove intermediate files to ensure we have the same results on the build server, somehow NuGet 
            // targets tries to find the resource assemblies in [ProjectName]\obj\Release\net46\de\[ProjectName].resources.dll',
            // we won't run a clean on the project since it will clean out the actual output (which we still need for packaging)

            Information("Cleaning intermediate files for component '{0}'", component);

            var binFolderPattern = string.Format("{0}/bin/{1}/**.dll", projectDirectory, configurationName);

            Information("Deleting 'bin' directory contents using '{0}'", binFolderPattern);

            var binFiles = GetFiles(binFolderPattern);
            DeleteFiles(binFiles);

            var objFolderPattern = string.Format("{0}/obj/{1}/**.dll", projectDirectory, configurationName);

            Information("Deleting 'bin' directory contents using '{0}'", objFolderPattern);

            var objFiles = GetFiles(objFolderPattern);
            DeleteFiles(objFiles);

            Information(string.Empty);

            // Step 2: Go packaging!
            Information("Using 'msbuild' to package '{0}'", component);

            var msBuildSettings = new MSBuildSettings {
                Verbosity = Verbosity.Quiet,
                //Verbosity = Verbosity.Diagnostic,
                ToolVersion = MSBuildToolVersion.Default,
                Configuration = configurationName,
                MSBuildPlatform = MSBuildPlatform.x86, // Always require x86, see platform for actual target platform
                PlatformTarget = PlatformTarget.MSIL
            };

            ConfigureMsBuild(msBuildSettings, component, "pack");

            // Note: we need to set OverridableOutputPath because we need to be able to respect
            // AppendTargetFrameworkToOutputPath which isn't possible for global properties (which
            // are properties passed in using the command line)
            msBuildSettings.WithProperty("OverridableOutputPath", outputDirectory);
            msBuildSettings.WithProperty("PackageOutputPath", buildContext.General.OutputRootDirectory);
            msBuildSettings.WithProperty("ConfigurationName", configurationName);
            msBuildSettings.WithProperty("PackageVersion", buildContext.General.Version.NuGet);

            // SourceLink specific stuff
            var repositoryUrl = buildContext.General.Repository.Url;
            var repositoryCommitId = buildContext.General.Repository.CommitId;
            if (!buildContext.General.SourceLink.IsDisabled && 
                !buildContext.General.IsLocalBuild && 
                !string.IsNullOrWhiteSpace(repositoryUrl))
            {       
                Information("Repository url is specified, adding commit specific data to package");

                // TODO: For now we are assuming everything is git, we might need to change that in the future
                // See why we set the values at https://github.com/dotnet/sourcelink/issues/159#issuecomment-427639278
                msBuildSettings.WithProperty("PublishRepositoryUrl", "true");
                msBuildSettings.WithProperty("RepositoryType", "git");
                msBuildSettings.WithProperty("RepositoryUrl", repositoryUrl);
                msBuildSettings.WithProperty("RevisionId", repositoryCommitId);
            }
            
            // Fix for .NET Core 3.0, see https://github.com/dotnet/core-sdk/issues/192, it
            // uses obj/release instead of [outputdirectory]
            msBuildSettings.WithProperty("DotNetPackIntermediateOutputPath", outputDirectory);
            
            msBuildSettings.WithProperty("NoBuild", "true");
            msBuildSettings.Targets.Add("Pack");

            MSBuild(projectFileName, msBuildSettings);

            LogSeparator();
        }

        var codeSign = (!buildContext.General.IsCiBuild && 
                        !buildContext.General.IsLocalBuild && 
                        !string.IsNullOrWhiteSpace(buildContext.General.CodeSign.CertificateSubjectName));
        if (codeSign)
        {
            // For details, see https://docs.microsoft.com/en-us/nuget/create-packages/sign-a-package
            // nuget sign MyPackage.nupkg -CertificateSubjectName <MyCertSubjectName> -Timestamper <TimestampServiceURL>
            var filesToSign = GetFiles(string.Format("{0}/*.nupkg", buildContext.General.OutputRootDirectory));

            foreach (var fileToSign in filesToSign)
            {
                Information("Signing NuGet package '{0}' using certificate subject '{1}'", fileToSign, buildContext.General.CodeSign.CertificateSubjectName);

                var exitCode = StartProcess(NuGetExe, new ProcessSettings
                {
                    Arguments = string.Format("sign \"{0}\" -CertificateSubjectName \"{1}\" -Timestamper \"{2}\"", 
                        fileToSign, buildContext.General.CodeSign.CertificateSubjectName, buildContext.General.CodeSign.TimeStampUri)
                });

                Information("Signing NuGet package exited with '{0}'", exitCode);
            }
        }        
    }

    public override async Task DeployAsync(BuildContext buildContext)
    {
        if (!HasItems(buildContext))
        {
            return;
        }

        foreach (var component in buildContext.Components.Items)
        {
            if (!ShouldDeployProject(component))
            {
                Information("Component '{0}' should not be deployed", component);
                continue;
            }

            LogSeparator("Deploying component '{0}'", component);

            var packageToPush = string.Format("{0}/{1}.{2}.nupkg", buildContext.General.OutputRootDirectory, component, buildContext.General.Version.NuGet);
            var nuGetRepositoryUrl = GetComponentNuGetRepositoryUrl(buildContext, component);
            var nuGetRepositoryApiKey = GetComponentNuGetRepositoryApiKey(buildContext, component);

            if (string.IsNullOrWhiteSpace(nuGetRepositoryUrl))
            {
                throw new Exception("NuGet repository is empty, as a protection mechanism this must *always* be specified to make sure packages aren't accidentally deployed to the default public NuGet feed");
            }

            NuGetPush(packageToPush, new NuGetPushSettings
            {
                Source = nuGetRepositoryUrl,
                ApiKey = nuGetRepositoryApiKey
            });

            await NotifyAsync(component, string.Format("Deployed to NuGet store"), TargetType.Component);
        }        
    }

    public override async Task FinalizeAsync(BuildContext buildContext)
    {

    }
}