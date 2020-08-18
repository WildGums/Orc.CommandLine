namespace Orc.CommandLine
{
    using System.Collections.Generic;
    using Catel.Data;

    public interface IContext
    {
        string OriginalCommandLine { get; set; }

        bool IsHelp { get; set; }

        Dictionary<string, string> RawValues { get; }

        List<char> QuoteSplitCharacters { get; }

        IValidationContext ValidationContext { get; }

        void Finish();
    }
}
