namespace Orc.CommandLine;

using System.Collections.Generic;
using Catel.Data;

public interface ICommandLineParser
{
    IValidationContext Parse(IContext targetContext);
    IValidationContext Parse(List<string> commandLineArguments, IContext targetContext);
    IValidationContext Parse(IEnumerable<string> commandLineArguments, IContext targetContext);
    IValidationContext Parse(string commandLine, IContext targetContext);
}
