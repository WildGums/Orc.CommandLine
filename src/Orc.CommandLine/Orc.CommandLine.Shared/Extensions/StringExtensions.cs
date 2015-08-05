// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CommandLine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel;

    public static class StringExtensions
    {
        public static readonly List<string> AcceptedSwitchPrefixes = new List<string>(new[] {"-", "/"});

        public static string GetCommandLine(this string commandLine, bool removeFirstArgument)
        {
            Argument.IsNotNull(() => commandLine);

            var splittedCommandLine = commandLine.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (removeFirstArgument && splittedCommandLine.Count > 0)
            {
                splittedCommandLine = splittedCommandLine.Skip(1).ToList();
            }

            var finalCommandLine = string.Join(" ", splittedCommandLine.Select(x => x));
            return finalCommandLine;
        }

        public static bool IsSwitch(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            foreach (var acceptedSwitchPrefix in AcceptedSwitchPrefixes)
            {
                if (value.StartsWith(acceptedSwitchPrefix))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsSwitch(this string switchName, string value)
        {
            foreach (var acceptedSwitchPrefix in AcceptedSwitchPrefixes)
            {
                if (value.StartsWith(acceptedSwitchPrefix))
                {
                    value = value.Remove(0, acceptedSwitchPrefix.Length);
                }
            }

            return string.Equals(switchName, value);
        }

        public static bool IsHelp(this string singleArgument)
        {
            return IsSwitch("h", singleArgument) || IsSwitch("help", singleArgument) || IsSwitch("?", singleArgument);
        }
    }
}