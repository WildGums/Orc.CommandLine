// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HelpWriterService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CommandLine
{
    using System.Collections.Generic;
    using System.Linq;
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

        public IEnumerable<string> GetAppHeader()
        {
            var assembly = AssemblyHelper.GetEntryAssembly();

            var lines = new List<string>();

            lines.Add(string.Format("{0} v{1}", assembly.Title(), assembly.Version()));
            lines.Add("================");
            lines.Add(string.Empty);

            return lines;
        }

        public IEnumerable<string> GetHelp(IContext targetContext)
        {
            var lines = new List<string>();

            var optionDefinitions = _optionDefinitionService.GetOptionDefinitions(targetContext).ToList();

            var prefixLength = optionDefinitions.Select(x => x.ToString().Length).Max();

            foreach (var optionDefinition in optionDefinitions)
            {
                var prefix = optionDefinition.ToString();

                for (var i = prefix.Length; i < prefixLength; i++)
                {
                    prefix += " ";
                }

                var line = string.Format("{0} {1}", prefix, optionDefinition.HelpText);

                lines.Add(line);
            }

            return lines;
        }
    }
}