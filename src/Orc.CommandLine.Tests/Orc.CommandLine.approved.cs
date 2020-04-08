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
        public CommandLineParser(Orc.CommandLine.IOptionDefinitionService optionDefinitionService, Catel.Services.ILanguageService languageService, Orc.CommandLine.ICommandLineService commandLineService) { }
        protected virtual System.Text.RegularExpressions.Regex CreateRegex(Orc.CommandLine.IContext targetContext) { }
        public Catel.Data.IValidationContext Parse(Orc.CommandLine.IContext targetContext) { }
        public Catel.Data.IValidationContext Parse(string commandLine, Orc.CommandLine.IContext targetContext) { }
        public Catel.Data.IValidationContext Parse(System.Collections.Generic.IEnumerable<string> commandLineArguments, Orc.CommandLine.IContext targetContext) { }
        public Catel.Data.IValidationContext Parse(System.Collections.Generic.List<string> commandLineArguments, Orc.CommandLine.IContext targetContext) { }
        protected virtual void ValidateMandatorySwitches(Catel.Data.IValidationContext validationContext, System.Collections.Generic.IEnumerable<Orc.CommandLine.OptionDefinition> optionDefinitions, System.Collections.Generic.HashSet<string> handledOptions) { }
    }
    public class CommandLineService : Orc.CommandLine.ICommandLineService
    {
        public CommandLineService() { }
        public virtual string GetCommandLine() { }
    }
    public abstract class ContextBase : Orc.CommandLine.IContext
    {
        protected ContextBase() { }
        public bool IsHelp { get; set; }
        public string OriginalCommandLine { get; set; }
        public System.Collections.Generic.List<char> QuoteSplitCharacters { get; }
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
        Catel.Data.IValidationContext Parse(Orc.CommandLine.IContext targetContext);
        Catel.Data.IValidationContext Parse(System.Collections.Generic.List<string> commandLineArguments, Orc.CommandLine.IContext targetContext);
        Catel.Data.IValidationContext Parse(System.Collections.Generic.IEnumerable<string> commandLineArguments, Orc.CommandLine.IContext targetContext);
        Catel.Data.IValidationContext Parse(string commandLine, Orc.CommandLine.IContext targetContext);
    }
    public class static ICommandLineParserExtensions
    {
        public static System.Collections.Generic.IEnumerable<string> GetAppHeader(this Orc.CommandLine.ICommandLineParser commandLineParser) { }
        public static System.Collections.Generic.IEnumerable<string> GetHelp(this Orc.CommandLine.ICommandLineParser commandLineParser, Orc.CommandLine.IContext targetContext) { }
    }
    public interface ICommandLineService
    {
        string GetCommandLine();
    }
    public interface IContext
    {
        bool IsHelp { get; set; }
        string OriginalCommandLine { get; set; }
        System.Collections.Generic.List<char> QuoteSplitCharacters { get; }
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
        [System.ObsoleteAttribute("Use string overload instead so multiple characters can be used for the short name" +
            ". Will be removed in version 4.0.0.", true)]
        public OptionAttribute(char shortName, string longName) { }
        public OptionAttribute(string shortName, string longName) { }
        public bool AcceptsValue { get; set; }
        public string DisplayName { get; set; }
        public string HelpText { get; set; }
        public bool IsMandatory { get; set; }
        public string LongName { get; }
        public string ShortName { get; }
        public bool TrimQuotes { get; set; }
        public bool TrimWhiteSpace { get; set; }
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
        public string ShortName { get; set; }
        public bool TrimQuotes { get; set; }
        public bool TrimWhiteSpace { get; set; }
        public override string ToString() { }
    }
    public class static OptionDefinitionExtensions
    {
        public static string GetSwitchDisplay(this Orc.CommandLine.OptionDefinition optionDefinition) { }
        public static bool HasSwitch(this Orc.CommandLine.OptionDefinition optionDefinition) { }
        public static bool IsSwitch(this Orc.CommandLine.OptionDefinition optionDefinition, string actualSwitch, char[] quoteSplitCharacters) { }
    }
    public class OptionDefinitionService : Orc.CommandLine.IOptionDefinitionService
    {
        public OptionDefinitionService() { }
        public System.Collections.Generic.IEnumerable<Orc.CommandLine.OptionDefinition> GetOptionDefinitions(Orc.CommandLine.IContext targetContext) { }
    }
    public class static StringExtensions
    {
        [System.ObsoleteAttribute("Use `Environment.GetCommandLineArgs()` instead. Will be treated as an error from " +
            "version 3.3.0. Will be removed in version 4.0.0.", false)]
        public static string GetCommandLine(this string commandLine, bool removeFirstArgument) { }
        public static bool IsHelp(this string singleArgument, char[] quoteSplitCharacters) { }
        public static bool IsSwitch(this string value, char[] quoteSplitCharacters) { }
        public static bool IsSwitch(this string switchName, string value, char[] quoteSplitCharacters) { }
        public static string TrimSwitchPrefix(this string value) { }
    }
}