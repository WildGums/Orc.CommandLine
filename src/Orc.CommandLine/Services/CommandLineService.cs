﻿namespace Orc.CommandLine;

using System;
using System.Linq;

public class CommandLineService : ICommandLineService
{
    public virtual string GetCommandLine()
    {
        var commandArguments = Environment.GetCommandLineArgs().Skip(1)
            .Select(x => x.Contains(' ') ? $"\'{x}\'" : x)
            .ToArray();

        var commandLine = string.Join(" ", commandArguments);

        return commandLine;
    }
}
