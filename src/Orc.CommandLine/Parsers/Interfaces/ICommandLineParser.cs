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
        [ObsoleteEx(TreatAsErrorFromVersion = "4.2.0",
                  RemoveInVersion = "5.0.0",
                  ReplacementTypeOrMember = "Parse<T>(ICommandLineParsingContext commandLineParsingContext)")]
        IValidationContext Parse(IContext targetContext);
        [ObsoleteEx(TreatAsErrorFromVersion = "4.2.0",
                  RemoveInVersion = "5.0.0",
                  ReplacementTypeOrMember = "Parse<T>(ICommandLineParsingContext commandLineParsingContext)")]
        IValidationContext Parse(List<string> commandLineArguments, IContext targetContext);
        [ObsoleteEx(TreatAsErrorFromVersion = "4.2.0",
                  RemoveInVersion = "5.0.0",
                  ReplacementTypeOrMember = "Parse<T>(ICommandLineParsingContext commandLineParsingContext)")]
        IValidationContext Parse(IEnumerable<string> commandLineArguments, IContext targetContext);
        [ObsoleteEx(TreatAsErrorFromVersion = "4.2.0",
                  RemoveInVersion = "5.0.0",
                  ReplacementTypeOrMember = "Parse<T>(ICommandLineParsingContext commandLineParsingContext)")]
        IValidationContext Parse(string commandLine, IContext targetContext);
        T Parse<T>(ICommandLineParsingContext commandLineParsingContext) where T : IResult;
    }

}
