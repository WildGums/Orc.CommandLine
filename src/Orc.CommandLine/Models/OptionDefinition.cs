namespace Orc.CommandLine;

public class OptionDefinition
{
    public OptionDefinition()
    {
        ShortName = string.Empty;
        LongName = string.Empty;
        DisplayName = string.Empty;
        HelpText = string.Empty;
        PropertyNameOnContext = string.Empty;
        AcceptsValue = true;
        TrimQuotes = true;
        TrimWhiteSpace = false;
    }

    public string ShortName { get; set; }

    public string LongName { get; set; }

    public string DisplayName { get; set; }

    public string HelpText { get; set; }

    public string PropertyNameOnContext { get; set; }

    public bool IsMandatory { get; set; }

    public bool AcceptsValue { get; set; }

    public bool TrimQuotes { get; set; }

    public bool TrimWhiteSpace { get; set; }

    public override string ToString()
    {
        var text = this.GetSwitchDisplay();
        return text;
    }
}
