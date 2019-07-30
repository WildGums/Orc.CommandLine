#addin "nuget:?package=Cake.MicrosoftTeams&version=0.9.0"

//-------------------------------------------------------------

public class MsTeamsContext : ContextBase
{
    public MsTeamsContext(IBuildContext parentBuildContext)
        : base(parentBuildContext)
    {
    }

    public string WebhookUrl { get; set; }
    public string WebhookUrlForErrors { get; set; }
    public bool IsAvailable { get; set; }

    protected override void ValidateContext()
    {
    }
    
    protected override void LogStateInfoForContext()
    {
        if (IsAvailable)
        {
            CakeContext.Information($"MS Teams is available");
        }
    }
}

//-------------------------------------------------------------

private MsTeamsContext InitializeMsTeamsContext(IBuildContext parentBuildContext)
{
    var data = new MsTeamsContext(parentBuildContext)
    {
    };

    data.WebhookUrl = GetBuildServerVariable(parentBuildContext, "MsTeamsWebhookUrl", showValue: false);
    data.WebhookUrlForErrors = GetBuildServerVariable(parentBuildContext, "MsTeamsWebhookUrlForErrors", data.WebhookUrl, showValue: false);

    if (!string.IsNullOrWhiteSpace(data.WebhookUrl))
    {
        data.IsAvailable = true;
    }
    
    return data;
}

//-------------------------------------------------------------

public static string GetMsTeamsWebhookUrl(BuildContext buildContext, string project, TargetType targetType)
{
    // Allow per target overrides via "MsTeamsWebhookUrlFor[TargetType]"
    var targetTypeUrl = GetTargetSpecificConfigurationValue(targetType, "MsTeamsWebhookUrlFor", string.Empty);
    if (!string.IsNullOrEmpty(targetTypeUrl))
    {
        return targetTypeUrl;
    }

    // Allow per project overrides via "MsTeamsWebhookUrlFor[ProjectName]"
    var projectTypeUrl = GetProjectSpecificConfigurationValue(project, "MsTeamsWebhookUrlFor", string.Empty);
    if (!string.IsNullOrEmpty(projectTypeUrl))
    {
        return projectTypeUrl;
    }

    // Return default fallback
    return MsTeamsWebhookUrl;
}

//-------------------------------------------------------------

public static string GetMsTeamsTarget(BuildContext buildContext, string project, TargetType targetType, NotificationType notificationType)
{
    if (notificationType == NotificationType.Error)
    {
        return buildContext.Notifications.MsTeams.WebhookUrlForErrors;
    }

    return GetMsTeamsWebhookUrl(buildContext, project, targetType);
}

//-------------------------------------------------------------

public static async Task NotifyMsTeamsAsync(BuildContext buildContext, string project, string message, TargetType targetType, NotificationType notificationType)
{
    var targetWebhookUrl = GetMsTeamsTarget(buildContext, project, targetType, notificationType);
    if (string.IsNullOrWhiteSpace(targetWebhookUrl))
    {
        return;
    }

    var messageCard = new MicrosoftTeamsMessageCard 
    {
        title = project,
        summary = notificationType.ToString(),
        sections = new []
        {
            new MicrosoftTeamsMessageSection
            {
                activityTitle = notificationType.ToString(),
                activitySubtitle = message,
                activityText = " ",
                activityImage = "https://raw.githubusercontent.com/cake-build/graphics/master/png/cake-small.png",
                facts = new [] 
                {
                    new MicrosoftTeamsMessageFacts { name ="Project", value = project },
                    new MicrosoftTeamsMessageFacts { name ="Version", value = buildContext.General.Version.FullSemVer },
                    new MicrosoftTeamsMessageFacts { name ="CakeVersion", value = Context.Environment.Runtime.CakeVersion.ToString() },
                    //new MicrosoftTeamsMessageFacts { name ="TargetFramework", value = Context.Environment.Runtime .TargetFramework.ToString() },
                },
            }
        }
    };

    var result = buildContext.CakeContext.MicrosoftTeamsPostMessage(messageCard, new MicrosoftTeamsSettings 
    {
        IncomingWebhookUrl = targetWebhookUrl
    });

    if (result != System.Net.HttpStatusCode.OK)
    {
        buildContext.CakeContext.Warning(string.Format("MsTeams result: {0}", result));
    }
}