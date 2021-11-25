namespace Orc.CommandLine
{
    using System.Collections.Generic;
    public class CommandLineParsingContextBase : ICommandLineParsingContext
    {
        public CommandLineParsingContextBase(string commandLine)
        {
            QuoteSplitCharacters = new List<char>(new[] { '\"', '\'' });
            CommandLine = commandLine;
        }

        public CommandLineParsingContextBase()
        {
        }

        public List<char> QuoteSplitCharacters { get; }

        public string CommandLine { get; set; }
    }
}
