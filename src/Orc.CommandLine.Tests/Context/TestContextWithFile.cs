namespace Orc.CommandLine.Tests.Context
{
    public class TestContextWithFile : TestContext
    {
        public TestContextWithFile()
        {
            FileName = string.Empty;
        }

        [Option("", "", DisplayName = "fileName", HelpText = "The file name to start with", TrimQuotes = true, TrimWhiteSpace = true)]
        public string FileName { get; set; }
    }
}
