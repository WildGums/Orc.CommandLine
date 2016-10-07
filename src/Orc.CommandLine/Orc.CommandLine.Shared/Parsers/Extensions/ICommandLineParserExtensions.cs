// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICommandLineParserExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CommandLine
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Catel;
    using Catel.Data;
    using Catel.IoC;

    public static class ICommandLineParserExtensions
    {
        #region Fields
        private static readonly Regex _regex = new Regex(@"(\""[^\""\\\-\/\s][\d\w\s\:\\.\-]+\"")|([^\""\\\s](\-?|\b)\w([\d\w]*[^\""\\\s])?)|((\-|\/)\?)|(\b\w[\d\w\:\\.]*\b)", RegexOptions.Compiled);
        #endregion

        #region Methods
        public static IValidationContext Parse(this ICommandLineParser commandLineParser, string commandLineArguments, IContext targetContext)
        {
            Argument.IsNotNull(() => commandLineParser);

            var splitted = _regex.Matches(commandLineArguments).Cast<Match>()
                .Select(m => m.Value)
                .ToList();

            return commandLineParser.Parse(splitted, targetContext);
        }

        public static IEnumerable<string> GetAppHeader(this ICommandLineParser commandLineParser)
        {
            Argument.IsNotNull(() => commandLineParser);

            var dependencyResolver = commandLineParser.GetDependencyResolver();

            var helpWriterService = dependencyResolver.Resolve<IHelpWriterService>();
            return helpWriterService.GetAppHeader();
        }

        public static IEnumerable<string> GetHelp(this ICommandLineParser commandLineParser, IContext targetContext)
        {
            Argument.IsNotNull(() => commandLineParser);

            var dependencyResolver = commandLineParser.GetDependencyResolver();

            var helpWriterService = dependencyResolver.Resolve<IHelpWriterService>();
            return helpWriterService.GetHelp(targetContext);
        }
        #endregion
    }
}