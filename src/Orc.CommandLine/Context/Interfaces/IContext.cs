// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IContext.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CommandLine
{
    public interface IContext
    {
        #region Properties
        string OriginalCommandLine { get; set; }

        bool IsHelp { get; set; }
        #endregion
    }
}