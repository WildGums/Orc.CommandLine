namespace Orc.CommandLine;

using System.Collections.Generic;

public interface IHelpWriterService
{
    IEnumerable<string> GetAppHeader();
    IEnumerable<string> GetHelp(IContext targetContext);
}
