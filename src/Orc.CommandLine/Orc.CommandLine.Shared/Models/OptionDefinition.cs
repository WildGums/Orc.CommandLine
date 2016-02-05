// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptionDefinition.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CommandLine
{
    public class OptionDefinition
    {
        public OptionDefinition()
        {
            AcceptsValue = true;
            TrimQuotes = true;
        }

        public char ShortName { get; set; }

        public string LongName { get; set; }

        public string DisplayName { get; set; }

        public string HelpText { get; set; }

        public string PropertyNameOnContext { get; set; }

        public bool IsMandatory { get; set; }

        public bool AcceptsValue { get; set; }

        public bool TrimQuotes { get; set; }

        public override string ToString()
        {
            var text = this.GetSwitchDisplay();
            return text;
        }
    }
}