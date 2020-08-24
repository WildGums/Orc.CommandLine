// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HelpWriterService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CommandLine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Catel;
    using Catel.Reflection;

    public class HelpWriterService : IHelpWriterService
    {
        private readonly IOptionDefinitionService _optionDefinitionService;

        public HelpWriterService(IOptionDefinitionService optionDefinitionService)
        {
            Argument.IsNotNull(() => optionDefinitionService);

            _optionDefinitionService = optionDefinitionService;
        }

        public IEnumerable<string> GetVersion()
        {
            var assembly = AssemblyHelper.GetEntryAssembly();

            var lines = new List<string>();

            lines.Add($"{assembly.Title()} v{assembly.Version()}");
            lines.Add(assembly.Copyright());
            lines.Add(string.Empty);

            return lines;
        }

        public IEnumerable<string> GetHelp(ICommandLineContext targetContext)
        {
            var optionDefinitions = _optionDefinitionService.GetOptionDefinitions(targetContext).ToList();
            if (optionDefinitions.Count == 0)
            {
                return Array.Empty<string>();
            }

            var lines = new List<string>();

            var prefixLength = optionDefinitions.Select(x => x.ToString().Length).Max();

            foreach (var optionDefinition in optionDefinitions)
            {
                var prefix = optionDefinition.ToString();

                var stringBuilder = new StringBuilder();
                stringBuilder.Append(prefix);

                for (var i = prefix.Length; i < prefixLength; i++)
                {
                    stringBuilder.Append(" ");
                }

                stringBuilder.Append($" {optionDefinition.HelpText}");
                lines.Add(stringBuilder.ToString());
            }

            return lines;
        }
    }
}
