namespace Orc.CommandLine
{
    using System.Collections.Generic;
    using Catel.Data;

    public abstract class ResultBase : IResult
    {
        public string OriginalCommandLine { get; set; }

        public bool IsHelp { get; set; }

        public Dictionary<string, string> RawValues { get; set; }

        public ValidationContext ValidationContext { get; set; }

        public string CommandLine { get; set; }

        public List<char> QuoteSplitCharacters { get; set; }

        public void Finish()
        {
        }
    }
}
