namespace Orc.CommandLine
{
    using System.Collections.Generic;
    using Catel.Data;

    public interface ICommandLineContext
    {
        string OriginalCommandLine { get; set; }

        bool IsHelp { get; set; }

        Dictionary<string, string> RawValues { get; }

        IValidationContext ValidationContext { get; }

        void Finish();
    }
}
