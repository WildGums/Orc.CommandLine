﻿namespace Orc.CommandLine.Tests.Models;

using NUnit.Framework;

[TestFixture]
public class OptionDefinitionExtensionsFacts
{
    [TestCase(" ", "", false)]
    [TestCase("b", "bool", true)]
    public void TheHasSwitchMethod(string shortName, string longName, bool expectedValue)
    {
        var optionDefinition = new OptionDefinition
        {
            ShortName = shortName,
            LongName = longName
        };

        Assert.That(optionDefinition.HasSwitch(), Is.EqualTo(expectedValue));
    }

    [TestCase("b", "bool", "-b", true)]
    [TestCase("b", "bool", "-bool", true)]
    [TestCase("b", "bool", "/b", true)]
    [TestCase("b", "bool", "/bool", true)]
    [TestCase("b", "bool", "b", false)]
    [TestCase("b", "bool", "bool", false)]
    public void TheIsSwitchMethod(string shortName, string longName, string actualSwitch, bool expectedValue)
    {
        var optionDefinition = new OptionDefinition
        {
            ShortName = shortName,
            LongName = longName
        };

        Assert.That(optionDefinition.IsSwitch(actualSwitch, new[] { '\"', '\'' }), Is.EqualTo(expectedValue));
    }

    [TestCase("", "", "", "[no switch]")]
    [TestCase("", "", "fileName", "fileName")]
    [TestCase("b", "bool", "", "b / bool")]
    public void TheGetSwitchDisplayMethod(string shortName, string longName, string displayName, string expectedString)
    {
        var optionDefinition = new OptionDefinition
        {
            ShortName = shortName,
            LongName = longName,
            DisplayName = displayName
        };

        var actual = optionDefinition.GetSwitchDisplay();

        Assert.That(actual, Is.EqualTo(expectedString));
    }
}
