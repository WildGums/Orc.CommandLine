[assembly: System.Resources.NeutralResourcesLanguage("en-US")]
[assembly: System.Runtime.InteropServices.ComVisible(false)]
[assembly: System.Runtime.Versioning.TargetFramework(".NETCoreApp,Version=v3.1", FrameworkDisplayName="")]
public static class ModuleInitializer
{
    public static void Initialize() { }
}
namespace Orc.CommandLine
{
    public abstract class CommandLineContextBase : Orc.CommandLine.ICommandLineContext
    {
        protected CommandLineContextBase() { }
        public bool IsHelp { get; set; }
        public string OriginalCommandLine { get; set; }
        public System.Collections.Generic.Dictionary<string, string> RawValues { get; }
        public Catel.Data.IValidationContext ValidationContext { get; }
        public virtual void Finish() { }
    }
    public class CommandLineException : System.Exception
    {
        public CommandLineException(string message) { }
        public CommandLineException(string message, System.Exception innerException) { }
    }
    public class CommandLineParseOptions
    {
        public CommandLineParseOptions() { }
        public System.Collections.Generic.List<char> QuoteSplitCharacters { get; }
    }
    public class CommandLineParser : Orc.CommandLine.ICommandLineParser
    {
        public CommandLineParser(Orc.CommandLine.IOptionDefinitionService optionDefinitionService, Catel.Services.ILanguageService languageService, Orc.CommandLine.ICommandLineService commandLineService, Catel.IoC.ITypeFactory typeFactory) { }
        public Orc.CommandLine.CommandLineParseOptions DefaultOptions { get; }
        protected virtual System.Text.RegularExpressions.Regex CreateRegex(Orc.CommandLine.CommandLineParseOptions options = null) { }
        public Orc.CommandLine.ICommandLineContext Parse(System.Type contextType, Orc.CommandLine.CommandLineParseOptions options = null) { }
        public Orc.CommandLine.ICommandLineContext Parse(System.Type contextType, System.Collections.Generic.IEnumerable<string> commandLineArguments, Orc.CommandLine.CommandLineParseOptions options = null) { }
        public Orc.CommandLine.ICommandLineContext Parse(System.Type contextType, System.Collections.Generic.List<string> commandLineArguments, Orc.CommandLine.CommandLineParseOptions options = null) { }
        public Orc.CommandLine.ICommandLineContext Parse(System.Type contextType, string commandLine, Orc.CommandLine.CommandLineParseOptions options = null) { }
        protected virtual void ValidateMandatorySwitches(Catel.Data.IValidationContext validationContext, System.Collections.Generic.IEnumerable<Orc.CommandLine.OptionDefinition> optionDefinitions, System.Collections.Generic.HashSet<string> handledOptions) { }
    }
    public class CommandLineService : Orc.CommandLine.ICommandLineService
    {
        public CommandLineService() { }
        public virtual string GetCommandLine() { }
    }
    public class HelpWriterService : Orc.CommandLine.IHelpWriterService
    {
        public HelpWriterService(Orc.CommandLine.IOptionDefinitionService optionDefinitionService) { }
        public System.Collections.Generic.IEnumerable<string> GetAppHeader() { }
        public System.Collections.Generic.IEnumerable<string> GetHelp(Orc.CommandLine.ICommandLineContext targetContext) { }
    }
    public interface ICommandLineContext
    {
        bool IsHelp { get; set; }
        string OriginalCommandLine { get; set; }
        System.Collections.Generic.Dictionary<string, string> RawValues { get; }
        Catel.Data.IValidationContext ValidationContext { get; }
        void Finish();
    }
    public interface ICommandLineParser
    {
        Orc.CommandLine.CommandLineParseOptions DefaultOptions { get; }
        Orc.CommandLine.ICommandLineContext Parse(System.Type contextType, Orc.CommandLine.CommandLineParseOptions options = null);
        Orc.CommandLine.ICommandLineContext Parse(System.Type contextType, System.Collections.Generic.IEnumerable<string> commandLineArguments, Orc.CommandLine.CommandLineParseOptions options = null);
        Orc.CommandLine.ICommandLineContext Parse(System.Type contextType, System.Collections.Generic.List<string> commandLineArguments, Orc.CommandLine.CommandLineParseOptions options = null);
        Orc.CommandLine.ICommandLineContext Parse(System.Type contextType, string commandLine, Orc.CommandLine.CommandLineParseOptions options = null);
    }
    public static class ICommandLineParserExtensions
    {
        public static System.Collections.Generic.IEnumerable<string> GetAppHeader(this Orc.CommandLine.ICommandLineParser commandLineParser) { }
        public static System.Collections.Generic.IEnumerable<string> GetHelp(this Orc.CommandLine.ICommandLineParser commandLineParser, Orc.CommandLine.ICommandLineContext targetContext) { }
        public static TContext Parse<TContext>(this Orc.CommandLine.ICommandLineParser commandLineParser)
            where TContext : Orc.CommandLine.ICommandLineContext { }
        public static TContext Parse<TContext>(this Orc.CommandLine.ICommandLineParser commandLineParser, System.Collections.Generic.IEnumerable<string> commandLineArguments)
            where TContext : Orc.CommandLine.ICommandLineContext { }
        public static TContext Parse<TContext>(this Orc.CommandLine.ICommandLineParser commandLineParser, System.Collections.Generic.List<string> commandLineArguments)
            where TContext : Orc.CommandLine.ICommandLineContext { }
        public static TContext Parse<TContext>(this Orc.CommandLine.ICommandLineParser commandLineParser, string commandLine)
            where TContext : Orc.CommandLine.ICommandLineContext { }
    }
    public interface ICommandLineService
    {
        string GetCommandLine();
    }
    public interface IHelpWriterService
    {
        System.Collections.Generic.IEnumerable<string> GetAppHeader();
        System.Collections.Generic.IEnumerable<string> GetHelp(Orc.CommandLine.ICommandLineContext targetContext);
    }
    public interface IOptionDefinitionService
    {
        System.Collections.Generic.IEnumerable<Orc.CommandLine.OptionDefinition> GetOptionDefinitions(Orc.CommandLine.ICommandLineContext targetContext);
    }
    [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.All)]
    public class OptionAttribute : System.Attribute
    {
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
    public static class OptionDefinitionExtensions
    {
        public static string GetSwitchDisplay(this Orc.CommandLine.OptionDefinition optionDefinition) { }
        public static bool HasSwitch(this Orc.CommandLine.OptionDefinition optionDefinition) { }
        public static bool IsSwitch(this Orc.CommandLine.OptionDefinition optionDefinition, string actualSwitch, char[] quoteSplitCharacters) { }
    }
    public class OptionDefinitionService : Orc.CommandLine.IOptionDefinitionService
    {
        public OptionDefinitionService() { }
        public System.Collections.Generic.IEnumerable<Orc.CommandLine.OptionDefinition> GetOptionDefinitions(Orc.CommandLine.ICommandLineContext targetContext) { }
    }
    public static class StringExtensions
    {
        public static bool IsHelp(this string singleArgument, char[] quoteSplitCharacters) { }
        public static bool IsSwitch(this string value, char[] quoteSplitCharacters) { }
        public static bool IsSwitch(this string switchName, string value, char[] quoteSplitCharacters) { }
        public static string TrimSwitchPrefix(this string value) { }
    }
}