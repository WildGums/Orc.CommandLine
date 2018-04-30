// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptionDefinitionExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CommandLine
{
    using Catel;
    using Catel.IoC;
    using Catel.Services;

    public static class OptionDefinitionExtensions
    {
        private static readonly ILanguageService LanguageService;

        static OptionDefinitionExtensions()
        {
            var serviceLocator = ServiceLocator.Default;

            LanguageService = serviceLocator.ResolveType<ILanguageService>();
        }

        public static bool HasSwitch(this OptionDefinition optionDefinition)
        {
            Argument.IsNotNull(() => optionDefinition);

            return !string.IsNullOrWhiteSpace(optionDefinition.ShortName.ToString());
        }

        public static bool IsSwitch(this OptionDefinition optionDefinition, string actualSwitch, char[] quoteSplitCharacters)
        {
            Argument.IsNotNull(() => optionDefinition);

            if (!actualSwitch.IsSwitch(quoteSplitCharacters))
            {
                return false;
            }

            if (optionDefinition.ShortName.ToString().IsSwitch(actualSwitch, quoteSplitCharacters))
            {
                return true;
            }

            if (optionDefinition.LongName.IsSwitch(actualSwitch, quoteSplitCharacters))
            {
                return true;
            }

            return false;
        }

        public static string GetSwitchDisplay(this OptionDefinition optionDefinition)
        {
            Argument.IsNotNull(() => optionDefinition);

            var text = optionDefinition.DisplayName;
            if (string.IsNullOrWhiteSpace(text))
            {
                var shortName = optionDefinition.ShortName.ToString().Trim();
                var longName = optionDefinition.LongName.Trim();

                var areEqual = string.Equals(shortName, longName);
                if (areEqual && string.IsNullOrWhiteSpace(longName))
                {
                    text = LanguageService.GetString("CommandLine_NoSwitch");
                }
                else
                {
                    text = string.Format("{0} / {1}", optionDefinition.ShortName, optionDefinition.LongName);
                }
            }

            return text;
        }
    }
}