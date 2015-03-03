// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IArgumentParserExtensions.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CommandLine
{
    using System;
    using System.Linq;
    using Catel.Data;

    public static class ICommandLineParserExtensions
    {
        public static IValidationContext Parse(this ICommandLineParser commandLineParser, string commandLineArguments, IContext targetContext)
        {
            return commandLineParser.Parse(commandLineArguments.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList(), targetContext);
        }
    }
}