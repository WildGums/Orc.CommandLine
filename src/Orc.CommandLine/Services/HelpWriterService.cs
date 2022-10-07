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
            ArgumentNullException.ThrowIfNull(optionDefinitionService);

            _optionDefinitionService = optionDefinitionService;
        }

        public IEnumerable<string> GetAppHeader()
        {
            var assembly = AssemblyHelper.GetRequiredEntryAssembly();

            var lines = new List<string>
            {
                string.Format("{0} v{1}", assembly.Title(), assembly.Version()),
                "================",
                string.Empty
            };

            return lines;
        }

        public IEnumerable<string> GetHelp(IContext targetContext)
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
