namespace Orc.CommandLine
{
    using System;
    using System.Collections.Generic;
    using Catel.IoC;

    public static class ICommandLineParserExtensions
    {
        public static IEnumerable<string> GetAppHeader(this ICommandLineParser commandLineParser)
        {
            ArgumentNullException.ThrowIfNull(commandLineParser);

            var dependencyResolver = commandLineParser.GetDependencyResolver();

            var helpWriterService = dependencyResolver.ResolveRequired<IHelpWriterService>();
            return helpWriterService.GetAppHeader();
        }

        public static IEnumerable<string> GetHelp(this ICommandLineParser commandLineParser, IContext targetContext)
        {
            ArgumentNullException.ThrowIfNull(commandLineParser);

            var dependencyResolver = commandLineParser.GetDependencyResolver();

            var helpWriterService = dependencyResolver.ResolveRequired<IHelpWriterService>();
            return helpWriterService.GetHelp(targetContext);
        }
    }
}
