// Customize this file when using a different test framework
#l "tests-variables.cake"
#l "tests-nunit.cake"

//-------------------------------------------------------------

private void BuildTestProjects(BuildContext buildContext)
{
    // In case of a local build and we have included / excluded anything, skip tests
    if (buildContext.General.IsLocalBuild && (buildContext.General.Includes.Count > 0 || buildContext.General.Excludes.Count > 0))
    {
        Information("Skipping test project because this is a local build with specific includes / excludes");
        return;
    }

    foreach (var testProject in buildContext.Tests.TestProjects)
    {
        LogSeparator("Building test project '{0}'", testProject);

        var projectFileName = GetProjectFileName(testProject);
        
        var msBuildSettings = new MSBuildSettings
        {
            Verbosity = Verbosity.Quiet, // Verbosity.Diagnostic
            ToolVersion = MSBuildToolVersion.Default,
            Configuration = ConfigurationName,
            MSBuildPlatform = MSBuildPlatform.x86, // Always require x86, see platform for actual target platform
            PlatformTarget = PlatformTarget.MSIL
        };

        ConfigureMsBuild(msBuildSettings, testProject);

        // Always disable SourceLink
        msBuildSettings.WithProperty("EnableSourceLink", "false");

        // Force disable SonarQube
        msBuildSettings.WithProperty("SonarQubeExclude", "true");

        // Note: we need to set OverridableOutputPath because we need to be able to respect
        // AppendTargetFrameworkToOutputPath which isn't possible for global properties (which
        // are properties passed in using the command line)
        var outputDirectory = string.Format("{0}/{1}/", OutputRootDirectory, testProject);
        Information("Output directory: '{0}'", outputDirectory);
        msBuildSettings.WithProperty("OverridableOutputPath", outputDirectory);
        msBuildSettings.WithProperty("PackageOutputPath", OutputRootDirectory);

        MSBuild(projectFileName, msBuildSettings);
    }
}

//-------------------------------------------------------------

private void RunUnitTests(string projectName)
{
    var testResultsDirectory = string.Format("{0}/testresults/{1}/", OutputRootDirectory, projectName);

    CreateDirectory(testResultsDirectory);

    var ranTests = false;
    var failed = false;

    try
    {
        if (IsDotNetCoreProject(projectName))
        {
            Information("Project '{0}' is a .NET core project, using 'dotnet test' to run the unit tests", projectName);

            var projectFileName = GetProjectFileName(projectName);

            DotNetCoreTest(projectFileName, new DotNetCoreTestSettings
            {
                Configuration = ConfigurationName,
                NoRestore = true,
                NoBuild = true,
                OutputDirectory = string.Format("{0}/{1}", GetProjectOutputDirectory(projectName), TestTargetFramework),
                ResultsDirectory = testResultsDirectory
            });

            // Information("Project '{0}' is a .NET core project, using 'dotnet vstest' to run the unit tests", projectName); 

            // var testFile = string.Format("{0}/{1}/{2}.dll", GetProjectOutputDirectory(projectName), TestTargetFramework, projectName);

            // DotNetCoreVSTest(testFile, new DotNetCoreVSTestSettings
            // {
            //     //Platform = TestFramework
            //     ResultsDirectory = testResultsDirectory
            // });

            ranTests = true;
        }
        else
        {
            Information("Project '{0}' is a .NET project, using '{1} runner' to run the unit tests", projectName, TestFramework);

            if (TestFramework.ToLower().Equals("nunit"))
            {
                RunTestsUsingNUnit(projectName, TestTargetFramework, testResultsDirectory);

                ranTests = true;
            }
        }
    }
    catch (Exception ex)
    {
        Warning("An exception occurred: {0}", ex.Message);

        failed = true;   
    }

    if (ranTests)
    {
        Information("Results are available in '{0}'", testResultsDirectory);
    }
    else if (failed)
    {
        throw new Exception("Unit test execution failed");
    }
    else
    {
        Warning("No tests were executed, check whether the used test framework '{0}' is available", TestFramework);
    }
}

//-------------------------------------------------------------