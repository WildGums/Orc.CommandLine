namespace Orc.CommandLine;

using System.Collections.Generic;

public interface IContext
{
    string OriginalCommandLine { get; set; }

    bool IsHelp { get; set; }

    Dictionary<string, string> RawValues { get; }

    List<char> QuoteSplitCharacters { get; }

    void Finish();
}
