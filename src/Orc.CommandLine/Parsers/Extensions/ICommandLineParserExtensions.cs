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
        #region Methods
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