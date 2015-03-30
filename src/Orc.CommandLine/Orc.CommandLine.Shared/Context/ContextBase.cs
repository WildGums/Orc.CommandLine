// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextBase.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
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