// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptionDefinitionExtensionsFacts.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CommandLine.Tests.Models
{
    using NUnit.Framework;

    [TestFixture]
    public class OptionDefinitionExtensionsFacts
    {
        [TestCase(' ', "", false)]
        [TestCase('b', "bool", true)]
        public void TheHasSwitchMethod(char shortName, string longName, bool expectedValue)
        {
            var optionDefinition = new OptionDefinition
            {
                ShortName = shortName,
                LongName = longName
            };

            Assert.AreEqual(expectedValue, optionDefinition.HasSwitch());
        }

        [TestCase('b', "bool", "-b", true)]
        [TestCase('b', "bool", "-bool", true)]
        [TestCase('b', "bool", "/b", true)]
        [TestCase('b', "bool", "/bool", true)]
        [TestCase('b', "bool", "b", false)]
        [TestCase('b', "bool", "bool", false)]
        public void TheIsSwitchMethod(char shortName, string longName, string actualSwitch, bool expectedValue)
        {
            var optionDefinition = new OptionDefinition
            {
                ShortName = shortName,
                LongName = longName
            };

            Assert.AreEqual(expectedValue, optionDefinition.IsSwitch(actualSwitch));
        }
    }
}