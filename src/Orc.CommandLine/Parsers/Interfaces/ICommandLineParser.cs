// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICommandLineParser.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CommandLine
{
    using System;
    using System.Collections.Generic;
    using Catel.Data;

    public interface ICommandLineParser
    {
        CommandLineParseOptions DefaultOptions { get; }

        ICommandLineContext Parse(Type contextType, CommandLineParseOptions options = null);
        ICommandLineContext Parse(Type contextType, List<string> commandLineArguments, CommandLineParseOptions options = null);
        ICommandLineContext Parse(Type contextType, IEnumerable<string> commandLineArguments, CommandLineParseOptions options = null);
        ICommandLineContext Parse(Type contextType, string commandLine, CommandLineParseOptions options = null);
    }
}
