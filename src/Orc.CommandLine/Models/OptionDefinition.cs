// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptionDefinition.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CommandLine
{
    public class OptionDefinition
    {
        public OptionDefinition()
        {
            AcceptsValue = true;
        }

        public char ShortName { get; set; }

        public string LongName { get; set; }

        public string HelpText { get; set; }

        public string PropertyNameOnContext { get; set; }

        public bool AcceptsValue { get; set; }
    }
}