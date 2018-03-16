[assembly: System.Resources.NeutralResourcesLanguageAttribute("en-US")]
[assembly: System.Runtime.InteropServices.ComVisibleAttribute(false)]
[assembly: System.Runtime.Versioning.TargetFrameworkAttribute(".NETFramework,Version=v4.6", FrameworkDisplayName=".NET Framework 4.6")]
public class static ModuleInitializer
{
    public static void Initialize() { }
}
namespace Orc.CommandLine
{
    public class CommandLineException : System.Exception
    {
        public CommandLineException(string message) { }
        public CommandLineException(string message, System.Exception innerException) { }
    }
    public class CommandLineParser : Orc.CommandLine.ICommandLineParser
    {
        public CommandLineParser(Orc.CommandLine.IOptionDefinitionService optionDefinitionService, Catel.Services.ILanguageService languageService) { }
        public Catel.Data.IValidationContext Parse(System.Collections.Generic.IEnumerable<string> commandLineArguments, Orc.CommandLine.IContext targetContext) { }
        public Catel.Data.IValidationContext Parse(System.Collections.Generic.List<string> commandLineArguments, Orc.CommandLine.IContext targetContext) { }
    }
    public abstract class ContextBase : Orc.CommandLine.IContext
    {
        protected ContextBase() { }
        public bool IsHelp { get; set; }
        public string OriginalCommandLine { get; set; }
        public System.Collections.Generic.Dictionary<string, string> RawValues { get; }
        public virtual void Finish() { }
    }
    public class HelpWriterService : Orc.CommandLine.IHelpWriterService
    {
        public HelpWriterService(Orc.CommandLine.IOptionDefinitionService optionDefinitionService) { }
        public System.Collections.Generic.IEnumerable<string> GetAppHeader() { }
        public System.Collections.Generic.IEnumerable<string> GetHelp(Orc.CommandLine.IContext targetContext) { }
    }
    public interface ICommandLineParser
    {
        Catel.Data.IValidationContext Parse(System.Collections.Generic.List<string> commandLineArguments, Orc.CommandLine.IContext targetContext);
        Catel.Data.IValidationContext Parse(System.Collections.Generic.IEnumerable<string> commandLineArguments, Orc.CommandLine.IContext targetContext);
    }
    public class static ICommandLineParserExtensions
    {
        public static System.Collections.Generic.IEnumerable<string> GetAppHeader(this Orc.CommandLine.ICommandLineParser commandLineParser) { }
        public static System.Collections.Generic.IEnumerable<string> GetHelp(this Orc.CommandLine.ICommandLineParser commandLineParser, Orc.CommandLine.IContext targetContext) { }
        public static Catel.Data.IValidationContext Parse(this Orc.CommandLine.ICommandLineParser commandLineParser, string commandLineArguments, Orc.CommandLine.IContext targetContext) { }
    }
    public interface IContext
    {
        bool IsHelp { get; set; }
        string OriginalCommandLine { get; set; }
        System.Collections.Generic.Dictionary<string, string> RawValues { get; }
        void Finish();
    }
    public interface IHelpWriterService
    {
        System.Collections.Generic.IEnumerable<string> GetAppHeader();
        System.Collections.Generic.IEnumerable<string> GetHelp(Orc.CommandLine.IContext targetContext);
    }
    public interface IOptionDefinitionService
    {
        System.Collections.Generic.IEnumerable<Orc.CommandLine.OptionDefinition> GetOptionDefinitions(Orc.CommandLine.IContext targetContext);
    }
    [System.AttributeUsageAttribute(System.AttributeTargets.Property | System.AttributeTargets.All)]
    public class OptionAttribute : System.Attribute
    {
        public OptionAttribute(char shortName, string longName) { }
        public bool AcceptsValue { get; set; }
        public string DisplayName { get; set; }
        public string HelpText { get; set; }
        public bool IsMandatory { get; set; }
        public string LongName { get; }
        public char ShortName { get; }
        public bool TrimQuotes { get; set; }
    }
    public class OptionDefinition
    {
        public OptionDefinition() { }
        public bool AcceptsValue { get; set; }
        public string DisplayName { get; set; }
        public string HelpText { get; set; }
        public bool IsMandatory { get; set; }
        public string LongName { get; set; }
        public string PropertyNameOnContext { get; set; }
        public char ShortName { get; set; }
        public bool TrimQuotes { get; set; }
        public override string ToString() { }
    }
    public class static OptionDefinitionExtensions
    {
        public static string GetSwitchDisplay(this Orc.CommandLine.OptionDefinition optionDefinition) { }
        public static bool HasSwitch(this Orc.CommandLine.OptionDefinition optionDefinition) { }
        public static bool IsSwitch(this Orc.CommandLine.OptionDefinition optionDefinition, string actualSwitch) { }
    }
    public class OptionDefinitionService : Orc.CommandLine.IOptionDefinitionService
    {
        public OptionDefinitionService() { }
        public System.Collections.Generic.IEnumerable<Orc.CommandLine.OptionDefinition> GetOptionDefinitions(Orc.CommandLine.IContext targetContext) { }
    }
    public class static StringExtensions
    {
        public static readonly System.Collections.Generic.List<string> AcceptedSwitchPrefixes;
        public static string GetCommandLine(this string commandLine, bool removeFirstArgument) { }
        public static bool IsHelp(this string singleArgument) { }
        public static bool IsSwitch(this string value) { }
        public static bool IsSwitch(this string switchName, string value) { }
        public static string TrimSwitchPrefix(this string value) { }
    }
}