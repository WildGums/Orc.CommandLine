namespace Orc.CommandLine
{
    using System.Collections.Generic;

    public interface ICommandLineParsingContext
    {
        string CommandLine { get; set; }
        List<char> QuoteSplitCharacters { get; }
    }
}
