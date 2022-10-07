namespace Orc.CommandLine
{
    using System;
    using System.Collections.Generic;

    public abstract class ContextBase : IContext
    {
        protected ContextBase()
        {
            OriginalCommandLine = string.Empty;
            RawValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            QuoteSplitCharacters = new List<char>(new [] { '\"', '\'' });
        }

        public string OriginalCommandLine { get; set; }
        public bool IsHelp { get; set; }
        public Dictionary<string, string> RawValues { get; private set; }
        public List<char> QuoteSplitCharacters { get; }

        public virtual void Finish()
        {
        }
    }
}
