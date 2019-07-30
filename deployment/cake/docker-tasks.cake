#pragma warning disable 1998

#l "docker-variables.cake"
#l "lib-octopusdeploy.cake"

#addin "nuget:?package=Cake.FileHelpers&version=3.0.0"
#addin "nuget:?package=Cake.Docker&version=0.9.9"

//-------------------------------------------------------------

public class DockerImagesProcessor : ProcessorBase
{
    public DockerImagesProcessor(ICakeContext cakeContext)
        : base(cakeContext)
    {
        
    }

    public override bool HasItems(BuildContext buildContext)
    {
        return buildContext.DockerImages.Items.Count > 0;
    }

    private string GetDockerRegistryUrl(BuildContext buildContext, string projectName)
    {
        // Allow per project overrides via "DockerRegistryUrlFor[ProjectName]"
        return GetProjectSpecificConfigurationValue(projectName, "DockerRegistryUrlFor", buildContext.DockerImages.DockerRegistryUrl);
    }

    private string GetDockerRegistryUserName(BuildContext buildContext, string projectName)
    {
        // Allow per project overrides via "DockerRegistryUserNameFor[ProjectName]"
        return GetProjectSpecificConfigurationValue(projectName, "DockerRegistryUserNameFor", buildContext.DockerImages.DockerRegistryUserName);
    }

    private string GetDockerRegistryPassword(BuildContext buildContext, string projectName)
    {
        // Allow per project overrides via "DockerRegistryPasswordFor[ProjectName]"
        return GetProjectSpecificConfigurationValue(projectName, "DockerRegistryPasswordFor", buildContext.DockerImages.DockerRegistryPassword);
    }

    private string GetDockerImageName(BuildContext buildContext, string projectName)
    {
        var name = projectName.Replace(".", "-");
        return name.ToLower();
    }

    private string GetDockerImageTag(BuildContext buildContext, string projectName, string version)
    {
        var dockerRegistryUrl = GetDockerRegistryUrl(projectName);

        var tag = string.Format("{0}/{1}:{2}", dockerRegistryUrl, GetDockerImageName(buildContext, projectName), version);
        return tag.ToLower();
    }

    private void ConfigureDockerSettings(BuildContext buildContext, AutoToolSettings dockerSettings)
    {
        var engineUrl = buildContext.DockerImages.DockerEngineUrl;
        if (!string.IsNullOrWhiteSpace(engineUrl))
        {
            Information("Using remote docker engine: '{0}'", engineUrl);

            dockerSettings.ArgumentCustomization = args => args.Prepend($"-H {engineUrl}");
            //dockerSettings.BuildArg = new [] { $"DOCKER_HOST={engineUrl}" };
        }
    }

    public override async Task PrepareAsync(BuildContext buildContext)
    {
        if (!HasItems(buildContext))
        {
            return;
        }

        // Check whether projects should be processed, `.ToList()` 
        // is required to prevent issues with foreach
        foreach (var dockerImage in buildContext.DockerImages.Items.ToList())
        {
            if (!ShouldProcessProject(buildContext, dockerImage))
            {
                DockerImages.Remove(dockerImage);
            }
        }        
    }

    public override async Task UpdateInfoAsync(BuildContext buildContext)
    {
        if (!HasItems(buildContext))
        {
            return;
        }

        // Doesn't seem neccessary yet
        // foreach (var dockerImage in buildContext.DockerImages.Items)
        // {
        //     Information("Updating version for docker image '{0}'", dockerImage);

        //     var projectFileName = GetProjectFileName(dockerImage);

        //     TransformConfig(projectFileName, new TransformationCollection 
        //     {
        //         { "Project/PropertyGroup/PackageVersion", VersionNuGet }
        //     });
        // }        
    }

