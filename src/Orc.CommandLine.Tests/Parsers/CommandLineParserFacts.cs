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

        Assert.That(validationContext.HasErrors, Is.False);
        Assert.That(validationContext.HasWarnings, Is.False);

        Assert.That(string.Equals(expectedBooleanSwitch, context.BooleanSwitch.ToString(), StringComparison.OrdinalIgnoreCase), Is.True);
        Assert.That(string.Equals(expectedIntegerSwitch, context.IntegerSwitch.ToString(), StringComparison.OrdinalIgnoreCase), Is.True);
        Assert.That(string.Equals(expectedStringSwitch, context.StringSwitch), Is.True);
        Assert.That(string.Equals(expectedFileName, context.FileName), Is.True);
    }

    [TestCase("-trimquotes \"bla\"", "bla", null)]
    [TestCase("-trimquotes \"bla bla\"", "bla bla", null)]
    [TestCase("-donttrimquotes \"bla\"", null, "\"bla\"")]
    public void CorrectlyHandlesQuoteTrimming(string input, string? expectedTrimQuotesValue, string? expectedDontTrimQuotesValue)
    {
        var commandLineParser = CreateCommandLineParser();

        var context = new Tests.Context.TestContext();
        var validationContext = commandLineParser.Parse(input, context);

        Assert.That(context.TrimQuotes, Is.EqualTo(expectedTrimQuotesValue));
        Assert.That(context.DontTrimQuotes, Is.EqualTo(expectedDontTrimQuotesValue));
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

        Assert.That(validationContext.HasErrors, Is.False);
        Assert.That(validationContext.HasWarnings, Is.False);

        Assert.That(context.IsHelp, Is.True);
    }

    [TestCase]
    public void ReturnsRawValuesForNonSpecifiedOptions()
    {
        var commandLineParser = CreateCommandLineParser();

        var context = new TestContextWithFile();
        var validationContext = commandLineParser.Parse("somefile /nonspecified /nonspecified2 somevalue /bs /s somestring /i 42", context);

        Assert.That(context.RawValues["NonSpecified"], Is.EqualTo(string.Empty));
        Assert.That(context.RawValues["NonSpecified2"], Is.EqualTo("somevalue"));
    }

    [TestCase]
    public void ReturnsValidationContextWithErrorsForMissingMandatoryOptions()
    {
        var commandLineParser = CreateCommandLineParser();

        var context = new TestContextWithMandatoryOption();
        var validationContext = commandLineParser.Parse(string.Empty, context);

        Assert.That(validationContext.HasErrors, Is.True);
    }

    [TestCase("-something 'some argument' -appcolor \"#FF483FFC\" -appname \"New name\" -headless")]
    public void ParsesBrandingContextWithAdditionalNonDefinedOptions(string commandLine)
    {
        var commandLineParser = CreateCommandLineParser();

        var context = new BrandingCommandLineContext();
        var validationContext = commandLineParser.Parse(commandLine, context);

        Assert.That(validationContext.HasErrors, Is.False);
    }
    #endregion
}
