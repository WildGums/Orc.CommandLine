// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICommandLineParser.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CommandLine
{
    using System.Collections.Generic;
    using Catel.Data;

    public interface ICommandLineParser
    {
        IValidationContext Parse(List<string> commandLineArguments, IContext targetContext);
    }
}