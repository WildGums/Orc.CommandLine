// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IContext.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CommandLine
{
    using System.Collections.Generic;

    public interface IContext
    {
        #region Properties
        string OriginalCommandLine { get; set; }

        bool IsHelp { get; set; }

        Dictionary<string, string> RawValues { get; }
        #endregion

        #region Methods
        void Finish();
        #endregion
    }
}