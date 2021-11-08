[assembly: System.Resources.NeutralResourcesLanguage("en-US")]
[assembly: System.Runtime.InteropServices.ComVisible(false)]
[assembly: System.Runtime.Versioning.TargetFramework(".NETCoreApp,Version=v5.0", FrameworkDisplayName="")]
public static class ModuleInitializer
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
        protected virtual System.Text.RegularExpressions.Regex CreateRegex(Orc.CommandLine.ICommandLineParsingContext targetContext) { }
        [System.Obsolete("Use `Parse<T>(ICommandLineParsingContext commandLineParsingContext)` instead. Wil" +
            "l be treated as an error from version 4.2.0. Will be removed in version 5.0.0.", false)]
        public Catel.Data.IValidationContext Parse(Orc.CommandLine.IContext targetContext) { }
        [System.Obsolete("Use `Parse<T>(ICommandLineParsingContext commandLineParsingContext)` instead. Wil" +
            "l be treated as an error from version 4.2.0. Will be removed in version 5.0.0.", false)]
        public Catel.Data.IValidationContext Parse(System.Collections.Generic.IEnumerable<string> commandLineArguments, Orc.CommandLine.IContext targetContext) { }
        [System.Obsolete("Use `Parse<T>(ICommandLineParsingContext commandLineParsingContext)` instead. Wil" +
            "l be treated as an error from version 4.2.0. Will be removed in version 5.0.0.", false)]
        public Catel.Data.IValidationContext Parse(System.Collections.Generic.List<string> commandLineArguments, Orc.CommandLine.IContext targetContext) { }
        [System.Obsolete("Use `Parse<T>(ICommandLineParsingContext commandLineParsingContext)` instead. Wil" +
            "l be treated as an error from version 4.2.0. Will be removed in version 5.0.0.", false)]
        public Catel.Data.IValidationContext Parse(string commandLine, Orc.CommandLine.IContext targetContext) { }
        public TResult Parse<TResult>(Orc.CommandLine.ICommandLineParsingContext commandLineParsingContext)
            where TResult : Orc.CommandLine.IResult { }
        protected virtual void ValidateMandatorySwitches(Catel.Data.IValidationContext validationContext, System.Collections.Generic.IEnumerable<Orc.CommandLine.OptionDefinition> optionDefinitions, System.Collections.Generic.HashSet<string> handledOptions) { }
    }
    public class CommandLineParsingContextBase : Orc.CommandLine.ICommandLineParsingContext
    {
        public CommandLineParsingContextBase() { }
        public CommandLineParsingContextBase(string commandLine) { }
        public string CommandLine { get; set; }
        public System.Collections.Generic.List<char> QuoteSplitCharacters { get; }
    }
    public class CommandLineService : Orc.CommandLine.ICommandLineService
    {
        public CommandLineService() { }
        public virtual string GetCommandLine() { }
    }
    public abstract class ContextBase : Orc.CommandLine.ICommandLineParsingContext, Orc.CommandLine.IContext, Orc.CommandLine.IResult
    {
        protected ContextBase() { }
        public string CommandLine { get; set; }
        public bool IsHelp { get; set; }
        public string OriginalCommandLine { get; set; }
        public System.Collections.Generic.List<char> QuoteSplitCharacters { get; }
        public System.Collections.Generic.Dictionary<string, string> RawValues { get; }
        public Catel.Data.ValidationContext ValidationContext { get; set; }
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
        [System.Obsolete("Use `Parse<T>(ICommandLineParsingContext commandLineParsingContext)` instead. Wil" +
            "l be treated as an error from version 4.2.0. Will be removed in version 5.0.0.", false)]
        Catel.Data.IValidationContext Parse(Orc.CommandLine.IContext targetContext);
        [System.Obsolete("Use `Parse<T>(ICommandLineParsingContext commandLineParsingContext)` instead. Wil" +
            "l be treated as an error from version 4.2.0. Will be removed in version 5.0.0.", false)]
        Catel.Data.IValidationContext Parse(System.Collections.Generic.IEnumerable<string> commandLineArguments, Orc.CommandLine.IContext targetContext);
        [System.Obsolete("Use `Parse<T>(ICommandLineParsingContext commandLineParsingContext)` instead. Wil" +
            "l be treated as an error from version 4.2.0. Will be removed in version 5.0.0.", false)]
        Catel.Data.IValidationContext Parse(System.Collections.Generic.List<string> commandLineArguments, Orc.CommandLine.IContext targetContext);
        [System.Obsolete("Use `Parse<T>(ICommandLineParsingContext commandLineParsingContext)` instead. Wil" +
            "l be treated as an error from version 4.2.0. Will be removed in version 5.0.0.", false)]
        Catel.Data.IValidationContext Parse(string commandLine, Orc.CommandLine.IContext targetContext);
        T Parse<T>(Orc.CommandLine.ICommandLineParsingContext commandLineParsingContext)
            where T : Orc.CommandLine.IResult;
    }
    public static class ICommandLineParserExtensions
    {
        public static System.Collections.Generic.IEnumerable<string> GetAppHeader(this Orc.CommandLine.ICommandLineParser commandLineParser) { }
        public static System.Collections.Generic.IEnumerable<string> GetHelp(this Orc.CommandLine.ICommandLineParser commandLineParser, Orc.CommandLine.IContext targetContext) { }
    }
    public interface ICommandLineParsingContext
    {
        string CommandLine { get; set; }
        System.Collections.Generic.List<char> QuoteSplitCharacters { get; }
    }
    public interface ICommandLineService
    {
        string GetCommandLine();
    }
    public interface IContext : Orc.CommandLine.ICommandLineParsingContext
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
    public interface IResult : Orc.CommandLine.ICommandLineParsingContext, Orc.CommandLine.IContext
    {
        Catel.Data.ValidationContext ValidationContext { get; set; }
    }
    public interface IValidatedResult : Orc.CommandLine.ICommandLineParsingContext, Orc.CommandLine.IContext
    {
        Catel.Data.IValidationContext ValidationContext { get; set; }
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
        public System.Collections.Generic.IEnumerable<Orc.CommandLine.OptionDefinition> GetOptionDefinitions(Orc.CommandLine.IContext targetContext) { }
    }
    public abstract class ResultBase : Orc.CommandLine.ICommandLineParsingContext, Orc.CommandLine.IContext, Orc.CommandLine.IResult
    {
        protected ResultBase() { }
        public string CommandLine { get; set; }
        public bool IsHelp { get; set; }
        public string OriginalCommandLine { get; set; }
        public System.Collections.Generic.List<char> QuoteSplitCharacters { get; set; }
        public System.Collections.Generic.Dictionary<string, string> RawValues { get; set; }
        public Catel.Data.ValidationContext ValidationContext { get; set; }
        public void Finish() { }
    }
    public static class StringExtensions
    {
        public static bool IsHelp(this string singleArgument, char[] quoteSplitCharacters) { }
        public static bool IsSwitch(this string value, char[] quoteSplitCharacters) { }
        public static bool IsSwitch(this string switchName, string value, char[] quoteSplitCharacters) { }
        public static string TrimSwitchPrefix(this string value) { }
    }
}