// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HelpWriterService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CommandLine
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class HelpWriterService : IHelpWriterService
    {
        public string ConvertToString(IEnumerable<OptionDefinition> optionDefinitions)
        {
            var stringBuilder = new StringBuilder();

            var prefixLength = optionDefinitions.Select(optionDefinition => GetDefinitionPrefix(optionDefinition).Length).Max();

            foreach (var optionDefinition in optionDefinitions)
            {
                var prefix = GetDefinitionPrefix(optionDefinition);

                for (var i = prefix.Length; i < prefixLength; i++)
                {
                    prefix += " ";
                }

                var line = string.Format("{0} {1}", prefix, optionDefinition.HelpText);

                stringBuilder.AppendLine(line);
            }

            return stringBuilder.ToString();
        }

        private string GetDefinitionPrefix(OptionDefinition optionDefinition)
        {
            var shortName = optionDefinition.ShortName.ToString();
            var longName = optionDefinition.LongName;

            var areEqual = string.Equals(shortName, longName);

            if (areEqual && string.IsNullOrWhiteSpace(shortName))
            {
                return "[no switch]";
            }

            var prefix = string.Format("/{0}", shortName);
            if (areEqual)
            {
                prefix += string.Format(" /{0}", longName);
            }

            return prefix;
        }
    }
}