    public override async Task BuildAsync(BuildContext buildContext)
    {
        if (!HasItems(buildContext))
        {
            return;
        }
        
        foreach (var dockerImage in buildContext.DockerImages.Items)
        {
            LogSeparator("Building docker image '{0}'", dockerImage);

            var projectFileName = GetProjectFileName(dockerImage);
            
            var msBuildSettings = new MSBuildSettings {
                Verbosity = Verbosity.Quiet, // Verbosity.Diagnostic
                ToolVersion = MSBuildToolVersion.Default,
                Configuration = buildContext.General.Solution.ConfigurationName,
                MSBuildPlatform = MSBuildPlatform.x86, // Always require x86, see platform for actual target platform
                PlatformTarget = PlatformTarget.MSIL
            };

            ConfigureMsBuild(msBuildSettings, dockerImage);

            // Always disable SourceLink
            msBuildSettings.WithProperty("EnableSourceLink", "false");

            // Note: we need to set OverridableOutputPath because we need to be able to respect
            // AppendTargetFrameworkToOutputPath which isn't possible for global properties (which
            // are properties passed in using the command line)
            var outputDirectory = string.Format("{0}/{1}/", buildContext.General.OutputRootDirectory, dockerImage);
            Information("Output directory: '{0}'", outputDirectory);
            msBuildSettings.WithProperty("OverridableOutputPath", outputDirectory);
            msBuildSettings.WithProperty("PackageOutputPath", buildContext.General.OutputRootDirectory);

            MSBuild(projectFileName, msBuildSettings);
        }        
    }

    public override async Task PackageAsync(BuildContext buildContext)
    {
        if (!HasItems(buildContext))
        {
            return;
        }

        // The following directories are being created, ready for docker images to be used:
        // ./output => output of the publish step
        // ./config => docker image and config files, in case they need to be packed as well

        foreach (var dockerImage in buildContext.DockerImages.Items)
        {
            LogSeparator("Packaging docker image '{0}'", dockerImage);

            var projectFileName = string.Format("./src/{0}/{0}.csproj", dockerImage);
            var dockerImageSpecificationDirectory = string.Format("./deployment/docker/{0}/", dockerImage);
            var dockerImageSpecificationFileName = string.Format("{0}/{1}", dockerImageSpecificationDirectory, dockerImage);

            var outputRootDirectory =  string.Format("{0}/{1}/output", buildContext.General.OutputRootDirectory, dockerImage);

            Information("1) Preparing ./config for package '{0}'", dockerImage);

            // ./config
            var confTargetDirectory = string.Format("{0}/conf", outputRootDirectory);
            Information("Conf directory: '{0}'", confTargetDirectory);

            CreateDirectory(confTargetDirectory);

            var confSourceDirectory = string.Format("{0}*", dockerImageSpecificationDirectory);
            Information("Copying files from '{0}' => '{1}'", confSourceDirectory, confTargetDirectory);

            CopyFiles(confSourceDirectory, confTargetDirectory, true);

            LogSeparator();

            Information("2) Preparing ./output using 'dotnet publish' for package '{0}'", dockerImage);

            // ./output
            var outputDirectory = string.Format("{0}/output", outputRootDirectory);
            Information("Output directory: '{0}'", outputDirectory);

            var msBuildSettings = new DotNetCoreMSBuildSettings();

            // Note: we need to set OverridableOutputPath because we need to be able to respect
            // AppendTargetFrameworkToOutputPath which isn't possible for global properties (which
            // are properties passed in using the command line)
            msBuildSettings.WithProperty("OverridableOutputPath", outputDirectory);
            msBuildSettings.WithProperty("PackageOutputPath", outputDirectory);
            msBuildSettings.WithProperty("ConfigurationName", buildContext.General.Solution.ConfigurationName);
            msBuildSettings.WithProperty("PackageVersion", buildContext.General.Version.NuGet);

            // Disable code analyses, we experienced publish issues with mvc .net core projects
            msBuildSettings.WithProperty("RunCodeAnalysis", "false");

            var publishSettings = new DotNetCorePublishSettings
            {
                MSBuildSettings = msBuildSettings,
                OutputDirectory = outputDirectory,
                Configuration = buildContext.General.Solution.ConfigurationName,
                //NoBuild = true
            };

            DotNetCorePublish(projectFileName, publishSettings);

            LogSeparator();

            Information("3) Using 'docker build' to package '{0}'", dockerImage);

            // docker build ..\..\output\Release\platform -f .\Dockerfile

            // From the docs (https://docs.microsoft.com/en-us/azure/app-service/containers/tutorial-custom-docker-image#use-a-docker-image-from-any-private-registry-optional), 
            // we need something like this:
            // docker tag <azure-container-registry-name>.azurecr.io/mydockerimage
            var dockerRegistryUrl = GetDockerRegistryUrl(buildContext, dockerImage);

            // Note: to prevent all output & source files to be copied to the docker context, we will set the
            // output directory as context (to keep the footprint as small as possible)

            var dockerSettings = new DockerImageBuildSettings
            {
                NoCache = true, // Don't use cache, always make sure to fetch the right images
                File = dockerImageSpecificationFileName,
                Platform = "linux",
                Tag = new string[] { GetDockerImageTag(dockerImage, buildContext.General.Version.NuGet) }
            };

            ConfigureDockerSettings(dockerSettings);

            Information("Docker files source directory: '{0}'", outputRootDirectory);

            DockerBuild(dockerSettings, outputRootDirectory);

            LogSeparator();
        }        
    }

