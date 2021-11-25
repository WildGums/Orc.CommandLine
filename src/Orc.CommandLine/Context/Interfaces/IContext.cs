namespace Orc.CommandLine
{
    using System.Collections.Generic;

    public interface IContext : ICommandLineParsingContext
    {
        #region Properties
        string OriginalCommandLine { get; set; }

        bool IsHelp { get; set; }

        Dictionary<string, string> RawValues { get; }
        #endregion

        #region Methods
        void Finish();
        #endregion
    }
}
