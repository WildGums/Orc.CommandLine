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

    public abstract class ContextBase : IContext, IResult
    {
        protected ContextBase()
        {
            RawValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            QuoteSplitCharacters = new List<char>(new [] { '\"', '\'' });
        }

        #region Properties
        public string OriginalCommandLine { get; set; }

        public bool IsHelp { get; set; }

        public Dictionary<string, string> RawValues { get; private set; }

        public List<char> QuoteSplitCharacters { get; }

        public string CommandLine { get; set; }

        public ValidationContext ValidationContext { get; set; }
        #endregion

        #region Methods
        public virtual void Finish()
        {
        }
        #endregion
    }
}
