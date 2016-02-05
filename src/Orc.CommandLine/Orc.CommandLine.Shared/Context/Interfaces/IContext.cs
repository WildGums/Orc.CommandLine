// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IContext.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
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

        #region Methods
        void Finish();
        #endregion
    }
}