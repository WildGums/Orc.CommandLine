﻿namespace Orc.CommandLine.Tests.Context
{
    public class TestContext : ContextBase
    {
        public TestContext()
        {
            StringSwitch = string.Empty;
        }

        [Option("bs", "bool", AcceptsValue = false, HelpText = "Some boolean switch")]
        public bool BooleanSwitch { get; set; }

        [Option("i", "int", HelpText = "Some integer switch")]
        public int IntegerSwitch { get; set; }

        [Option("s", "string", HelpText = "Some string switch")]
        public string StringSwitch { get; set; }

        [Option("x", "trimquotes", TrimQuotes = true)]
        public string TrimQuotes { get; set; }

        [Option("y", "donttrimquotes", TrimQuotes = false)]
        public string DontTrimQuotes { get; set; }
    }
}
