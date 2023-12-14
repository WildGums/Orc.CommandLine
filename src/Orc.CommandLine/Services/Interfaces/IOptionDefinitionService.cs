namespace Orc.CommandLine;

using System.Collections.Generic;

public interface IOptionDefinitionService
{
    IEnumerable<OptionDefinition> GetOptionDefinitions(IContext targetContext);
}
