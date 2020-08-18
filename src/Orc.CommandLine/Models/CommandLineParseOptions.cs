namespace Orc.CommandLine
{
    using System.Collections.Generic;

    public class CommandLineParseOptions
    {
        public CommandLineParseOptions()
        {
            QuoteSplitCharacters = new List<char>(new[] { '\"', '\'' });
        }

        public List<char> QuoteSplitCharacters { get; }
    }
}
