﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOptionDefinitionService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CommandLine
{
    using System.Collections.Generic;

    public interface IOptionDefinitionService
    {
        IEnumerable<OptionDefinition> GetOptionDefinitions(ICommandLineContext targetContext);
    }
}
