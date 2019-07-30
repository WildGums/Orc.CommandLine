#tool "nuget:?package=OctopusTools&version=6.8.1"

public static string OctopusRepositoryUrl = GetBuildServerVariable("OctopusRepositoryUrl", showValue: true);
public static string OctopusRepositoryApiKey = GetBuildServerVariable("OctopusRepositoryApiKey", showValue: false);
public static string OctopusDeploymentTarget = GetBuildServerVariable("OctopusDeploymentTarget", "Staging", showValue: true);

//-------------------------------------------------------------

private static string GetOctopusRepositoryUrl(BuildContext buildContext, string projectName)
{
    // Allow per project overrides via "OctopusRepositoryUrlFor[ProjectName]"
    return GetProjectSpecificConfigurationValue(buildContext, projectName, "OctopusRepositoryUrlFor", OctopusRepositoryUrl);
}

//-------------------------------------------------------------

private static string GetOctopusRepositoryApiKey(BuildContext buildContext, string projectName)
{
    // Allow per project overrides via "OctopusRepositoryApiKeyFor[ProjectName]"
    return GetProjectSpecificConfigurationValue(buildContext, projectName, "OctopusRepositoryApiKeyFor", OctopusRepositoryApiKey);
}

//-------------------------------------------------------------

private static string GetOctopusDeploymentTarget(BuildContext buildContext, string projectName)
{
    // Allow per project overrides via "OctopusDeploymentTargetFor[ProjectName]"
    return GetProjectSpecificConfigurationValue(buildContext, projectName, "OctopusDeploymentTargetFor", OctopusDeploymentTarget);
}
