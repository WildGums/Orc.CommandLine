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
        Assert.That(input.TrimSwitchPrefix(), Is.EqualTo(expectedValue));
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
        Assert.That(input.IsSwitch(new[] { '\"', '\'' }), Is.EqualTo(expectedValue));
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
        Assert.That(input.IsSwitch(switchValue, new [] { '\"', '\'' }), Is.EqualTo(expectedValue));
    }
}
