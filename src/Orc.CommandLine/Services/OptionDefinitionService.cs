// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptionDefinitionService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CommandLine
{
    using System.Collections.Generic;
    using Catel;
    using Catel.Reflection;

    public class OptionDefinitionService : IOptionDefinitionService
    {
        public IEnumerable<OptionDefinition> GetOptionDefinitions(ICommandLineContext targetContext)
        {
            Argument.IsNotNull(() => targetContext);

            var optionDefinitions = new List<OptionDefinition>();

            var properties = targetContext.GetType().GetPropertiesEx();
            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.IsDecoratedWithAttribute<OptionAttribute>())
                {
                    var optionAttribute = (OptionAttribute) propertyInfo.GetCustomAttributeEx(typeof (OptionAttribute), true);

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
}
