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
        [ObsoleteEx(ReplacementTypeOrMember = "IContext Parse(Type)", TreatAsErrorFromVersion = "4.0", RemoveInVersion = "5.0")]
        IValidationContext Parse(IContext targetContext);

        [ObsoleteEx(ReplacementTypeOrMember = "IContext Parse(Type, List<string>)", TreatAsErrorFromVersion = "4.0", RemoveInVersion = "5.0")]
        IValidationContext Parse(List<string> commandLineArguments, IContext targetContext);

        [ObsoleteEx(ReplacementTypeOrMember = "IContext Parse(Type, IEnumerable<string>)", TreatAsErrorFromVersion = "4.0", RemoveInVersion = "5.0")]
        IValidationContext Parse(IEnumerable<string> commandLineArguments, IContext targetContext);

        [ObsoleteEx(ReplacementTypeOrMember = "IContext Parse(Type, string)", TreatAsErrorFromVersion = "4.0", RemoveInVersion = "5.0")]
        IValidationContext Parse(string commandLine, IContext targetContext);

        IContext Parse(Type contextType);
        IContext Parse(Type contextType, List<string> commandLineArguments);
        IContext Parse(Type contextType, IEnumerable<string> commandLineArguments);
        IContext Parse(Type contextType, string commandLine);
    }
}
