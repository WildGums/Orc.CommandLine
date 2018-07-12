namespace Orc.CommandLine.Tests.Context
{
    public class BrandingCommandLineContext : ContextBase
    {
        [Option("ac", "appcolor", HelpText = "The accent color for the application")]
        public string AppColor { get; set; }

        [Option("an", "appname", HelpText = "The name for the application")]
        public string AppName { get; set; }
    }
}