    public override async Task DeployAsync(BuildContext buildContext)
    {
        if (!HasItems(buildContext))
        {
            return;
        }

        foreach (var dockerImage in buildContext.DockerImages.Items)
        {
            if (!ShouldDeployProject(buildContext, dockerImage))
            {
                Information("Docker image '{0}' should not be deployed", dockerImage);
                continue;
            }

            LogSeparator("Deploying docker image '{0}'", dockerImage);

            var dockerRegistryUrl = GetDockerRegistryUrl(buildContext, dockerImage);
            var dockerRegistryUserName = GetDockerRegistryUserName(buildContext, dockerImage);
            var dockerRegistryPassword = GetDockerRegistryPassword(buildContext, dockerImage);
            var dockerImageName = GetDockerImageName(buildContext, dockerImage);
            var dockerImageTag = GetDockerImageTag(buildContext, dockerImage, buildContext.General.Version.NuGet);
            var octopusRepositoryUrl = GetOctopusRepositoryUrl(buildContext, dockerImage);
            var octopusRepositoryApiKey = GetOctopusRepositoryApiKey(buildContext, dockerImage);
            var octopusDeploymentTarget = GetOctopusDeploymentTarget(buildContext, dockerImage);

            if (string.IsNullOrWhiteSpace(dockerRegistryUrl))
            {
                throw new Exception("Docker registry url is empty, as a protection mechanism this must *always* be specified to make sure packages aren't accidentally deployed to some default public registry");
            }

            // Note: we are logging in each time because the registry might be different per container
            Information("Logging in to docker @ '{0}'", dockerRegistryUrl);

            var dockerLoginSettings = new DockerRegistryLoginSettings
            {
                Username = dockerRegistryUserName,
                Password = dockerRegistryPassword
            };

            ConfigureDockerSettings(dockerLoginSettings);

            DockerLogin(dockerLoginSettings, dockerRegistryUrl);

            try
            {
                Information("Pushing docker images with tag '{0}' to '{1}'", dockerImageTag, dockerRegistryUrl);

                var dockerImagePushSettings = new DockerImagePushSettings
                {
                };

                ConfigureDockerSettings(dockerImagePushSettings);

                DockerPush(dockerImagePushSettings, dockerImageTag);

                if (string.IsNullOrWhiteSpace(octopusRepositoryUrl))
                {
                    Warning("Octopus Deploy url is not specified, skipping deployment to Octopus Deploy");
                    continue;
                }

                var imageVersion = buildContext.General.Version.NuGet;

                Information("Creating release '{0}' in Octopus Deploy", imageVersion);

                OctoCreateRelease(dockerImage, new CreateReleaseSettings 
                {
                    Server = octopusRepositoryUrl,
                    ApiKey = octopusRepositoryApiKey,
                    ReleaseNumber = imageVersion,
                    DefaultPackageVersion = imageVersion,
                    IgnoreExisting = true,
                    Packages = new Dictionary<string, string>
                    {
                        { dockerImageName, imageVersion }
                    }
                });

                Information("Deploying release '{0}' via Octopus Deploy", imageVersion);

                OctoDeployRelease(octopusRepositoryUrl, octopusRepositoryApiKey, dockerImage, octopusDeploymentTarget, 
                    imageVersion, new OctopusDeployReleaseDeploymentSettings
                {
                    ShowProgress = true,
                    WaitForDeployment = true,
                    DeploymentTimeout = TimeSpan.FromMinutes(5),
                    CancelOnTimeout = true,
                    GuidedFailure = true,
                    Force = true,
                    NoRawLog = true,
                });

                await NotifyAsync(dockerImage, string.Format("Deployed to Octopus Deploy"), TargetType.DockerImage);
            }
            finally
            {
                Information("Logging out of docker @ '{0}'", dockerRegistryUrl);

                var dockerLogoutSettings = new DockerRegistryLogoutSettings
                {
                };

                ConfigureDockerSettings(dockerLogoutSettings);

                DockerLogout(dockerLogoutSettings, dockerRegistryUrl);
            }
        }        
    }

    public override async Task FinalizeAsync(BuildContext buildContext)
    {

    }
}
