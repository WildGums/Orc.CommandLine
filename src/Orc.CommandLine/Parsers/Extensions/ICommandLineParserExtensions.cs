// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IArgumentParserExtensions.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CommandLine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel;
    using Catel.Data;
    using Catel.IoC;

    public static class ICommandLineParserExtensions
    {
        public static IValidationContext Parse(this ICommandLineParser commandLineParser, string commandLineArguments, IContext targetContext)
        {
            Argument.IsNotNull(() => commandLineParser);

            return commandLineParser.Parse(commandLineArguments.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList(), targetContext);
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
    }
}