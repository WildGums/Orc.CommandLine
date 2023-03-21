namespace Orc.CommandLine;

using System;

[AttributeUsage(AttributeTargets.Property)]
public class OptionAttribute : Attribute
{
    public OptionAttribute(string shortName, string longName)
    {
        ShortName = shortName;
        LongName = longName;
        DisplayName = string.Empty;
        HelpText = string.Empty;
        AcceptsValue = true;
        TrimQuotes = true;
        TrimWhiteSpace = false;
        IsMandatory = false;
    }

    public string ShortName { get; }

    public string LongName { get; }

    public string DisplayName { get; set; }

    public string HelpText { get; set; }

    public bool AcceptsValue { get; set; }

    public bool TrimQuotes { get; set; }

    public bool TrimWhiteSpace { get; set; }

    public bool IsMandatory { get; set; }
}
