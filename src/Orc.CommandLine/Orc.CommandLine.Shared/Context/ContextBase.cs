// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextBase.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CommandLine
{
    public abstract class ContextBase : IContext
    {
        #region Properties
        public string OriginalCommandLine { get; set; }
        public bool IsHelp { get; set; }
        #endregion

        #region Methods
        public virtual void Finish()
        {
        }
        #endregion
    }
}