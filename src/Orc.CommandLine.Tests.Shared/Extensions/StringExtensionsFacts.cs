// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensionsFacts.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CommandLine.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class StringExtensionsFacts
    {
        [TestCase("", false, "")]
        [TestCase("", true, "")]
        [TestCase("\"MyApplication.exe\" -a test", false, "\"MyApplication.exe\" -a test")]
        [TestCase("\"MyApplication.exe\" -a test", true, "-a test")]
        public void TheGetCommandLineMethod(string input, bool removeFirstArgument, string expectedValue)
        {
            Assert.AreEqual(expectedValue, input.GetCommandLine(removeFirstArgument));
        }

        [TestCase("", "")]
        [TestCase("a", "a")]
        [TestCase("ab", "ab")]
        [TestCase("-a", "a")]
        [TestCase("-ab", "ab")]
        [TestCase("/a", "a")]
        [TestCase("/ab", "ab")]
        [TestCase("//ab", "/ab")]
        public void TheTrimSwitchPrefixMethod(string input, string expectedValue)
        {
            Assert.AreEqual(expectedValue, input.TrimSwitchPrefix());
        }

        [TestCase("", false)]
        [TestCase("a", false)]
        [TestCase("ab", false)]
        [TestCase("-a", true)]
        [TestCase("-ab", true)]
        [TestCase("/a", true)]
        [TestCase("/ab", true)]
        public void TheIsSwitch1Method(string input, bool expectedValue)
        {
            Assert.AreEqual(expectedValue, input.IsSwitch());
        }

        [TestCase("", "-a", false)]
        [TestCase("a", "-aa", false)]
        [TestCase("aa", "-a", false)]
        [TestCase("a", "-a", true)]
        [TestCase("abc", "-abc", true)]
        [TestCase("", "/a", false)]
        [TestCase("a", "/aa", false)]
        [TestCase("aa", "/a", false)]
        [TestCase("a", "/a", true)]
        [TestCase("abc", "/abc", true)]
        public void TheIsSwitch2Method(string input, string switchValue, bool expectedValue)
        {
            Assert.AreEqual(expectedValue, input.IsSwitch(switchValue));
        }
    }
}