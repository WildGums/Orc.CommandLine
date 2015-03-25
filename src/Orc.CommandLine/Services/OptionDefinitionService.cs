// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptionDefinitionService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CommandLine
{
    using System.Collections.Generic;
    using Catel;
    using Catel.Reflection;

    public class OptionDefinitionService : IOptionDefinitionService
    {
        #region Methods
        public IEnumerable<OptionDefinition> GetOptionDefinitions(IContext targetContext)
        {
            Argument.IsNotNull(() => targetContext);

            var optionDefinitions = new List<OptionDefinition>();

            var properties = targetContext.GetType().GetPropertiesEx();
            foreach (var propertyInfo in properties)
            {
                if (AttributeHelper.IsDecoratedWithAttribute<OptionAttribute>(propertyInfo))
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
                        IsMandatory = optionAttribute.IsMandatory,
                        PropertyNameOnContext = propertyInfo.Name
                    });
                }
            }

            return optionDefinitions;
        }
        #endregion
    }
}