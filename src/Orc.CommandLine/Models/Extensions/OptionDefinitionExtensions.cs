// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptionDefinitionExtensions.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CommandLine
{
    using Catel;

    public static class OptionDefinitionExtensions
    {
        public static bool HasSwitch(this OptionDefinition optionDefinition)
        {
            Argument.IsNotNull(() => optionDefinition);

            return !string.IsNullOrWhiteSpace(optionDefinition.ShortName.ToString());
        }

        public static bool IsSwitch(this OptionDefinition optionDefinition, string actualSwitch)
        {
            Argument.IsNotNull(() => optionDefinition);

            if (!actualSwitch.IsSwitch())
            {
                return false;
            }

            if (optionDefinition.ShortName.ToString().IsSwitch(actualSwitch))
            {
                return true;
            }

            if (optionDefinition.LongName.IsSwitch(actualSwitch))
            {
                return true;
            }

            return false;
        }
    }
}