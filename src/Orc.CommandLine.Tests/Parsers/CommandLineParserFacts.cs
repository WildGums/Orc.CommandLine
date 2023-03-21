namespace Orc.CommandLine.Tests;

using System;
using Catel.IoC;
using Context;
using NUnit.Framework;

[TestFixture]
public partial class CommandLineParserFacts
{
    #region Methods
    private ICommandLineParser CreateCommandLineParser()
    {
        var serviceLocator = ServiceLocator.Default;
#pragma warning disable IDISP001 // Dispose created
        var typeFactory = serviceLocator.ResolveType<ITypeFactory>();
#pragma warning restore IDISP001 // Dispose created

        return typeFactory.CreateInstanceWithParametersAndAutoCompletion<CommandLineParser>(new OptionDefinitionService());
    }

    [TestCase("", "false", "0", "", "")]
    [TestCase("somefile", "false", "0", "", "somefile")]
    [TestCase("somefile /bs /s somestring /i 42", "true", "42", "somestring", "somefile")]
    [TestCase("C:\\folder\\file.txt /bs /s somestring /i 42", "true", "42", "somestring", "C:\\folder\\file.txt")]
    [TestCase("\"C:\\some folder\\file.txt\" /bs /s somestring /i 42", "true", "42", "somestring", "C:\\some folder\\file.txt")]
    [TestCase("\"C:\\some folder\\file.txt\" \"/bs\" \"/s\" somestring \"/i\" 42", "true", "42", "somestring", "C:\\some folder\\file.txt")]
    [TestCase("\"  C:\\some folder\\file.txt  \" \"/bs\" \"/s\" somestring \"/i\" 42", "true", "42", "somestring", "C:\\some folder\\file.txt")]
    [TestCase("'C:\\some folder\\file.txt' '/bs' '/s' somestring '/i' 42", "true", "42", "somestring", "C:\\some folder\\file.txt")]
    [TestCase("\"C:\\some folder\\file.txt\" \"-bs\" \"-s\" somestring \"-i\" 42", "true", "42", "somestring", "C:\\some folder\\file.txt")]
    [TestCase("\"C:\\some - folder\\file.txt\" \"-bs\" \"-s\" somestring \"-i\" 42", "true", "42", "somestring", "C:\\some - folder\\file.txt")]
    [TestCase("\"some file\" /bs /s somestring /i 42", "true", "42", "somestring", "some file")]
    [TestCase("/bs /s somestring /i 42", "true", "42", "somestring", "")]
    [TestCase("/bs /s \" some string \" /i 42", "true", "42", " some string ", "")]
    public void CorrectlyParsesCommandLinesWithFile(string input, string expectedBooleanSwitch, string expectedIntegerSwitch,
        string expectedStringSwitch, string expectedFileName)
    {
        var commandLineParser = CreateCommandLineParser();

        var context = new TestContextWithFile();
        var validationContext = commandLineParser.Parse(input, context);

        Assert.IsFalse(validationContext.HasErrors);
        Assert.IsFalse(validationContext.HasWarnings);

        Assert.IsTrue(string.Equals(expectedBooleanSwitch, context.BooleanSwitch.ToString(), StringComparison.OrdinalIgnoreCase));
        Assert.IsTrue(string.Equals(expectedIntegerSwitch, context.IntegerSwitch.ToString(), StringComparison.OrdinalIgnoreCase));
        Assert.IsTrue(string.Equals(expectedStringSwitch, context.StringSwitch));
        Assert.IsTrue(string.Equals(expectedFileName, context.FileName));
    }

    [TestCase("-trimquotes \"bla\"", "bla", null)]
    [TestCase("-trimquotes \"bla bla\"", "bla bla", null)]
    [TestCase("-donttrimquotes \"bla\"", null, "\"bla\"")]
    public void CorrectlyHandlesQuoteTrimming(string input, string expectedTrimQuotesValue, string expectedDontTrimQuotesValue)
    {
        var commandLineParser = CreateCommandLineParser();

        var context = new Tests.Context.TestContext();
        var validationContext = commandLineParser.Parse(input, context);

        Assert.AreEqual(expectedTrimQuotesValue, context.TrimQuotes);
        Assert.AreEqual(expectedDontTrimQuotesValue, context.DontTrimQuotes);
    }

    [TestCase("-h")]
    [TestCase("/h")]
    [TestCase("-help")]
    [TestCase("/help")]
    [TestCase("-?")]
    [TestCase("/?")]
    [TestCase("somefile -h")]
    [TestCase("somefile /bs /s somestring /i 42 /help")]
    [TestCase("somefile /bs /s somestring /i 42 -?")]
    public void CorrectlyParsesCommandLineWithHelp(string input)
    {
        var commandLineParser = CreateCommandLineParser();

        var context = new TestContextWithFile();
        var validationContext = commandLineParser.Parse(input, context);

        Assert.IsFalse(validationContext.HasErrors);
        Assert.IsFalse(validationContext.HasWarnings);

        Assert.IsTrue(context.IsHelp);
    }

    [TestCase]
    public void ReturnsRawValuesForNonSpecifiedOptions()
    {
        var commandLineParser = CreateCommandLineParser();

        var context = new TestContextWithFile();
        var validationContext = commandLineParser.Parse("somefile /nonspecified /nonspecified2 somevalue /bs /s somestring /i 42", context);

        Assert.AreEqual(string.Empty, context.RawValues["NonSpecified"]);
        Assert.AreEqual("somevalue", context.RawValues["NonSpecified2"]);
    }

    [TestCase]
    public void ReturnsValidationContextWithErrorsForMissingMandatoryOptions()
    {
        var commandLineParser = CreateCommandLineParser();

        var context = new TestContextWithMandatoryOption();
        var validationContext = commandLineParser.Parse(string.Empty, context);

        Assert.IsTrue(validationContext.HasErrors);
    }

    [TestCase("-something 'some argument' -appcolor \"#FF483FFC\" -appname \"New name\" -headless")]
    public void ParsesBrandingContextWithAdditionalNonDefinedOptions(string commandLine)
    {
        var commandLineParser = CreateCommandLineParser();

        var context = new BrandingCommandLineContext();
        var validationContext = commandLineParser.Parse(commandLine, context);

        Assert.IsFalse(validationContext.HasErrors);
    }
    #endregion
}
