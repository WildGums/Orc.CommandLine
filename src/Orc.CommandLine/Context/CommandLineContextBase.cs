// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextBase.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CommandLine
{
    using System;
    using System.Collections.Generic;
    using Catel.Data;

    public abstract class CommandLineContextBase : ICommandLineContext
    {
        protected CommandLineContextBase()
        {
            RawValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            ValidationContext = new ValidationContext();
        }

        #region Properties
        public string OriginalCommandLine { get; set; }
        public bool IsHelp { get; set; }
        public Dictionary<string, string> RawValues { get; private set; }

        public IValidationContext ValidationContext { get; private set; }
        #endregion

        #region Methods
        public virtual void Finish()
        {
        }
        #endregion
    }
}
