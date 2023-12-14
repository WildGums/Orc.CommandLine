namespace Orc.CommandLine;

using System.Linq;

public static class StringExtensions
{
    internal static readonly string[] AcceptedSwitchPrefixes = { "-", "/" };

    public static bool IsSwitch(this string value, char[] quoteSplitCharacters)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        value = value.Trim(quoteSplitCharacters);

        return AcceptedSwitchPrefixes.Any(acceptedSwitchPrefix => value.StartsWith(acceptedSwitchPrefix));
    }

    public static string TrimSwitchPrefix(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        foreach (var acceptedSwitchPrefix in AcceptedSwitchPrefixes)
        {
            if (value.StartsWith(acceptedSwitchPrefix))
            {
                value = value[acceptedSwitchPrefix.Length..];

                // Just trim 1
                break;
            }
        }

        return value;
    }

    public static bool IsSwitch(this string switchName, string value, char[] quoteSplitCharacters)
    {
        value = value.Trim(quoteSplitCharacters);

        foreach (var acceptedSwitchPrefix in AcceptedSwitchPrefixes)
        {
            if (value.StartsWith(acceptedSwitchPrefix))
            {
                value = value.Remove(0, acceptedSwitchPrefix.Length);
            }
        }

        return string.Equals(switchName, value);
    }

    public static bool IsHelp(this string singleArgument, char[] quoteSplitCharacters)
    {
        return IsSwitch("h", singleArgument, quoteSplitCharacters) ||
               IsSwitch("help", singleArgument, quoteSplitCharacters) ||
               IsSwitch("?", singleArgument, quoteSplitCharacters);
    }
}
