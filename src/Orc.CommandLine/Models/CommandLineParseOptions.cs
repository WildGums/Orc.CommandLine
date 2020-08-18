namespace Orc.CommandLine
{
    using System.Collections.Generic;

    public class CommandLineParseOptions
    {
        public CommandLineParseOptions()
        {
            QuoteSplitCharacters = new List<char>(new[] { '\"', '\'' });
            LogMissingMandatoryOptionsAsErrors = true;
        }

        public List<char> QuoteSplitCharacters { get; }

        public bool LogMissingMandatoryOptionsAsErrors { get; set; }
    }
}
