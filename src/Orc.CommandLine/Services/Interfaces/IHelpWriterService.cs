// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHelpWriterService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CommandLine
{
    using System.Collections.Generic;

    public interface IHelpWriterService
    {
        #region Methods
        string ConvertToString(IEnumerable<OptionDefinition> optionDefinitions);
        #endregion
    }
}