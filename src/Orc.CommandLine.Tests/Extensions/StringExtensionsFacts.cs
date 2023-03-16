namespace Orc.CommandLine.Tests;

using NUnit.Framework;

[TestFixture]
public class StringExtensionsFacts
{
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
        Assert.AreEqual(expectedValue, input.IsSwitch(new[] { '\"', '\'' }));
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
        Assert.AreEqual(expectedValue, input.IsSwitch(switchValue, new [] { '\"', '\'' }));
    }
}
