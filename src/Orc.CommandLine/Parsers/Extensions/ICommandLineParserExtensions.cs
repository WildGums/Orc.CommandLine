// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICommandLineParserExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2018 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CommandLine
{
    using System.Collections.Generic;
    using Catel;
    using Catel.IoC;

    public static class ICommandLineParserExtensions
    {
        public static IEnumerable<string> GetVersion(this ICommandLineParser commandLineParser)
        {
            Argument.IsNotNull(() => commandLineParser);

            var dependencyResolver = commandLineParser.GetDependencyResolver();

            var helpWriterService = dependencyResolver.Resolve<IHelpWriterService>();
            return helpWriterService.GetVersion();
        }

        public static IEnumerable<string> GetHelp(this ICommandLineParser commandLineParser, ICommandLineContext targetContext)
        {
            Argument.IsNotNull(() => commandLineParser);

            var dependencyResolver = commandLineParser.GetDependencyResolver();

            var helpWriterService = dependencyResolver.Resolve<IHelpWriterService>();
            return helpWriterService.GetHelp(targetContext);
        }

        public static TContext Parse<TContext>(this ICommandLineParser commandLineParser)
            where TContext : ICommandLineContext
        {
            return (TContext)commandLineParser.Parse(typeof(TContext));
        }

        public static TContext Parse<TContext>(this ICommandLineParser commandLineParser, List<string> commandLineArguments)
            where TContext : ICommandLineContext
        {
            return (TContext)commandLineParser.Parse(typeof(TContext), commandLineArguments);
        }

        public static TContext Parse<TContext>(this ICommandLineParser commandLineParser, IEnumerable<string> commandLineArguments)
            where TContext : ICommandLineContext
        {
            return (TContext)commandLineParser.Parse(typeof(TContext), commandLineArguments);
        }

        public static TContext Parse<TContext>(this ICommandLineParser commandLineParser, string commandLine)
            where TContext : ICommandLineContext
        {
            return (TContext)commandLineParser.Parse(typeof(TContext), commandLine);
        }
    }
}
