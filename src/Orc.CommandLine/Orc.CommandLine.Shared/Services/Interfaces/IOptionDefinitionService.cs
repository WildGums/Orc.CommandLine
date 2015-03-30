// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOptionDefinitionService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CommandLine
{
    using System.Collections.Generic;

    public interface IOptionDefinitionService
    {
        #region Methods
        IEnumerable<OptionDefinition> GetOptionDefinitions(IContext targetContext);
        #endregion
    }
}