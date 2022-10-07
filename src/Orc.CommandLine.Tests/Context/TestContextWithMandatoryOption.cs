namespace Orc.CommandLine.Tests.Context
{
    public class TestContextWithMandatoryOption : TestContext
    {
        public TestContextWithMandatoryOption()
        {
            RequiredStringSwitch = string.Empty;
        }

        [Option("r", "required", IsMandatory = true, HelpText = "Some required string switch")]
        public string RequiredStringSwitch { get; set; }
    }
}
