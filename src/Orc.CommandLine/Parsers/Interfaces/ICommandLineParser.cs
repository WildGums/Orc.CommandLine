// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICommandLineParser.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CommandLine
{
    using System.Collections.Generic;
    using Catel.Data;

    public interface ICommandLineParser
    {
        IValidationContext Parse(IContext targetContext);
        IValidationContext Parse(List<string> commandLineArguments, IContext targetContext);
        IValidationContext Parse(IEnumerable<string> commandLineArguments, IContext targetContext);
        IValidationContext Parse(string commandLine, IContext targetContext);
        T Parse<T>(ICommandLineParsingContext commandLineParsingContext) where T : IResult;
    }

}
