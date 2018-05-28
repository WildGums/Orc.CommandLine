// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptionAttribute.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CommandLine
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public class OptionAttribute : Attribute
    {
        [ObsoleteEx(Message = "Use string overload instead so multiple characters can be used for the short name", TreatAsErrorFromVersion = "3.0", RemoveInVersion = "4.0")]
        public OptionAttribute(char shortName, string longName)
            : this(shortName.ToString(), longName)
        {
        }

        public OptionAttribute(string shortName, string longName)
        {
            ShortName = shortName;
            LongName = longName;
            AcceptsValue = true;
            TrimQuotes = true;
            TrimWhiteSpace = false;
            IsMandatory = false;
        }

        public string ShortName { get; private set; }

        public string LongName { get; private set; }

        public string DisplayName { get; set; }

        public string HelpText { get; set; }

        public bool AcceptsValue { get; set; }

        public bool TrimQuotes { get; set; }

        public bool TrimWhiteSpace { get; set; }

        public bool IsMandatory { get; set; }
    }
}