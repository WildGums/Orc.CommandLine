namespace Orc.CommandLine;

using System;
using System.Collections.Generic;
using Catel.Reflection;

public class OptionDefinitionService : IOptionDefinitionService
{
    public IEnumerable<OptionDefinition> GetOptionDefinitions(IContext targetContext)
    {
        ArgumentNullException.ThrowIfNull(targetContext);

        var optionDefinitions = new List<OptionDefinition>();

        var properties = targetContext.GetType().GetPropertiesEx();
        foreach (var propertyInfo in properties)
        {
            if (propertyInfo.TryGetAttribute<OptionAttribute>(out var optionAttribute))
            {
                optionDefinitions.Add(new OptionDefinition
                {
                    ShortName = optionAttribute.ShortName,
                    LongName = optionAttribute.LongName,
                    DisplayName = optionAttribute.DisplayName,
                    HelpText = optionAttribute.HelpText,
                    AcceptsValue = optionAttribute.AcceptsValue,
                    TrimQuotes = optionAttribute.TrimQuotes,
                    TrimWhiteSpace = optionAttribute.TrimWhiteSpace,
                    IsMandatory = optionAttribute.IsMandatory,
                    PropertyNameOnContext = propertyInfo.Name
                });
            }
        }

        return optionDefinitions;
    }
}
