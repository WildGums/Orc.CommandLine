namespace Orc.CommandLine.Tests
{
    using NUnit.Framework;

    public partial class CommandLineParserFacts
    {
        private class SingleQuotesContext : ContextBase
        {
            [Option("", "project", DisplayName = "project", HelpText = "", TrimQuotes = true, TrimWhiteSpace = true)]
            public string Project { get; set; }

            [Option("", "extension", DisplayName = "extension", HelpText = "", TrimQuotes = true, TrimWhiteSpace = true)]
            public string Extension { get; set; }
        }

        [TestCase(@"-project 'D:\Data\Testing\My Project Path' -extension 'MyProjects.Plugins.MyPlugin'", @"D:\Data\Testing\My Project Path", "MyProjects.Plugins.MyPlugin")]
        public void SupportsSingleQuotes(string input, string expectedProject, string expectedExtension)
        {
            var commandLineParser = CreateCommandLineParser();

            var context = new SingleQuotesContext();
            var validationContext = commandLineParser.Parse(input, context);

            Assert.IsFalse(validationContext.HasErrors);
            Assert.IsFalse(validationContext.HasWarnings);

            Assert.AreEqual(expectedProject, context.Project);
            Assert.AreEqual(expectedExtension, context.Extension);
        }
    }
}
