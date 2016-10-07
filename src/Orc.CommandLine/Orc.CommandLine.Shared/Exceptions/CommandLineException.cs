// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandLineException.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CommandLine
{
    using System;

    public class CommandLineException : Exception
    {
        public CommandLineException(string message)
            : base(message)
        {
        }

        public CommandLineException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}